using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Tooltip("The colour theme to use.")]
    [SerializeField] private List<Theme> _themes;

    private Locator _locator;

    public Theme Theme { get; private set; }

    private void Awake()
    {
        _locator = Locator.Instance;
        _locator.RegisterInstance(this);
        ChangeTheme();
    }

    private void Start()
    {
        _locator.InputManager.SetCursorMode(true);
        _locator.InputManager.SetActionMap(InputManager.ActionMap.Menu);
    }

    public void StartGame()
    {
        _locator.BrickArea.ResetBricks();

        _locator.GameOverlay.Setup();
        _locator.GameOverlay.ChangeBallColours();
        _locator.GameOverlay.enabled = true;

        _locator.Paddle.CreateNewBall();

        _locator.InputManager.SetActionMap(InputManager.ActionMap.Player);
    }

    public void EndGame(bool won)
    {
        foreach (var ball in _locator.Paddle.ActiveBalls) Destroy(ball.gameObject);
        _locator.Paddle.ActiveBalls.Clear();
        _locator.PowerUpManager.KillPowerUps();

        ChangeTheme();
        _locator.Menu.gameObject.SetActive(true);
        _locator.InputManager.SetActionMap(InputManager.ActionMap.Menu);

        if (won) WinGame();
        else GameOver();
    }

    private void GameOver()
    {
        _locator.BrickArea.ResetBrickDensity();

        _locator.Menu.EnableGameOverScreen();
    }

    private void WinGame()
    {
        _locator.BrickArea.IncreaseBrickDensity();

        _locator.Menu.EnableWinScreen();
    }

    private void ChangeTheme()
    {
        Theme = _themes[Random.Range(0, _themes.Count)];
        SetThemeElements();
    }

    private void SetThemeElements()
    {
        // Set camera background to theme background
        var theme = Locator.Instance.GameManager.Theme;
        Camera.main.backgroundColor = theme.Background;

        foreach (var obj in GameObject.FindGameObjectsWithTag("Background"))
        {
            var spriteRenderer = obj.GetComponent<Renderer>();
            if (spriteRenderer != null) spriteRenderer.material.color = theme.Background;
        }

        // Set sprite color to theme forground for all objects with the "Foreground" tag
        foreach (var obj in GameObject.FindGameObjectsWithTag("Foreground"))
        {
            var spriteRenderer = obj.GetComponent<Renderer>();
            if (spriteRenderer != null) spriteRenderer.material.color = theme.Foreground;
        }
    }
}
