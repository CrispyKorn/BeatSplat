using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverlay : MonoBehaviour
{
    [SerializeField] private List<Image> _ballImages = new();

    private int _colorID = 0;
    private int _ballsLeft;
    private Theme _theme;

    private void Awake()
    {
        Locator.Instance.RegisterInstance(this);
    }

    private void Start()
    {
        foreach(var ballImage in _ballImages) ballImage.enabled = false;
        _theme = Locator.Instance.GameManager.Theme;
    }

    public void Setup()
    {
        foreach (var ballImage in _ballImages) ballImage.enabled = true;

        _ballsLeft = _ballImages.Count;
    }

    public void ChangeBallColours()
    {
        _colorID++;
        if (_colorID == _theme.BrickColours.Count) _colorID = 0;

        var actualColor = _theme.BrickColours[_colorID];

        foreach (var ballImage in _ballImages) ballImage.color = actualColor;
    }

    public void RemoveBall()
    {
        _ballsLeft--;

        if (_ballsLeft < 0)
        {
            Locator.Instance.GameManager.EndGame(false);
        }
        else _ballImages[_ballImages.Count - _ballsLeft - 1].enabled = false;
    }
}
