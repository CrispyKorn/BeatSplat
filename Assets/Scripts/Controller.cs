using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Controller : MonoBehaviour {

    public Theme theme;

    public float moveSpeed = 8f;
    public GameObject ballPrefab;

    public GameObject changeIndicator;

    new public Camera camera;
    public AudioSource preLaunchLoop;
    public AudioSource postLaunchLoop;
    public AudioSource sfxSource;
    public AudioSource melodySource;

    int countdown = -1;
    public AudioClip countdownSound;
    public AudioClip arpUpSound;
    public AudioClip arpDownSound;
    public List<AudioClip> bounceSounds;

    List<AudioClip> playNextTick = new List<AudioClip>();

    public bool enablePlay = true;


    [Header("Percentage of dropping power-ups")]
    [Tooltip("1 is 100%, 0 is 0% chance of dropping power ups when a brick is destroyed")]
    [Range(0f, 1f)]
    public float powerUpPercentage = 0.5f;

    private PowerUpManager powerUpManager = null;

    Rigidbody2D phys;

    readonly List<Ball> activeBalls = new List<Ball>();
  
    Ball held;

    float lastTickMod;

    int beats = 0; // Metronome pulses
    float lastBeatMod;
     
    void Start() {
        phys = GetComponent<Rigidbody2D>();

        // Set camera background to theme background
        camera.backgroundColor = theme.background;

        // Set sprite color to theme forground for all objects with the "Foreground" tag
        foreach(var obj in GameObject.FindGameObjectsWithTag("Foreground")){
            var sprite = obj.GetComponent<SpriteRenderer>();
            if (sprite != null) sprite.color = theme.foreground; 
        }

        NewBall();

        powerUpManager = FindObjectOfType<PowerUpManager>();
    }

    void Update() {

        // Music sync

        var tickMod = preLaunchLoop.time % (0.5f / 4f);
        if(tickMod < lastTickMod){
            foreach (var clip in playNextTick) melodySource.PlayOneShot(clip);
            playNextTick.Clear();
        }
        lastTickMod = tickMod;

        var beatMod = preLaunchLoop.time % 0.5f;
        if(beatMod < lastBeatMod){
            beats++;
            countdown--;

            if (countdown == 4) sfxSource.PlayOneShot(countdownSound);

            if(countdown == 0) {
                held.Serve();
                held = null;
            }
                
            if(beats % 4 == 0)
            foreach(var ball in activeBalls){
                ball.NewColor();
                Menu.gameOverlay.ChangeBallColours();
            }
        }
        lastBeatMod = beatMod;

        changeIndicator.transform.localScale = new Vector3(1, (preLaunchLoop.time % 2f) / 2f * 10f, 1);

        // Pulse camera in time with music
        camera.orthographicSize = 5f + (preLaunchLoop.time % 2f) / 8f + (preLaunchLoop.time % 0.5f) / 4f;

        // Remove all balls below the game area
        var outOfBounds = activeBalls.Where(b => b.transform.position.y < -6);
        foreach (var ball in outOfBounds) Destroy(ball.gameObject);
        activeBalls.RemoveAll(b => outOfBounds.Contains(b));

        // If there's no active balls, attach a new one to the paddle
        if (activeBalls.Count == 0) {
            sfxSource.PlayOneShot(arpDownSound);
            NewBall();
            powerUpManager.KillPowerUps();
            Menu.gameOverlay.RemoveBall();
        }

        if (activeBalls.FindAll(b => b.served).Count == 0){
            preLaunchLoop.volume = 1;
            postLaunchLoop.volume = 0;
        } else {
            preLaunchLoop.volume = 0;
            postLaunchLoop.volume = 1;
        }

        // If we have a ball stuck to the paddle
        if (held) {

            // Move the ball to where the paddle is
            held.gameObject.transform.position = transform.position + Vector3.up * 0.5f;

            // Serve the ball when fire is pressed, and release the reference to the ball
            if (Input.GetButtonDown("Fire1") && countdown < 0 && enablePlay) {
                countdown = 5;
                sfxSource.PlayOneShot(arpUpSound);
            }
        }

        if (enablePlay)
        {
            // TODO: Pretty sure this input is meant to come from fixedUpdate, but we can fix that later!
            phys.linearVelocity = Vector2.right * moveSpeed * Input.GetAxisRaw("Horizontal");
        }
    }

    public void NewBall(){
        if (held == null) {
            
            held = Instantiate(ballPrefab).GetComponent<Ball>();
            activeBalls.Add(held);
            held.Hold();
        }
    }

    public void HandleBrickBreak(Vector3 location){
        //Debug.Log("Brick broken at: " + location);

        if (powerUpManager == null)
        {
            Debug.LogError("Controller cannot find the powerup manager!");
            return;
        }


        if (UnityEngine.Random.Range(0, 1) <= powerUpPercentage)
            powerUpManager.HitBrick(location);
    }

    public void HandleBounce(Vector3 location) {
        var clip = bounceSounds[UnityEngine.Random.Range(0, bounceSounds.Count)];
        playNextTick.Add(clip);
    }


    //Remove if we remove the gravity power up
    public void AddGravityToBall(float gravity)
    {
        foreach(Ball ball in activeBalls)
        {
            ball.GravityAdded(gravity);
        }
    }

    public void ExtendPaddle(float extend) {
        transform.localScale += new Vector3(extend, 0);
    }

    public void MultiColourMode(bool mode){
        foreach(Ball ball in activeBalls)
        {
            ball.MultiColourMode(mode);
        }
    }
}
