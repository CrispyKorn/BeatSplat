using System.Collections.Generic;
using UnityEngine;

public class Splat : MonoBehaviour
{
    /*
        This script is to control the function of the splats
      
        This will allow them to be set to a random size, sprite and rotation
        This also keeps track of objects it touches, this is so it can remove itself when it is no 
        longer needed, this is because it will only be seen through masks
      
        Collisions will only be setup to activate on bricks and other objects, but not the ball for example
    */

    [Tooltip("The minimum scaling size of the splat.")]
    [SerializeField] private float _minSize = 0.8f;
    [Tooltip("The maximum scaling size of the splat.")]
    [SerializeField] private float _maxSize = 1.5f;
    [Tooltip("The sprites to use as splats.")]
    [SerializeField] private Sprite[] _sprites;

    private SpriteRenderer _spriteRenderer;
    private List<string> _touchingObjects = new();
    private float _waitTime = 3f;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Update()
    {
        // This is to check if the object is on any objects, if not, delete self
        if (_waitTime >= 0) _waitTime -= Time.deltaTime;

        if (_touchingObjects.Count == 0 && _waitTime < 0f) Destroy(gameObject);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // This is to take note of any objects I am touching
        _touchingObjects.Add(collision.gameObject.name);
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        // This is to take note of any objects I am no longer touching
        _touchingObjects.Remove(collision.gameObject.name);
    }

    /// <summary>
    /// Used to setup the splat
    /// </summary>
    public void Setup()
    {
        SetupSize();
        SetupSprite();
        SetupRotation();

        _spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        _spriteRenderer.sortingOrder = 3;
    }

    /// <summary>
    /// Randomly set the sprite used
    /// </summary>
    private void SetupSprite()
    {
        _spriteRenderer.sprite = _sprites[Random.Range(0, _sprites.Length)];
    }

    /// <summary>
    /// Randomly set the size used
    /// </summary>
    private void SetupSize()
    {
        transform.localScale *= Random.Range(_minSize, _maxSize);
    }

    /// <summary>
    /// Randomdly set the rotation
    /// </summary>
    private void SetupRotation()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-360, 360));
    }
}
