using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameOverlay : MonoBehaviour
{
    private int color = 0;
    public List<UnityEngine.UI.Image> balls = new List<UnityEngine.UI.Image>();
    public int ballsLeft;


    public void Awake()
    {
        foreach(UnityEngine.UI.Image ball in balls)
            ball.enabled = false;
    }


    public void Setup()
    {
        foreach (UnityEngine.UI.Image ball in balls)
            ball.enabled = true;

        ballsLeft = balls.Count;
    }


    public void ChangeBallColours()
    {
        color++;
        if (color == Menu.controller.theme.brickColours.Count)
            color = 0;

        Color actualColor = Menu.controller.theme.brickColours[color];

        foreach (UnityEngine.UI.Image ball in balls)
            ball.color = actualColor;
    }


    public void RemoveBall()
    {
        ballsLeft--;

        if(ballsLeft < 0)
        {
            Menu.ReenableMainMenu();
            Menu.MainMenu.GameOverScreen();
        }
        else
        {
            balls[balls.Count - ballsLeft - 1].enabled = false;
        }
    }
}
