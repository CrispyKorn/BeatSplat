

public class MultiBallPowerUp : PowerUp
{
    public override void ActivatePowerUp()
    {
        _powerUpManager.Controller.CreateNewBall();
    }

    public override void DeactivatePowerUp()
    {

    }
}
