using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Clean up all the GetComponent maybe it doesn't happen every frame but still it looks kind of gross

public class Ball : MonoBehaviour {

    public ParticleSystem splatParticalSystemPrefab;
    private static ParticleSystem splatParticalSystem = null;

    public float speed = 3f;

    private Collider2D ballCollider;
    private Rigidbody2D ballBody;
    private SpriteRenderer ballRenderer;

    public bool served = false;

    private bool multiColourMode = false;

    // Current colour ID in theme's brick colour list
    int color = 0;

    Controller controller;

    private void Awake()
    {
        ballCollider = GetComponent<Collider2D>();
        ballBody = GetComponent<Rigidbody2D>();
        ballRenderer = GetComponent<SpriteRenderer>();
    }

    void Start() {

        controller = FindObjectOfType<Controller>();
        NewColor();

        if (splatParticalSystemPrefab != null && splatParticalSystem == null)
            splatParticalSystem = Instantiate(splatParticalSystemPrefab, transform.parent);
        else if (splatParticalSystemPrefab == null)
            Debug.LogError("Missing Splatter Partical Ssytem Prefab on ball!");
    }

    public void NewColor() {
        color ++;
        if (color == controller.theme.brickColours.Count) color = 0;
        Color actualColor = controller.theme.brickColours[color];
        ballRenderer.color = actualColor;
    }

    public void Hold() {
        served = false;
        ballCollider.isTrigger = true;
        ballBody.isKinematic = true;
        ballBody.linearVelocity = Vector2.zero;
    }

    public void Serve() {
        served = true;
        ballCollider.isTrigger = false;
        ballBody.isKinematic = false;
        ballBody.linearVelocity = new Vector2(Random.Range(-1f, 1f), 1f).normalized * speed;
    }

    void OnCollisionEnter2D(Collision2D hit) {
        // Try to find brick component of what we hit
        var brick = hit.gameObject.GetComponent<Brick>();

        controller.HandleBounce(transform.position);

        if (hit.gameObject.layer != LayerMask.NameToLayer("Player"))
            foreach (ContactPoint2D contact in hit.contacts) SplatterPaint(contact.point);

        if (brick != null)
            brick.HandleBounce(color);

    }


    private void SplatterPaint(Vector2 hit)
    {
        if (splatParticalSystem != null)
        {
            splatParticalSystem.transform.position = hit;
            splatParticalSystem.Play();
        }
        else
            Debug.LogError("Missing splatter Partical System on ball!");
    }



    //This is for a powerup that I dont think is fun
    public void GravityAdded(float gravity) {
        ballBody.gravityScale += gravity;
    }


    public void MultiColourMode(bool mode) {
        multiColourMode = mode;
    }
}
