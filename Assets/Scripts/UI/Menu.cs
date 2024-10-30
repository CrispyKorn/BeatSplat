using TMPro;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private string _playAgainText = "Play Again";
    [SerializeField] private GameObject _gameOverText;
    [SerializeField] private GameObject _gameWinText;
    [SerializeField] private TextMeshProUGUI _playButtonText;

    private void Awake()
    {
        Locator.Instance.RegisterInstance(this);
    }

    private void Start()
    {
        _gameOverText.SetActive(false);
        _gameWinText.SetActive(false);
    }

    public void StartGame()
    {
        gameObject.SetActive(false);
        Locator.Instance.GameManager.StartGame();
    }

    public void EnableGameOverScreen()
    {
        _playButtonText.text = _playAgainText;
        _gameWinText.SetActive(false);
        _gameOverText.SetActive(true);
    }

    public void EnableWinScreen()
    {
        _playButtonText.text = _playAgainText;
        _gameOverText.SetActive(false);
        _gameWinText.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
