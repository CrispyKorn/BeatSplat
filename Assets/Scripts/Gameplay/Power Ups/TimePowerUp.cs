using UnityEngine;

public class TimePowerUp : PowerUp
{
    /*
        This is to change the time of the game
        This could slow or speed up the game
    */

    [Header("Change Time Scale")]
    [Tooltip("How much to speed up / slow down time.")]
    [SerializeField, Range(0f, 2f)] private float _speedMultiplier = 1f;

    private Paddle _paddle;

    new private void Awake()
    {
        base.Awake();

        _paddle = Locator.Instance.Paddle;
    }

    public override void ActivatePowerUp()
    {
        Ball.s_speedMultiplier *= _speedMultiplier;
    }

    public override void DeactivatePowerUp()
    {
        Ball.s_speedMultiplier *= 1f / _speedMultiplier;
    }
}
