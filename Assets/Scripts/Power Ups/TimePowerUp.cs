﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimePowerUp : PowerUp
{

    /*
     * 
     * This is to change the time of the game
     * This could slow or speed up the game
     * 
     */


    [Header("Change time Scale")]
    [Tooltip("0.5 will speed up the time to 1.5 speed, -0.5 will slow the speed to 0.5")]
    [SerializeField]
    [Range(-1.0f, 1.0f)]
    private float SlowTime = -0.5f;


    public override void ActivatePowerUp()
    {
        Time.timeScale += SlowTime;
    }

    public override void DeactivatePowerUp()
    {
        Time.timeScale -= SlowTime;
    }
}