using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverlay : MonoBehaviour
{
    [SerializeField] private List<Image> _balls = new();

    private int _colorID = 0;
    private int ballsLeft;

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

        ballsLeft = _balls.Count;
    }

    public void ChangeBallColours()
    {
        _colorID++;
        if (_colorID == Locator.Instance.Controller.Theme.brickColours.Count) _colorID = 0;

        Color actualColor = Locator.Instance.Controller.Theme.brickColours[_colorID];

        foreach (Image ball in _balls) ball.color = actualColor;
    }

    public void RemoveBall()
    {
        ballsLeft--;

        if (ballsLeft < 0)
        {
            Locator.Instance.Menu.ReenableMainMenu();
            Locator.Instance.Menu.MoveToGameOverScreen();
        }
        else _balls[_balls.Count - ballsLeft - 1].enabled = false;
    }
}
