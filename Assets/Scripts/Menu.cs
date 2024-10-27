using TMPro;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private string _playAgainText = "Play Again?";
    [SerializeField] private GameObject _gameOverText;
    [SerializeField] private TextMeshProUGUI _playButtonText;

    private BrickArea _brickArea;
    private GameOverlay _gameOverlay;
    private bool _resetGame = false;
    private Controller _controller;

    private void Awake()
    {
        Locator.Instance.RegisterInstance(this);
    }

    public void Start()
    {
        _brickArea = Locator.Instance.BrickArea;
        _gameOverlay = Locator.Instance.GameOverlay;
        _controller = Locator.Instance.Controller;

        _gameOverText.SetActive(false);

        if (_controller is Controller) _controller.PlayEnabled = false;
        else Debug.LogError("No controller found for main menu canvas!");
    }

    public void ReenableMainMenu()
    {
        gameObject.SetActive(true);
    }

    public void MoveToGameOverScreen()
    {
        _playButtonText.text = _playAgainText;
        _gameOverText.SetActive(true);

        _controller.PlayEnabled = false;
        _resetGame = true;

        Locator.Instance.InputManager.SetActionMap(InputManager.ActionMap.Menu);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void EnableGame()
    {
        if (_resetGame)
            _brickArea.ResetBricks();

        _resetGame = false;

        _controller.PlayEnabled = true;

        _gameOverlay.Setup();
        _gameOverlay.enabled = true;
        _gameOverlay.ChangeBallColours();

        gameObject.SetActive(false);

        Locator.Instance.InputManager.SetActionMap(InputManager.ActionMap.Player);
    }
}
