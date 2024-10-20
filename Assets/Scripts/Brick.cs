using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Brick : MonoBehaviour {

    // Colour ID in theme's brick colour list
    public int colorId;
    Color color;

    readonly float flashDuration = 0.1f;
    float lastFlashTime = -1000f;
    List<Brick> neighbours = new List<Brick>();

    SpriteRenderer sprite;
    Controller controller;

    void Start() {
        controller = FindObjectOfType<Controller>();
        sprite = GetComponent<SpriteRenderer>();
        colorId = controller.theme.RandomColourID;
 
    }

    private void Update(){
        float sinceFlash = Mathf.Min(Time.time - lastFlashTime, flashDuration);
        var mainColor = controller.theme.brickColours[colorId];
        sprite.color = Color.Lerp(controller.theme.foreground, mainColor, sinceFlash / flashDuration);
    }

    public void addNeighbour(Brick brick){
        if (brick != null) neighbours.Add(brick);
    }

    public void HandleBounce(int ballColor) { 
        if (colorId == ballColor) {
            controller.HandleBrickBreak(transform.position);
            foreach (var brick in neighbours) {
                brick.neighbours.Remove(this);
                brick.StartFlash();
            }
            Destroy(gameObject);
        }
        else StartFlash();
    }


    public void StartFlash(){
        HandleFlashSpread(new List<Brick>());
    }

    void HandleFlashSpread(List<Brick> seen){
        lastFlashTime = Time.time;
        seen.Add(this);        
        StartCoroutine(DelayFlashSpread(seen));
    }

    IEnumerator DelayFlashSpread(List<Brick> seen){
        yield return new WaitForSeconds(0.01f);
        foreach (var brick in neighbours.FindAll(b => !seen.Contains(b)))
            brick.HandleFlashSpread(seen);
    }


}