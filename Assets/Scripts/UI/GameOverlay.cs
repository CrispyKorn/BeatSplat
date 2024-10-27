using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverlay : MonoBehaviour
{
    [SerializeField] private List<Image> _balls = new();

    private int _colorID = 0;
    private int _ballsLeft;

    private void Awake()
    {
        Locator.Instance.RegisterInstance(this);
    }

    public void Start()
    {
        foreach(Image ball in _balls) ball.enabled = false;
    }

    public void Setup()
    {
        foreach (Image ball in _balls) ball.enabled = true;

        _ballsLeft = _balls.Count;
    }

    public void ChangeBallColours()
    {
        _colorID++;
        if (_colorID == Locator.Instance.Paddle.Theme.brickColours.Count) _colorID = 0;

        Color actualColor = Locator.Instance.Paddle.Theme.brickColours[_colorID];

        foreach (Image ball in _balls) ball.color = actualColor;
    }

    public void RemoveBall()
    {
        _ballsLeft--;

        if (_ballsLeft < 0)
        {
            Locator.Instance.Menu.ReenableMainMenu();
            Locator.Instance.Menu.MoveToGameOverScreen();
        }
        else _balls[_balls.Count - _ballsLeft - 1].enabled = false;
    }
}
