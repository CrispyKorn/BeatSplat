using System.Collections.Generic;

public class MultiColourMode : PowerUp
{
    //Keeps track of the number of activates
    private static List<MultiColourMode> multiBallPU = new();

    //Activate the balls on screen and add to list
    public override void ActivatePowerUp()
    {
        multiBallPU.Add(this);
        _powerUpManager.Controller.MultiColourMode(true);
    }

    //Deactivate the balls if this is the last one active
    public override void DeactivatePowerUp()
    {
        if(multiBallPU.Count == 1) _powerUpManager.Controller.MultiColourMode(false);

        multiBallPU.Remove(this);
    }
}
