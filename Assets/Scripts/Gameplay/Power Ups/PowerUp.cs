using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    protected PowerUpManager _powerUpManager;
    
    [Tooltip("Duration of a power-up in seconds.")]
    [SerializeField, Range(0f, 60f)] private float _duration = 5f;

    private Rigidbody2D _body;

    public float Duration => _duration;

    protected void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _powerUpManager = Locator.Instance.PowerUpManager;
    }

    private void OnBecameInvisible()
    {
        //This is for when the object is out of view
        Destroy(gameObject, 1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //This is for when the powerup hits the player
        if (collision.gameObject.layer == _powerUpManager.PlayerLayer)
        {
            _powerUpManager.ActivatePowerUp(this);
        }
    }

    public void SetGravity(float gravity)
    {
        _body.gravityScale = gravity;
    }

    public void Freeze()
    {
        //This is used to freeze the power up in a position
        _body.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public abstract void ActivatePowerUp();

    public abstract void DeactivatePowerUp();
}
