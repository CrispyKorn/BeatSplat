using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEditor.VersionControl;
using UnityEngine;

public class SplatParticles : MonoBehaviour
{

    /*
     * 
     * This script is for controlling the partical effects
     * This allows splats to be created when a partical hits something
     * 
     */

    
    public GameObject splatPrefab;          //What prefab do I use to create a splatter?
    private static Transform splatHolder;           //Where do I store the splatter?


    private ParticleSystem splatParticleSystem;         //What is my own particle system
    private List<ParticleCollisionEvent> particleCollisionEvents = new List<ParticleCollisionEvent>();          //All the collisions


    private void Start()
    {
        splatParticleSystem = GetComponent<ParticleSystem>();

        if (splatHolder == null)
            splatHolder = new GameObject("Splat Holder").transform;
    }


    //THis for for when a partical hits something
    private void OnParticleCollision(GameObject other)
    {
        //Make sure it isnt the player or the ball
        if (other.layer != LayerMask.NameToLayer("Player") && other.layer != LayerMask.NameToLayer("Ball"))
        {
            ParticlePhysicsExtensions.GetCollisionEvents(splatParticleSystem, other, particleCollisionEvents);

            //For each event, create a splat
            foreach (ParticleCollisionEvent particle in particleCollisionEvents)
            {
                GameObject splat = Instantiate(splatPrefab, particle.intersection, Quaternion.identity);

                splat.transform.SetParent(splatHolder, true);
                Splat splatScript = splat.GetComponent<Splat>();

                splatScript.Setup();
            }
        }
    }
}
