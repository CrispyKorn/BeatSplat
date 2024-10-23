using UnityEngine;

public class Ball : MonoBehaviour 
{
    [Tooltip("The particle system used to create splats.")]
    [SerializeField] private ParticleSystem _splatParticalSystemPrefab;
    [Tooltip("The speed of the ball in units/sec.")]
    [SerializeField] private float _speed = 3f;

    private static ParticleSystem s_splatParticalSystem = null;

    private Collider2D _ballCollider;
    private Rigidbody2D _ballBody;
    private SpriteRenderer _ballRenderer;
    private bool _multiColourMode = false;
    private int _colorID = 0; // Current colour ID in theme's brick colour list
    private Controller _controller;

    public bool Served { get; private set; } = false;

    private void Awake()
    {
        _ballCollider = GetComponent<Collider2D>();
        _ballBody = GetComponent<Rigidbody2D>();
        _ballRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start() 
    {
        _controller = Locator.Instance.Controller;
        SetNewColor();

        if (_splatParticalSystemPrefab != null && s_splatParticalSystem == null)
        {
            s_splatParticalSystem = Instantiate(_splatParticalSystemPrefab, transform.parent);
        }
        else if (_splatParticalSystemPrefab == null)
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
        _ballBody.linearVelocity = new Vector2(Random.Range(-1f, 1f), 1f).normalized * _speed;
    }

    void OnCollisionEnter2D(Collision2D hit) 
    {
        var brick = hit.gameObject.GetComponent<Brick>(); // Try to find brick component of what we hit

        _controller.HandleBounce(transform.position);

        if (hit.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            foreach (ContactPoint2D contact in hit.contacts)
            {
                SplatterPaint(contact.point);
            }
        }

        if (brick != null) brick.HandleBounce(_colorID);
    }

    private void SplatterPaint(Vector2 hit)
    {
        if (s_splatParticalSystem != null)
        {
            s_splatParticalSystem.transform.position = hit;
            s_splatParticalSystem.Play();
        }
        else Debug.LogError("Missing splatter Partical System on ball!");
    }

    //This is for a powerup that I dont think is fun
    public void ApplyGravity(float gravity) 
    {
        _ballBody.gravityScale += gravity;
    }

    public void SetMultiColourMode(bool mode) 
    {
        _multiColourMode = mode;
    }
}
