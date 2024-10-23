using UnityEngine;

public class PlayerResizePowerUp : PowerUp
{
    [Tooltip("This is for how big the player can be, -1 is half the size while 1 is double size.")]
    [SerializeField, Range(-2f, 2f)] private float _extendAmount = 0.5f;

    private float _extendedSize;

    public override void ActivatePowerUp()
    {
        _extendedSize = _powerUpManager.Controller.transform.localScale.x * _extendAmount;
        _powerUpManager.Controller.ExtendPaddle(_extendedSize);
    }

    public override void DeactivatePowerUp()
    {
        _powerUpManager.Controller.ExtendPaddle(-_extendedSize);
    }
}
