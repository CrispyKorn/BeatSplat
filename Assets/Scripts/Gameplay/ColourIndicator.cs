using UnityEngine;

public class ColourIndicator : MonoBehaviour
{
    private int _colorID = 0;
    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        SetNewColour();
    }

    private async void OnEnable()
    {
        while (Locator.Instance.BeatManager == null) await Awaitable.NextFrameAsync();

        Locator.Instance.BeatManager.OnBar += SetNewColour;
    }

    private void OnDisable()
    {
        Locator.Instance.BeatManager.OnBar -= SetNewColour;
    }

    private void SetNewColour()
    {
        var theme = Locator.Instance.GameManager.Theme;
        _colorID++;
        if (_colorID == theme.BrickColours.Count) _colorID = 0; // Loop colorID

        _renderer.material.color = theme.BrickColours[_colorID];
    }
}
