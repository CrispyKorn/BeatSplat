using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public static Controller controller;
    public static Menu MainMenu;
    public static GameOverlay gameOverlay;
    private static GameObject mainMenu;

    public string playAgainText = "Play Again?";

    private GameObject gameOverText;
    private Text playButtonText;

    private BrickArea brickArea;

    private bool resetGame = false;
    


    public void Start()
    {
        brickArea = FindObjectOfType<BrickArea>();

        MainMenu = this;
        mainMenu = gameObject;
        gameOverlay = FindObjectOfType<GameOverlay>();

        gameOverText = GameObject.Find("Game Over");
        gameOverText.SetActive(false);
        playButtonText = GameObject.Find("Play Game").GetComponentInChildren<Text>();


        controller = FindObjectOfType<Controller>();

        if (controller != null)
            controller.enablePlay = false;
        else
            Debug.LogError("No controller found for main menu canvas!");
    }

    public static void ReenableMainMenu()
    {
        mainMenu.SetActive(true);
    }

    public void GameOverScreen()
    {
        playButtonText.text = playAgainText;
        gameOverText.SetActive(true);

        controller.enablePlay = false;
        resetGame = true;
    }


    public void Exit()
    {
        Application.Quit();
    }


    public void EnableGame()
    {
        if (resetGame)
            brickArea.ResetBricks();

        resetGame = false;

        controller.enablePlay = true;

        gameOverText.SetActive(true);


        gameOverlay.Setup();
        gameOverlay.enabled = true;
        gameOverlay.ChangeBallColours();

        gameObject.SetActive(false);
    }
}
