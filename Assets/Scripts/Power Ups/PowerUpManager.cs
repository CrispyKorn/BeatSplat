using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class PowerUpManager : MonoBehaviour
{

    /*
     * 
     * This script is for controlling powerups
     * This allows for power-ups to be created, move, actiavated, timed and deactivated
     * 
     */


    //This is for how many power ups to think ahead for, this makes sure we dont see the same power up in at least # of moves
    public int rememberedPowerUps = 3;


    //This is used for activated powerups, it holds the timer and power up
    //Allows the powerup manager to handle the timer and powerup position
    private struct PowerUpInfo
    {
        public PowerUp powerUp;
        public TimerBar timer;
    }


    private Controller gameManager = null;     //This is the controller, to contact and get relitive information


    [Header("Speed of falling power-ups")]
    [Range(0f, 2f)]
    public float powerUpGravity = 0.8f;


    [Header("Timerbar used to display the time left on a power-up")]
    public TimerBar timer;

    [Header("What power-pus are in play")]
    public List<PowerUp> powerUpList = new List<PowerUp>();

    private List<PowerUpInfo> activatedPowerUps = new List<PowerUpInfo>();
    private List<int> upComingPowerups = new List<int>();

    [Header("Location of powers when activated")]
    public List<Transform> powerUpHolders = new List<Transform>();


    //This is to get the player layer, this is used so a power up knows it hit the player and nothing else
    public int PlayerLayer
    {
        get => gameManager.gameObject.layer;
    }


    public Controller Controller => gameManager;


    private void Start()
    {
        gameManager = FindObjectOfType<Controller>();

        if (rememberedPowerUps > powerUpList.Count)
            rememberedPowerUps = powerUpList.Count;

        SelectNextPowerUps();
    }



    private void SelectNextPowerUps()
    {
        while(upComingPowerups.Count < rememberedPowerUps)
        {
            int next = Random.Range(0, powerUpList.Count);

            if(upComingPowerups.Contains(next))
            {
                continue;
            }

            upComingPowerups.Add(next);
        }
    }



    //This is used when the controller thinks a power up shoudl come out of a brick
    //Creates the power up and randomly chooses one, sets all the settings it needs
    public void HitBrick(Vector2 hitBrick)
    {
        if (powerUpList.Count > 0)
        {
            PowerUp powerUp = Instantiate(powerUpList[upComingPowerups[0]], transform);

            powerUp.powerUpManager = this;
            powerUp.SetGravity(powerUpGravity);

            powerUp.gameObject.transform.position = hitBrick;

            upComingPowerups.RemoveAt(0);
            SelectNextPowerUps();
        }
        else
        {
            Debug.LogError("No power ups attached to power up manager");
        }
    }


    //This is to activate a powerup
    public void ActivatePowerUp(PowerUp powerUp)
    {
        PowerUpInfo activatedPowerUp = new PowerUpInfo();

        if (activatedPowerUps.Count >= powerUpHolders.Count)            //Make sure there is a holder spare
        {                                                                   //If not, delete the first one
            RemoveActivatedPower(activatedPowerUps[0]);
        }

        powerUp.Freeze();               //Stop the movement of the power up
        powerUp.ActivatePowerUp();      //Activate the powerup
        activatedPowerUp.powerUp = powerUp;     //Move it to the power up info struct


        //Create the timer for the power up
        if (timer != null)
        {
            TimerBar tempTimer = Instantiate(timer, powerUp.transform);
            tempTimer.TimeLeft = powerUp.TimeLimit;
            tempTimer.Active = true;

            activatedPowerUp.timer = tempTimer;
        }
        else
        {
            Debug.Log("Missing Timer in Power-Up Manager!");
        }

        //Make sure the powerup and the timer are set, add to the list of activated power ups and also move to the correct location/holder
        if (activatedPowerUp.powerUp != null && activatedPowerUp.timer != null)
        {
            MoveToHolder(activatedPowerUp, powerUpHolders[activatedPowerUps.Count]);
            activatedPowerUps.Add(activatedPowerUp);
        }
    }


    //This is to deactivate and remove a power up
    private void RemoveActivatedPower(PowerUpInfo powerUp)
    {
        powerUp.powerUp.DeactivatePowerUp();
        
        Destroy(powerUp.powerUp.gameObject);
        Destroy(powerUp.timer.gameObject);

        activatedPowerUps.Remove(powerUp);
    }


    //This function moves a powerup to the correct position and scale
    private PowerUpInfo MoveToHolder(PowerUpInfo activatedPowerUp, Transform holder)
    {
        if(activatedPowerUp.powerUp.transform.parent != holder)         //Move the powerup to be a child of the holder
            activatedPowerUp.powerUp.transform.parent = holder;


        if (activatedPowerUp.powerUp.transform.localPosition.x != 0 && activatedPowerUp.powerUp.transform.localPosition.y != 0)
        {
            activatedPowerUp.powerUp.transform.localPosition = new Vector2(0, 0);
            activatedPowerUp.timer.transform.localPosition = new Vector2(0, 0);
            activatedPowerUp.powerUp.transform.localRotation = new Quaternion();
        }


        if( activatedPowerUp.powerUp.transform.localScale.x != 1)
            activatedPowerUp.powerUp.transform.localScale = new Vector2(1, 1);

        return activatedPowerUp;
    }



    public void KillPowerUps()
    {
        for(int i =0; i < activatedPowerUps.Count; i++)
        {
            RemoveActivatedPower(activatedPowerUps[i]);
            i--;
        }
    }


    
    private void Update()
    {
        if(activatedPowerUps.Count != 0)            //CHeck if any of the powerups need to be deactivated and removed
        {
            List<PowerUpInfo> removePowerUp = new List<PowerUpInfo>();

            for(int i = 0; i < activatedPowerUps.Count; i++)                //Check each activated powerup
            {
                if(activatedPowerUps[i].timer.TimeLeft <= 0 && activatedPowerUps[i].timer.Active)
                {
                    removePowerUp.Add(activatedPowerUps[i]);            //Add the power up to the remove list
                    continue;
                }

                MoveToHolder(activatedPowerUps[i], powerUpHolders[i]);      //Move power ups to the correct position
            }


            foreach(PowerUpInfo powerUpInfo in removePowerUp)       //Remove each power up that needs to die
            {
                RemoveActivatedPower(powerUpInfo);
            }
        }
    }
}
