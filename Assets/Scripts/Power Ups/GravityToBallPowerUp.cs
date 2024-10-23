using UnityEngine;

public class GravityToBallPowerUp : PowerUp
{
    [Tooltip("Gravity to apply to ball")]
    [SerializeField, Range(0f, 1f)] private float _gravity = 0.4f;

    public override void ActivatePowerUp()
    {
        _powerUpManager.Controller.AddGravityToBall(_gravity);
    }

    public override void DeactivatePowerUp()
    {
        _powerUpManager.Controller.AddGravityToBall(-_gravity);
    }
}
