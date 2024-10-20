using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Splat : MonoBehaviour
{
    /*
     * This script is to control the function of the splats
     * 
     * This will allow them to be set to a random size, sprite and rotation
     * This also keeps track of objects it touches, this is so it can remove itself when it is no 
     * longer needed, this is because it will only be seen through masks
     * 
     * Collisions will only be setup to activate on bricks and other objects, but not the ball for example
     * 
     */


    public float minSize = 0.8f;                //Min size I can be scaled
    public float maxSize = 1.5f;                //Max size I can be scaled


    public Sprite[] sprites;                    //Holds all the sprites this object can use
    private SpriteRenderer spriteRenderer;      //Used to load in the sprite

    List<string> touchingObjects = new List<string>();

    private float wait = 3f;

    //When awake, get the sprite renderer of this object
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }



    //This is to check if the object is on any objects, if not, delete self
    private void Update()
    {
        if(wait >= 0)
            wait -= Time.deltaTime;

        if(touchingObjects.Count == 0 && wait < 0)
        {
            Destroy(gameObject);
        }
    }


    //This is to take note of any objects I am touching
    private void OnCollisionEnter2D(Collision2D collision)
    {
        touchingObjects.Add(collision.gameObject.name);
    }


    //This is to take note of any objects I am no longer touching
    private void OnCollisionExit2D(Collision2D collision)
    {
        touchingObjects.Remove(collision.gameObject.name);
    }


    //This is used to setup the splat
    public void Setup()
    {
        SetupSize();
        SetupSprite();
        SetupRotation();

        spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        spriteRenderer.sortingOrder = 3;
    }


    //Randomly set the sprite used
    private void SetupSprite()
    {
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
    }


    //Randomly set the size used
    private void SetupSize()
    {
        transform.localScale *= Random.Range(minSize, maxSize);
    }


    //Randomdly set the rotation
    private void SetupRotation()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-360, 360));
    }
}
