using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    /*
     * 
     * This script is for powerups
     * This is the base for how a powerup can be interacted with
     * 
     */

    //This is a link back to the powerup manager
    [HideInInspector]
    public PowerUpManager powerUpManager;

    
    [Header("Length of a power-up")]
    [SerializeField]
    [Range(0f, 60f)]
    private float timeLimit = 5f;

    //Needed to change the gravity
    private Rigidbody2D body;


    public float TimeLimit => timeLimit;


    public void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    //This is to set the gravity of the power up, the manager interacts with this
    public void SetGravity(float gravity)
    {
        body.gravityScale = gravity;
    }


    //This it to activate the power up and to deactivate
    public abstract void ActivatePowerUp();
    public abstract void DeactivatePowerUp();


    //THis is for when the object is out of view
    public void OnBecameInvisible()
    {
        GameObject.Destroy(gameObject, 1f);
    }


    //This if for when the powerup hits the player
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == powerUpManager.PlayerLayer)
        {
            powerUpManager.ActivatePowerUp(this);
        }
    }


    //This is used to freeze the power up in a position
    public void Freeze()
    {
        body.constraints = RigidbodyConstraints2D.FreezeAll;
    }
}
