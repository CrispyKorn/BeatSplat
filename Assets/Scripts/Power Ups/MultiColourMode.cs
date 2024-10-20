using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiColourMode : PowerUp
{
    //Keeps track of the number of activates
    private static List<MultiColourMode> multiBallPU = new List<MultiColourMode>();

    //Activate the balls on screen and add to list
    public override void ActivatePowerUp()
    {
        multiBallPU.Add(this);
        powerUpManager.Controller.MultiColourMode(true);
    }

    //Deactivate the balls if this is the last one active
    public override void DeactivatePowerUp()
    {
        if(multiBallPU.Count == 1)
            powerUpManager.Controller.MultiColourMode(false);

        multiBallPU.Remove(this);
    }
}
