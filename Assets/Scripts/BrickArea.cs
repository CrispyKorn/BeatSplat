using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BrickArea : MonoBehaviour {

    public GameObject brick;
    public Vector2 brickSize = new Vector2(0.8f, 0.5f);
    public float gap = 0.3f;
    public float stagger = 0.0f;

    private Dictionary<Vector2Int, Brick> grid;

    int getXCount(){
        return (int) Mathf.Floor(transform.localScale.x / (brickSize.x + gap));
    }

    int getYCount(){
        return (int) Mathf.Floor(transform.localScale.y / (brickSize.y + gap));
    }

    // Generate a grid of brick locations using the parameters, location, and size provided
    // This method takes a Func<Vector3>, which will recieve the actual brick positions
    // Think of it kind of like passing some code to this method as a parameter,
    // this method can give the brick positions to that other code without needing to
    // allocate, return, and then iterate over a list!
    Dictionary<Vector2Int, T> GenerateBricks<T>(Func<Vector3, T> handler) {
        Dictionary<Vector2Int, T> grid = new Dictionary<Vector2Int, T>();
        int xCount = getXCount();
        int yCount = getYCount(); ;
        float totalWidth = brickSize.x * xCount + gap * (xCount - 1);
        float totalHeight = brickSize.y * yCount + gap * (yCount - 1);
        for (int x = 0; x < xCount; x++){
            for (int y = 0; y < yCount; y++){
                Vector3 pos = transform.position;
                pos.x += y % 2 == 0 ? stagger : -stagger;
                pos.x += brickSize.x / 2 + (brickSize.x + gap) * x - totalWidth / 2;
                pos.y += brickSize.y / 2 + (brickSize.y + gap) * y - totalHeight / 2;
                grid.Add(new Vector2Int(x, y), handler(pos));
            }
        }
        return grid;
    }

    // Uses the location generator to render a wireframe in the editor
    void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        Gizmos.color = Color.cyan;
        GenerateBricks<object>((Vector3 v) => {
            Gizmos.DrawWireCube(v, new Vector3(brickSize.x, brickSize.y, 0));
            return null;
        });
    }


    // Actually spawns the brick objects at the generated locations when the script starts
    void Start() {
        SetupBricks();
    }


    public void ResetBricks()
    {
        DestroyAllBricks();
        SetupBricks();
    }


    private void SetupBricks()
    {
        // Making a container here to contain the generated brick objects
        // This object itself can't contain them because we use scale to determine
        // the sizer of the region and scale makes child objects wonky
        var container = new GameObject("Brick container");

        //var grid = GenerateBricks((Vector3 v) => {
        grid = GenerateBricks((Vector3 v) => {

            // Spawn brick in container at generated location
            var brick = Instantiate(this.brick, v, Quaternion.identity, container.transform);

            // Make the brick adopt the size defined in this script's parameters
            brick.transform.localScale = new Vector3(brickSize.x, brickSize.y, 0);

            return brick.GetComponent<Brick>();
        });

        foreach (var key in grid.Keys)
        {
            var brick = grid[key];
            if (grid.ContainsKey(key + Vector2Int.left)) brick.addNeighbour(grid[key + Vector2Int.left]);
            if (grid.ContainsKey(key + Vector2Int.right)) brick.addNeighbour(grid[key + Vector2Int.right]);
            if (grid.ContainsKey(key + Vector2Int.up)) brick.addNeighbour(grid[key + Vector2Int.up]);
            if (grid.ContainsKey(key + Vector2Int.down)) brick.addNeighbour(grid[key + Vector2Int.down]);

        }
    }



    private void DestroyAllBricks()
    {

        foreach (var key in grid.Keys)
        {
            var brick = grid[key];
            if(brick != null)
                Destroy(brick.gameObject);
        }

        //var list = grid.Values.ToList();
        //for(int i = list.Count - 1; i >= 0; i--)
        //{
        //    Destroy(list[i].gameObject);
        //}


        grid.Clear();
    }
}
