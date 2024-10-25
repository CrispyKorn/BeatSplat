using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour 
{
    [Tooltip("Colour ID in theme's brick colour list.")]
    [SerializeField] private int _colorId;

    private readonly float _flashDuration = 0.1f;
    private float _lastFlashTime = -1000f;
    private List<Brick> _neighbours = new();
    private SpriteRenderer _sprite;
    private Controller _controller;

    private void Start() 
    {
        _controller = Locator.Instance.Controller;
        _sprite = GetComponent<SpriteRenderer>();
        _colorId = _controller.Theme.RandomColourID;
    }

    private void Update()
    {
        float secondsSinceFlash = Mathf.Min(Time.time - _lastFlashTime, _flashDuration);
        Color mainColor = _controller.Theme.brickColours[_colorId];
        _sprite.color = Color.Lerp(_controller.Theme.foreground, mainColor, secondsSinceFlash / _flashDuration);
    }

    private void HandleFlashSpread(List<Brick> affectedBricks)
    {
        _lastFlashTime = Time.time;
        affectedBricks.Add(this);
        DelayFlashSpread(affectedBricks);
    }

    private async void DelayFlashSpread(List<Brick> affectedBricks)
    {
        await Awaitable.WaitForSecondsAsync(0.01f);

        foreach (var brick in _neighbours.FindAll(b => !affectedBricks.Contains(b)))
        {
            brick.HandleFlashSpread(affectedBricks);
        }
    }

    public void AddNeighbour(Brick brick)
    {
        if (brick != null) _neighbours.Add(brick);
    }

    public void HandleBounce(int ballColor)
    {
        if (_colorId == ballColor)
        {
            _controller.HandleBrickBreak(transform.position);
            foreach (var brick in _neighbours)
            {
                brick._neighbours.Remove(this);
                brick.StartFlash();
            }

            Destroy(gameObject);
        }
        else StartFlash();
    }

    public void StartFlash()
    {
        HandleFlashSpread(new List<Brick>());
    }
}