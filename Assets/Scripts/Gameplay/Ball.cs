using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour 
{
    public static float s_speedMultiplier = 1f;

    [Tooltip("The particle system used to create splats.")]
    [SerializeField] private ParticleSystem _splatParticalSystemPrefab;
    [Tooltip("The speed of the ball in units/sec.")]
    [SerializeField] private float _speed = 3f;

    private static ParticleSystem s_splatParticalSystem = null;

    private Collider2D _ballCollider;
    private Rigidbody2D _ballBody;
    private SpriteRenderer _ballRenderer;
    private TrailRenderer _trailRenderer;
    private int _colorID = 0; // Current colour ID in theme's brick colour list
    private Paddle _controller;

    public bool Served { get; private set; } = false;

    public event Action<Ball> OnDestroyed;

    private void Awake()
    {
        _ballCollider = GetComponent<Collider2D>();
        _ballBody = GetComponent<Rigidbody2D>();
        _ballRenderer = GetComponent<SpriteRenderer>();
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    private void Start() 
    {
        _controller = Locator.Instance.Paddle;
        SetNewColor();

        if (_splatParticalSystemPrefab is ParticleSystem && s_splatParticalSystem is null)
        {
            s_splatParticalSystem = Instantiate(_splatParticalSystemPrefab, transform.parent);
        }
        else if (_splatParticalSystemPrefab is null)
        {
            Debug.LogError("Missing Splatter Partical Ssytem Prefab on ball!");
        }
    }

    public void SetNewColor() 
    {
        _colorID++;
        if (_colorID == _controller.Theme.brickColours.Count) _colorID = 0; // Loop colorID

        Color actualColor = _controller.Theme.brickColours[_colorID];
        _ballRenderer.color = actualColor;
        _trailRenderer.startColor = actualColor;
        _trailRenderer.endColor = actualColor;
    }

    public void Hold() 
    {
        Served = false;
        _ballCollider.isTrigger = true;
        _ballBody.bodyType = RigidbodyType2D.Kinematic;
        _ballBody.linearVelocity = Vector2.zero;
    }

    public void Serve() 
    {
        Served = true;
        _ballCollider.isTrigger = false;
        _ballBody.bodyType = RigidbodyType2D.Dynamic;
        _ballBody.linearVelocity = new Vector2(UnityEngine.Random.Range(-1f, 1f), 1f).normalized;
    }

    private void LateUpdate()
    {
        if (Served) _ballBody.linearVelocity = _ballBody.linearVelocity.normalized * _speed * s_speedMultiplier;
    }

    void OnCollisionEnter2D(Collision2D hit)
    {
        var brick = hit.gameObject.GetComponent<Brick>(); // Try to find brick component of what we hit

        _controller.HandleBounce(transform.position);

        if (hit.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            foreach (ContactPoint2D contact in hit.contacts)
            {
                SplatterPaint(new Vector3(contact.point.x, contact.point.y, 0f));
            }
        }

        if (brick is Brick) brick.HandleBounce(_colorID);
    }

    private void SplatterPaint(Vector3 hit)
    {
        if (s_splatParticalSystem is ParticleSystem)
        {
            s_splatParticalSystem.transform.position = hit;
            s_splatParticalSystem.Play();
        }
        else Debug.LogError("Missing splatter Partical System on ball!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DeathZone"))
        {
            OnDestroyed?.Invoke(this);
            Destroy(gameObject);
        }
    }
}
