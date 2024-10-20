using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiBallPowerUp : PowerUp
{
    private GameObject createdBall;

    public override void ActivatePowerUp()
    {
        powerUpManager.Controller.NewBall();
    }

    public override void DeactivatePowerUp()
    {
    }
}
