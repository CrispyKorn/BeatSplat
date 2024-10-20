using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityToBallPowerUp : PowerUp
{
    [Header("Gravity to apply to ball")]
    [SerializeField]
    [Range(0f, 1f)]
    private float gravity = 0.4f;

    public override void ActivatePowerUp()
    {
        powerUpManager.Controller.AddGravityToBall(gravity);
    }

    public override void DeactivatePowerUp()
    {
        powerUpManager.Controller.AddGravityToBall(-gravity);
    }
}
