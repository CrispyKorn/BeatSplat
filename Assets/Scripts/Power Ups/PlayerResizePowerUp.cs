using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResizePowerUp : PowerUp
{
    [Header("This is for how big the player can be, -1 is half the size while 1 is double")]
    [SerializeField]
    [Range(-2f, 2f)]
    private float extendSize = 0.5f;

    private float extendedSize;

    public override void ActivatePowerUp()
    {
        extendedSize = powerUpManager.Controller.transform.localScale.x * extendSize;
        powerUpManager.Controller.ExtendPaddle(extendedSize);
    }

    public override void DeactivatePowerUp()
    {
        powerUpManager.Controller.ExtendPaddle(-extendedSize);
    }
}
