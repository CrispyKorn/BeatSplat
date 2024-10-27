using System.Collections.Generic;
using UnityEngine;

public class BrickArea : MonoBehaviour 
{
    [Tooltip("The size of the spawn area.")]
    [SerializeField] private Vector2 _spawnAreaSize = new Vector2(10f, 5f);
    [Tooltip("The brick prefab to spawn.")]
    [SerializeField] private GameObject _brick;
    [Tooltip("The size of each brick in units.")]
    [SerializeField] private Vector2 _brickSize = new Vector2(0.8f, 0.5f);
    [Tooltip("The gap between bricks in units.")]
    [SerializeField] private float _gap = 0.3f;
    [Tooltip("The amount to stagger bricks in units.")]
    [SerializeField] private float _stagger = 0.0f;

    private Dictionary<Vector2Int, Brick> _grid;

    private void Awake()
    {
        Locator.Instance.RegisterInstance(this);
    }

    private void Start()
    {
        SetupBricks();
    }
    
    private void OnDrawGizmos()
    {
        // Uses the location generator to render a wireframe in the editor

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, _spawnAreaSize);

        Gizmos.color = Color.cyan;
        foreach (var brickPos in GenerateBrickPositions().Values)
        {
            Gizmos.DrawWireCube(brickPos, new Vector3(_brickSize.x, _brickSize.y, 0));
        }
    }

    private int GetXCount()
    {
        return Mathf.FloorToInt(_spawnAreaSize.x / (_brickSize.x + _gap));
    }

    private int GetYCount()
    {
        return Mathf.FloorToInt(_spawnAreaSize.y / (_brickSize.y + _gap));
    }

    private Dictionary<Vector2Int, Vector3> GenerateBrickPositions() 
    {
        Dictionary<Vector2Int, Vector3> grid = new();

        int horizontalBrickCount = GetXCount();
        int verticalBrickCount = GetYCount();

        float totalBrickWidth = _brickSize.x * horizontalBrickCount;
        float totalGapWidth = _gap * (horizontalBrickCount - 1);
        float totalWidth = totalBrickWidth + totalGapWidth;

        float totalBrickHeight = _brickSize.y * verticalBrickCount;
        float totalGapHeight = _gap * (verticalBrickCount - 1);
        float totalHeight = totalBrickHeight + totalGapHeight;

        for (var x = 0; x < horizontalBrickCount; x++)
        {
            for (var y = 0; y < verticalBrickCount; y++)
            {
                float stagger = (y % 2 == 0) ? _stagger : -_stagger;
                float halfBrickWidth = _brickSize.x / 2f;
                float singleBrickTotalWidth = _brickSize.x + _gap;
                float halfTotalWidth = totalWidth / 2f;
                float brickPositionOffsetX = halfBrickWidth + (singleBrickTotalWidth * x) - halfTotalWidth;

                float halfBrickHeight = _brickSize.y / 2f;
                float singleBrickTotalHeight = _brickSize.y + _gap;
                float halfTotalHeight = totalHeight / 2f;
                float brickPositionOffsetY = halfBrickHeight + (singleBrickTotalHeight * y) - halfTotalHeight;

                Vector3 pos = transform.position;
                pos.x += brickPositionOffsetX + stagger;
                pos.y += brickPositionOffsetY;

                grid.Add(new Vector2Int(x, y), pos);
            }
        }

        return grid;
    }

    private Dictionary<Vector2Int, Brick> GenerateBricks(Dictionary<Vector2Int, Vector3> brickPositions, Transform container)
    {
        Dictionary<Vector2Int, Brick> bricks = new();

        foreach (var brickPos in brickPositions.Keys)
        {
            var brick = Instantiate(_brick, brickPositions[brickPos], Quaternion.identity, container); // Spawn brick in container at generated location
            brick.transform.localScale = new Vector3(_brickSize.x, _brickSize.y, 0); // Make the brick adopt the size defined in this script's parameters

            bricks.Add(brickPos, brick.GetComponent<Brick>());
        }

        return bricks;
    }

    private void SetupBricks()
    {
        // Making a container here to contain the generated brick objects
        // This object itself can't contain them because we use scale to determine
        // the sizer of the region and scale makes child objects wonky
        var container = new GameObject("Brick container");

        _grid = GenerateBricks(GenerateBrickPositions(), container.transform);

        // Assign brick neighbours
        foreach (var key in _grid.Keys)
        {
            var brick = _grid[key];

            if (_grid.ContainsKey(key + Vector2Int.left)) brick.AddNeighbour(_grid[key + Vector2Int.left]);
            if (_grid.ContainsKey(key + Vector2Int.right)) brick.AddNeighbour(_grid[key + Vector2Int.right]);
            if (_grid.ContainsKey(key + Vector2Int.up)) brick.AddNeighbour(_grid[key + Vector2Int.up]);
            if (_grid.ContainsKey(key + Vector2Int.down)) brick.AddNeighbour(_grid[key + Vector2Int.down]);

        }
    }

    private void DestroyAllBricks()
    {
        foreach (var key in _grid.Keys)
        {
            var brick = _grid[key];
            if (brick != null) Destroy(brick.gameObject);
        }

        _grid.Clear();
    }

    public void ResetBricks()
    {
        DestroyAllBricks();
        SetupBricks();
    }
}
