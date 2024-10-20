using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerBar : MonoBehaviour
{
    private float maxTime = 100f;
    private float timeLeft;
    private bool active = false;

    private Transform frontOfBar;


    //This is used to allow the timer to tick forward, if not set, it wont move
    [HideInInspector]
    public bool Active { get => active; set => active = value; }

    public float TimeLeft
    {
        get => timeLeft;
        set
        {
            maxTime = value;
            timeLeft = value;
        }
    }


    // Start is called before the first frame update
    void Awake()
    {
        frontOfBar = gameObject.transform.Find("Frontbar");
    }



    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            timeLeft -= Time.unscaledDeltaTime;         //move the time down

            if (timeLeft < 0)           //Make sure the timer doesnt loop around past zero
            {                               //Shouldnt be an issue, but to be safe
                timeLeft = 0;
            }

            Vector2 temp = frontOfBar.localScale;           //Work out where to move the slider to
            temp.x = timeLeft / maxTime;

            frontOfBar.localScale = temp;
        }
    }
}
