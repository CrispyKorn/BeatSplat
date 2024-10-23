using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Controller : MonoBehaviour 
{
    [HideInInspector] public bool PlayEnabled = true;

    [Tooltip("The theme to use for the game.")]
    [SerializeField] private Theme _theme;
    [Tooltip("The movespeed of the player paddle in units/sec.")]
    [SerializeField] private float _moveSpeed = 8f;
    [Tooltip("The ball to use.")]
    [SerializeField] private GameObject _ballPrefab;
    [Tooltip(".")]
    [SerializeField] private GameObject _changeIndicator;
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
    [Tooltip(".")]
    [SerializeField] private AudioClip _arpUpSound;
    [Tooltip(".")]
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
    private float _lastTickMod;
    private int _beats = 0; // Metronome pulses
    private float _lastBeatMod;

    public Theme Theme { get => _theme; }

    private void Awake()
    {
        Locator.Instance.RegisterInstance(this);
    }

    private async void OnEnable()
    {
        while (Locator.Instance.InputManager is null) await Awaitable.NextFrameAsync();

        Locator.Instance.InputManager.OnRelease += Release;
        Locator.Instance.InputManager.OnMove += Move;
        Locator.Instance.InputManager.OnMove_Ended += StopMove;
    }

    private void OnDisable()
    {
        Locator.Instance.InputManager.OnRelease -= Release;
        Locator.Instance.InputManager.OnMove -= Move;
        Locator.Instance.InputManager.OnMove_Ended -= StopMove;
    }

    private void Start() 
    {
        _body = GetComponent<Rigidbody2D>();
        _powerUpManager = Locator.Instance.PowerUpManager;

        // Set camera background to theme background
        Camera.main.backgroundColor = Theme.background;

        // Set sprite color to theme forground for all objects with the "Foreground" tag
        foreach (var obj in GameObject.FindGameObjectsWithTag("Foreground"))
        {
            var sprite = obj.GetComponent<SpriteRenderer>();
            if (sprite != null) sprite.color = Theme.foreground; 
        }

        CreateNewBall();
    }

    private void Update() 
    {
        HandleMusicSync();

        RemoveInactiveBalls();

        HandleReset();

        //Set music based on whether all balls have been launched
        if (_activeBalls.FindAll(b => b.Served).Count == 0)
        {
            _preLaunchLoop.volume = 1;
            _postLaunchLoop.volume = 0;
        }
        else
        {
            _preLaunchLoop.volume = 0;
            _postLaunchLoop.volume = 1;
        }

        // If we have a ball stuck to the paddle
        if (_heldBall is Ball) 
        {
            // Move the ball to where the paddle is
            _heldBall.gameObject.transform.position = transform.position + Vector3.up * 0.5f;
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

    public void CreateNewBall()
    {
        if (_heldBall == null) 
        {
            _heldBall = Instantiate(_ballPrefab).GetComponent<Ball>();
            _activeBalls.Add(_heldBall);
            _heldBall.Hold();
        }
    }

    private void HandleMusicSync()
    {
        // Music sync
        var tickMod = _preLaunchLoop.time % (0.5f / 4f);

        if (tickMod < _lastTickMod)
        {
            foreach (var clip in _playNextTick) _melodySource.PlayOneShot(clip);
            _playNextTick.Clear();
        }

        _lastTickMod = tickMod;

        var beatMod = _preLaunchLoop.time % 0.5f;

        if (beatMod < _lastBeatMod)
        {
            _beats++;
            _countdown--;

            if (_countdown == 4) _sfxSource.PlayOneShot(_countdownSound);

            if (_countdown == 0)
            {
                _heldBall.Serve();
                _heldBall = null;
            }

            if (_beats % 4 == 0)
            {
                foreach (var ball in _activeBalls)
                {
                    ball.SetNewColor();
                    Locator.Instance.GameOverlay.ChangeBallColours();
                }
            }
        }

        _lastBeatMod = beatMod;

        _changeIndicator.transform.localScale = new Vector3(1, (_preLaunchLoop.time % 2f) / 2f * 10f, 1);

        // Pulse camera in time with music
        Camera.main.orthographicSize = 5f + (_preLaunchLoop.time % 2f) / 8f + (_preLaunchLoop.time % 0.5f) / 4f;
    }

    private void RemoveInactiveBalls()
    {
        // Remove all balls below the game area
        var outOfBounds = _activeBalls.Where(b => b.transform.position.y < -6);
        foreach (var ball in outOfBounds) Destroy(ball.gameObject);
        _activeBalls.RemoveAll(b => outOfBounds.Contains(b));
    }

    private void HandleReset()
    {
        // If there's no active balls, attach a new one to the paddle
        if (_activeBalls.Count == 0)
        {
            _sfxSource.PlayOneShot(_arpDownSound);
            CreateNewBall();
            _powerUpManager.KillPowerUps();
            Locator.Instance.GameOverlay.RemoveBall();
        }
    }

    public void HandleBrickBreak(Vector3 location)
    {
        if (_powerUpManager == null)
        {
            Debug.LogError("Controller cannot find the powerup manager!");
            return;
        }

        if (UnityEngine.Random.Range(0, 1) <= _powerUpPercentage) _powerUpManager.HitBrick(location);
    }

    public void HandleBounce(Vector3 location) 
    {
        var clip = _bounceSounds[UnityEngine.Random.Range(0, _bounceSounds.Count)];
        _playNextTick.Add(clip);
    }

    //Remove if we remove the gravity power up
    public void AddGravityToBall(float gravity)
    {
        foreach(var ball in _activeBalls)
        {
            ball.ApplyGravity(gravity);
        }
    }

    public void ExtendPaddle(float extend) 
    {
        transform.localScale += new Vector3(extend, 0);
    }

    public void MultiColourMode(bool mode)
    {
        foreach(var ball in _activeBalls)
        {
            ball.SetMultiColourMode(mode);
        }
    }
}
