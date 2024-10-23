using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    protected PowerUpManager _powerUpManager;
    
    [Tooltip("Duration of a power-up in seconds.")]
    [SerializeField, Range(0f, 60f)] private float _duration = 5f;

    private Rigidbody2D _body;

    public float Duration => _duration;

    public void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
        _powerUpManager = Locator.Instance.PowerUpManager;
    }

    public void SetGravity(float gravity)
    {
        _body.gravityScale = gravity;
    }

    public abstract void ActivatePowerUp();
    public abstract void DeactivatePowerUp();

    public void OnBecameInvisible()
    {
        //This is for when the object is out of view
        Destroy(gameObject, 1f);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        //This is for when the powerup hits the player
        if (collision.gameObject.layer == _powerUpManager.PlayerLayer)
        {
            _powerUpManager.ActivatePowerUp(this);
        }
    }

    public void Freeze()
    {
        //This is used to freeze the power up in a position
        _body.constraints = RigidbodyConstraints2D.FreezeAll;
    }
}
