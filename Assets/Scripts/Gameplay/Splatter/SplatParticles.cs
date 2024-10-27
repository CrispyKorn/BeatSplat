using System.Collections.Generic;
using UnityEngine;

public class SplatParticles : MonoBehaviour
{
    /*
        This script is for controlling the partical effects
        This allows splats to be created when a partical hits something
    */

    [Tooltip("The prefab to use for the splatter.")]
    [SerializeField] private GameObject _splatPrefab;

    private static Transform _splatHolder;

    private ParticleSystem _splatParticleSystem;
    private List<ParticleCollisionEvent> _particleCollisionEvents = new();

    private void Start()
    {
        _splatParticleSystem = GetComponent<ParticleSystem>();

        if (_splatHolder == null) _splatHolder = new GameObject("Splat Holder").transform;
    }

    private void OnParticleCollision(GameObject other)
    {
        //Make sure it isnt the player or the ball
        if (other.layer != LayerMask.NameToLayer("Player") && other.layer != LayerMask.NameToLayer("Ball"))
        {
            ParticlePhysicsExtensions.GetCollisionEvents(_splatParticleSystem, other, _particleCollisionEvents);

            //For each event, create a splat
            foreach (var particle in _particleCollisionEvents)
            {
                GameObject splat = Instantiate(_splatPrefab, particle.intersection, Quaternion.identity);

                splat.transform.SetParent(_splatHolder, true);
                var splatScript = splat.GetComponent<Splat>();

                splatScript.Setup();
            }
        }
    }
}
