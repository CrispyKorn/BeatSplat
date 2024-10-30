using System.Collections.Generic;
using UnityEngine;
using System;

public class Paddle : MonoBehaviour
{
    [Tooltip("The actual paddle.")]
    [SerializeField] private GameObject _paddle;
    [Tooltip("The movespeed of the player paddle in units/sec.")]
    [SerializeField] private float _moveSpeed = 8f;
    [Tooltip("The ball to use.")]
    [SerializeField] private GameObject _ballPrefab;
    [SerializeField, Range(0f, 10f)] private float _moveLimit = 7.8f;

    [Header("Sounds")]
    [Tooltip("The sound that plays during the countdown.")]
    [SerializeField] private Sound _countdownSound;
    [Tooltip("The sound that plays when the ball is ready to be released.")]
    [SerializeField] private Sound _arpUpSound;
    [Tooltip("The sound that plays when all balls are lost.")]
    [SerializeField] private Sound _arpDownSound;

    private readonly List<Ball> _activeBalls = new();
    private int _countdown = -1;
    private PowerUpManager _powerUpManager;
    private Rigidbody2D _body;
    private Ball _heldBall;
    private BeatManager _beatManager;
    private Collider2D _paddleCollider;
    private MusicManager _musicManager;
    private SoundPlayer _soundPlayer;

    public GameObject PaddleObj { get => _paddle; }
    public List<Ball> ActiveBalls { get => _activeBalls; }

    private void Awake()
    {
        Locator.Instance.RegisterInstance(this);

        _paddleCollider = _paddle.GetComponent<Collider2D>();
        _soundPlayer = GetComponent<SoundPlayer>();
    }

    private async void OnEnable()
    {
        while (Locator.Instance.InputManager == null || Locator.Instance.BeatManager == null) await Awaitable.NextFrameAsync();

        Locator.Instance.InputManager.OnRelease += Release;
        Locator.Instance.InputManager.OnMove += Move;
        Locator.Instance.InputManager.OnMove_Ended += StopMove;
        Locator.Instance.BeatManager.OnBeat += HandleBeat;
        Locator.Instance.BeatManager.OnBar += HandleBar;
    }

    private void OnDisable()
    {
        Locator.Instance.InputManager.OnRelease -= Release;
        Locator.Instance.InputManager.OnMove -= Move;
        Locator.Instance.InputManager.OnMove_Ended -= StopMove;
        Locator.Instance.BeatManager.OnBeat -= HandleBeat;
        Locator.Instance.BeatManager.OnBar -= HandleBar;
    }

    private void Start() 
    {
        _body = GetComponent<Rigidbody2D>();
        _powerUpManager = Locator.Instance.PowerUpManager;
        _beatManager = Locator.Instance.BeatManager;
        _musicManager = Locator.Instance.MusicManager;
    }

    private void LateUpdate()
    {
        LimitMovement();
    }

    private void OnDrawGizmosSelected()
    {
        var halfLineHeight = 1f;
        var leftStart = new Vector2(-_moveLimit, transform.position.y - halfLineHeight);
        var leftEnd = new Vector2(-_moveLimit, transform.position.y + halfLineHeight);
        var rightStart = new Vector2(_moveLimit, transform.position.y - halfLineHeight);
        var rightEnd = new Vector2(_moveLimit, transform.position.y + halfLineHeight);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(leftStart, leftEnd);
        Gizmos.DrawLine(rightStart, rightEnd);
    }

    private void Release()
    {
        if (_heldBall == null) return;

        if (_countdown < 0)
        {
            _countdown = 5;
            _soundPlayer.SwitchSound(_arpUpSound);
            _soundPlayer.PlaySound(false, true);
        }
    }

    private void Move(float movement)
    {
        _body.linearVelocity = Vector2.right * _moveSpeed * movement;
    }

    private void StopMove()
    {
        _body.linearVelocity = Vector2.zero;
    }

    private void HandleBeat()
    {
        HandleReleaseCountdown();
    }

    private void HandleBar()
    {
        UpdateBallColour();
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

        if (_countdown == 4)
        {
            _soundPlayer.SwitchSound(_countdownSound);
            _soundPlayer.PlaySound(false, true);
        }

        if (_countdown == 0) ServeBall();
    }

    private void ServeBall()
    {
        _heldBall.transform.parent = null;
        _heldBall.Serve();
        _heldBall = null;
        _musicManager.SetMusic(true);
    }

    private void UpdateBallColour()
    {
        foreach (var ball in _activeBalls)
        {
            ball.SetNewColor();
            Locator.Instance.GameOverlay.ChangeBallColours();
        }
    }

    private void HandleBallLost()
    {
        _soundPlayer.SwitchSound(_arpDownSound);
        _soundPlayer.PlaySound();
        CreateNewBall();
        _powerUpManager.KillPowerUps();
        Locator.Instance.GameOverlay.RemoveBall();
    }

    private void LimitMovement()
    {
        var nextPos = _body.position + (_body.linearVelocity * Time.deltaTime);
        var paddleExtentsX = _paddleCollider.bounds.extents.x;

        if (nextPos.x + paddleExtentsX > _moveLimit)
        {
            _body.position = new Vector2(_moveLimit - paddleExtentsX, _body.position.y);
            _body.linearVelocity = Vector2.zero;
        }

        if (nextPos.x - paddleExtentsX < -_moveLimit)
        {
            _body.position = new Vector2(paddleExtentsX - _moveLimit, _body.position.y);
            _body.linearVelocity = Vector2.zero;
        }
    }

    public void CreateNewBall()
    {
        if (_heldBall != null) return;

        var heldPos = transform.position + Vector3.up;
        _heldBall = Instantiate(_ballPrefab, heldPos, Quaternion.identity, transform).GetComponent<Ball>();
        _activeBalls.Add(_heldBall);
        _heldBall.OnDestroyed += RemoveBall;
        _heldBall.Hold();
        _musicManager.SetMusic(false);
    }
}
