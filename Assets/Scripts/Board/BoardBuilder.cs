using UnityEngine;

public class BoardBuilder : ManagerLocatable
{
    private BoardManager _boardManager;
    
    [Header("Board")]
    [SerializeField]  private Transform boardTransform;
    
    [Header("Prefabs")]
    [SerializeField] private Transform tilePrefab;
    [SerializeField] private Transform wallPrefab;
    [SerializeField] private Transform cornerPrefab;
    
    private Transform _wallParent;
    private Transform _tileParent;

    protected override void OnInitialized()
    {
        _wallParent = new GameObject("Walls").transform;
        _wallParent.SetParent(boardTransform);
        
        _tileParent = new GameObject("Tiles").transform;
        _tileParent.SetParent(boardTransform);
    }

    public void Build(LevelData levelData, BoardManager boardManager)
    {
        _boardManager = boardManager;
        SpawnTiles(levelData.Width, levelData.Height);
        SpawnWalls(levelData.Width, levelData.Height);
    }

    private void SpawnTiles(int width, int height)
    {
        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
        {
            var tile = Instantiate(tilePrefab, _tileParent);
            tile.name = $"({x},{y})";
            tile.transform.localPosition = _boardManager.GridToWorld(new Vector2Int(x, y));
        }
    }

    private void SpawnWalls(int width, int height)
    {
        for (var x = 0; x < width; x++)
        {
            SpawnWall(new Vector2Int(x, -1), wallPrefab, 180f); // Bottom
            SpawnWall(new Vector2Int(x, height), wallPrefab, 0f); // Top
        }
    
        for (var y = 0; y < height; y++)
        {
            SpawnWall(new Vector2Int(-1, y), wallPrefab, 270f); // Left
            SpawnWall(new Vector2Int(width, y), wallPrefab, 90f); // Right
        }
        
        SpawnWall(new Vector2Int(width, height), cornerPrefab, 0f); // Top Right
        SpawnWall(new Vector2Int(width, -1), cornerPrefab, 90); // Bottom Right
        SpawnWall(new Vector2Int(-1, -1), cornerPrefab, 180f); // Bottom Left
        SpawnWall(new Vector2Int(-1, height), cornerPrefab, 270f); // Top Left
    }
    
    private void SpawnWall(Vector2Int cellPosition, Transform prefab, float rotationY)
    {
        var wall = Instantiate(prefab, _wallParent);
        wall.transform.position = _boardManager.GridToWorld(cellPosition);
        wall.transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }
}
