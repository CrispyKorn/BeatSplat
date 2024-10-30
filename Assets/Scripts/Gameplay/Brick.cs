using System;
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
    private Theme _theme;

    public event Action OnDestroyed;

    private void Start() 
    {
        _sprite = GetComponent<SpriteRenderer>();
        _theme = Locator.Instance.GameManager.Theme;
        _colorId = _theme.RandomColourID;
    }

    private void Update()
    {
        var secondsSinceFlash = Mathf.Min(Time.time - _lastFlashTime, _flashDuration);
        var mainColor = _theme.BrickColours[_colorId];
        _sprite.color = Color.Lerp(_theme.Foreground, mainColor, secondsSinceFlash / _flashDuration);
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
        if (ballColor != _colorId) return;

        if (UnityEngine.Random.value <= Locator.Instance.PowerUpManager.PowerUpPercentage) Locator.Instance.PowerUpManager.SpawnPowerup(transform.position);

        foreach (var brick in _neighbours)
        {
            brick._neighbours.Remove(this);
            brick.StartFlash();
        }

        OnDestroyed?.Invoke();
        Destroy(gameObject);
    }

    public void StartFlash()
    {
        HandleFlashSpread(new List<Brick>());
    }
}