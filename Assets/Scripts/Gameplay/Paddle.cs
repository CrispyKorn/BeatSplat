using System.Collections.Generic;
using UnityEngine;
using System;

public class Paddle : MonoBehaviour
{
    [HideInInspector] public bool PlayEnabled = true;

    [Tooltip("The actual paddle.")]
    [SerializeField] private GameObject _paddle;
    [Tooltip("The theme to use for the game.")]
    [SerializeField] private Theme _theme;
    [Tooltip("The movespeed of the player paddle in units/sec.")]
    [SerializeField] private float _moveSpeed = 8f;
    [Tooltip("The ball to use.")]
    [SerializeField] private GameObject _ballPrefab;
    [Tooltip("Music that plays preceeding ball launch.")]
    [SerializeField] private AudioSource _preLaunchLoop;
    [Tooltip("Music that plays post ball launch.")]
    [SerializeField] private AudioSource _postLaunchLoop;
    [Tooltip(".")]
    [SerializeField] private AudioSource _sfxSource;
    [Tooltip(".")]
    [SerializeField] private AudioSource _melodySource;
    [Tooltip("The sound that plays during the countdown.")]
    [SerializeField] private AudioClip _countdownSound;
    [Tooltip("The sound that plays when the ball is ready to be released.")]
    [SerializeField] private AudioClip _arpUpSound;
    [Tooltip("The sound that plays when all balls are lost.")]
    [SerializeField] private AudioClip _arpDownSound;
    [Tooltip("The sounds that play when a ball hits the paddle.")]
    [SerializeField] private List<AudioClip> _bounceSounds;
    [Tooltip("Percentage of dropping power-ups. 1 is 100%, 0 is 0% chance of dropping power ups when a brick is destroyed.")]
    [SerializeField, Range(0f, 1f)] private float _powerUpPercentage = 0.5f;

    private readonly List<Ball> _activeBalls = new();
    private int _countdown = -1;
    private List<AudioClip> _playNextTick = new();
    private PowerUpManager _powerUpManager = null;
    private Rigidbody2D _body;
    private Ball _heldBall;
    private BeatManager _beatManager;

    public Theme Theme { get => _theme; }
    public GameObject PaddleObj { get => _paddle; }
    public List<Ball> ActiveBalls { get => _activeBalls; }

    private void Awake()
    {
        Locator.Instance.RegisterInstance(this);
    }

    private async void OnEnable()
    {
        while (Locator.Instance.InputManager is null || Locator.Instance.BeatManager is null) await Awaitable.NextFrameAsync();

        Locator.Instance.InputManager.OnRelease += Release;
        Locator.Instance.InputManager.OnMove += Move;
        Locator.Instance.InputManager.OnMove_Ended += StopMove;
        Locator.Instance.BeatManager.OnSemiQuaver += HandleTick;
        Locator.Instance.BeatManager.OnBeat += HandleBeat;
    }

    private void OnDisable()
    {
        Locator.Instance.InputManager.OnRelease -= Release;
        Locator.Instance.InputManager.OnMove -= Move;
        Locator.Instance.InputManager.OnMove_Ended -= StopMove;
        Locator.Instance.BeatManager.OnSemiQuaver -= HandleTick;
        Locator.Instance.BeatManager.OnBeat -= HandleBeat;
    }

    private void Start() 
    {
        _body = GetComponent<Rigidbody2D>();
        _powerUpManager = Locator.Instance.PowerUpManager;
        _beatManager = Locator.Instance.BeatManager;

        // Set camera background to theme background
        Camera.main.backgroundColor = Theme.background;
        foreach (var obj in GameObject.FindGameObjectsWithTag("Background"))
        {
            var spriteRenderer = obj.GetComponent<SpriteRenderer>();
            if (spriteRenderer is SpriteRenderer) spriteRenderer.color = Theme.background;
        }

        // Set sprite color to theme forground for all objects with the "Foreground" tag
        foreach (var obj in GameObject.FindGameObjectsWithTag("Foreground"))
        {
            var spriteRenderer = obj.GetComponent<SpriteRenderer>();
            if (spriteRenderer is SpriteRenderer) spriteRenderer.color = Theme.foreground; 
        }

        CreateNewBall();
    }

    private void LateUpdate()
    {
        var nextPos = _body.position + (_body.linearVelocity * Time.deltaTime);
        var paddleExtentsX = _paddle.GetComponent<BoxCollider2D>().bounds.extents.x;
        var maxX = 7.8f;
        if (nextPos.x + paddleExtentsX > maxX)
        {
            _body.position = new Vector2(maxX - paddleExtentsX, _body.position.y);
            _body.linearVelocity = Vector2.zero;
        }

        if (nextPos.x - paddleExtentsX < -maxX)
        {
            _body.position = new Vector2(paddleExtentsX - maxX, _body.position.y);
            _body.linearVelocity = Vector2.zero;
        }
    }

    private void Release()
    {
        if (_heldBall is null) return;

        // Serve the ball when fire is pressed, and release the reference to the ball
        if (_countdown < 0 && PlayEnabled)
        {
            _countdown = 5;
            _sfxSource.PlayOneShot(_arpUpSound);
        }
    }

    private void Move(float movement)
    {
        if (!PlayEnabled) return;

        _body.linearVelocity = Vector2.right * _moveSpeed * movement;
    }

    private void StopMove()
    {
        _body.linearVelocity = Vector2.zero;
    }

    
    private void HandleTick(int obj)
    {
        PlayBounceSound();
    }

    private void HandleBeat(int obj)
    {
        UpdateBallColour();
        HandleReleaseCountdown();
    }

    private void RemoveBall(Ball ball)
    {
        ball.OnDestroyed -= RemoveBall;
        _activeBalls.Remove(ball);
        if (_activeBalls.Count == 0) HandleBallLost();
    }

    private void HandleReleaseCountdown()
    {
        if (_countdown < 0) return;
        
        _countdown--;

        if (_countdown == 4) _sfxSource.PlayOneShot(_countdownSound);

        if (_countdown == 0) ServeBall();
    }

    private void ServeBall()
    {
        _heldBall.transform.parent = null;
        _heldBall.Serve();
        _heldBall = null;
        _preLaunchLoop.volume = 0;
        _postLaunchLoop.volume = 1;
    }

    private void UpdateBallColour()
    {
        if (_beatManager.BeatCount % 4 != 0) return;

        foreach (var ball in _activeBalls)
        {
            ball.SetNewColor();
            Locator.Instance.GameOverlay.ChangeBallColours();
        }
    }

    private void PlayBounceSound()
    {
        foreach (var clip in _playNextTick) _melodySource.PlayOneShot(clip);
        _playNextTick.Clear();
    }

    private void HandleBallLost()
    {
        _sfxSource.PlayOneShot(_arpDownSound);
        CreateNewBall();
        _powerUpManager.KillPowerUps();
        Locator.Instance.GameOverlay.RemoveBall();
    }

    public void CreateNewBall()
    {
        if (_heldBall is Ball) return;

        var heldPos = transform.position + Vector3.up * 0.5f;
        _heldBall = Instantiate(_ballPrefab, heldPos, Quaternion.identity, transform).GetComponent<Ball>();
        _activeBalls.Add(_heldBall);
        _heldBall.OnDestroyed += RemoveBall;
        _heldBall.Hold();
        _preLaunchLoop.volume = 1;
        _postLaunchLoop.volume = 0;
    }

    public void HandleBrickBreak(Vector3 location)
    {
        if (_powerUpManager is null)
        {
            Debug.LogError("Controller cannot find the powerup manager!");
            return;
        }

        if (UnityEngine.Random.value <= _powerUpPercentage) _powerUpManager.SpawnPowerup(location);
    }

    public void HandleBounce(Vector3 location) 
    {
        var clip = _bounceSounds[UnityEngine.Random.Range(0, _bounceSounds.Count)];
        _playNextTick.Add(clip);
    }

    public void ExtendPaddle(float extend) 
    {
        _paddle.transform.localScale += new Vector3(extend, 0f, 0f);
    }
}
