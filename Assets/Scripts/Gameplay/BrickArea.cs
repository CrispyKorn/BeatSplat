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

    private static Transform s_brickHolder;

    private Dictionary<Vector2Int, Brick> _grid;
    private int _destroyedBricksCount = 0;
    private Vector2 _initialBrickSize;

    private void Awake()
    {
        Locator.Instance.RegisterInstance(this);
        _initialBrickSize = _brickSize;
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

        var horizontalBrickCount = GetXCount();
        var verticalBrickCount = GetYCount();

        var totalBrickWidth = _brickSize.x * horizontalBrickCount;
        var totalGapWidth = _gap * (horizontalBrickCount - 1);
        var totalWidth = totalBrickWidth + totalGapWidth;

        var totalBrickHeight = _brickSize.y * verticalBrickCount;
        var totalGapHeight = _gap * (verticalBrickCount - 1);
        var totalHeight = totalBrickHeight + totalGapHeight;

        for (var x = 0; x < horizontalBrickCount; x++)
        {
            for (var y = 0; y < verticalBrickCount; y++)
            {
                var stagger = (y % 2 == 0) ? _stagger : -_stagger;
                var halfBrickWidth = _brickSize.x / 2f;
                var singleBrickTotalWidth = _brickSize.x + _gap;
                var halfTotalWidth = totalWidth / 2f;
                var brickPositionOffsetX = halfBrickWidth + (singleBrickTotalWidth * x) - halfTotalWidth;

                var halfBrickHeight = _brickSize.y / 2f;
                var singleBrickTotalHeight = _brickSize.y + _gap;
                var halfTotalHeight = totalHeight / 2f;
                var brickPositionOffsetY = halfBrickHeight + (singleBrickTotalHeight * y) - halfTotalHeight;

                var pos = transform.position;
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
            var brickObj = Instantiate(_brick, brickPositions[brickPos], Quaternion.identity, container); // Spawn brick in container at generated location
            brickObj.transform.localScale = new Vector3(_brickSize.x, _brickSize.y, 0); // Make the brick adopt the size defined in this script's parameters
            var brick = brickObj.GetComponent<Brick>();
            brick.OnDestroyed += OnBrickDestroyed;

            bricks.Add(brickPos, brick);
        }

        return bricks;
    }

    private void SetupBricks()
    {
        // Making a container here to contain the generated brick objects
        if (s_brickHolder == null) s_brickHolder = new GameObject("Brick container").transform;

        _grid = GenerateBricks(GenerateBrickPositions(), s_brickHolder);

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

    private void OnBrickDestroyed()
    {
        _destroyedBricksCount++;

        if (_destroyedBricksCount == _grid.Count)
        {
            _grid.Clear();
            Locator.Instance.GameManager.EndGame(true);
        }
    }

    private void DestroyAllBricks()
    {
        foreach (var key in _grid.Keys)
        {
            var brick = _grid[key];
            if (brick != null)
            {
                brick.OnDestroyed -= OnBrickDestroyed;
                Destroy(brick.gameObject);
            }
        }

        _destroyedBricksCount = 0;
        _grid.Clear();
    }

    public void ResetBricks()
    {
        DestroyAllBricks();
        SetupBricks();
    }

    public void IncreaseBrickDensity()
    {
        _brickSize *= 0.75f;
    }

    public void ResetBrickDensity()
    {
        _brickSize = _initialBrickSize;
    }
}
