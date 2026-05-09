using UnityEngine;

public class BoardManager : ManagerLocatable
{
    [SerializeField] private Transform boardTransform;
    
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform wallPrefab;
    [SerializeField] private Transform cornerPrefab;
    
    [SerializeField] private float cellSize = 1f;
    
    private Transform _wallParent;
    private Transform _tileParent;
    
    private int _width, _height;
    private int[,] _occupancyMap;
    
    private BlockManager _blockManager;

    protected override void OnInitialized()
    {
        _blockManager = Locator.GetLocatable<BlockManager>();
        
        _wallParent = new GameObject("Walls").transform;
        _wallParent.SetParent(boardTransform);
        
        _tileParent = new GameObject("Tiles").transform;
        _tileParent.SetParent(boardTransform);
    }

    public void CreateBoard(LevelData levelData)
    {
        _width = levelData.Width;
        _height = levelData.Height;
        SpawnTiles();
        SpawnWalls();
        
        _occupancyMap = new int[_width, _height];
        ClearOccupancyMap();
        foreach (var blockData in levelData.Blocks)
        {
            RegisterBlock(blockData);
        }
        
        
        Locator.GetLocatable<BlockManager>().SpawnBlocks(levelData.Blocks);
    }

    private void SpawnTiles()
    {
        for (var x = 0; x < _width; x++)
        for (var y = 0; y < _height; y++)
        {
            var tile = Instantiate(tilePrefab, _tileParent);
            tile.name = $"({x},{y})";
            tile.transform.localPosition = GridToWorld(new Vector2Int(x, y));
        }
    }

    private void SpawnWalls()
    {
        for (var x = 0; x < _width; x++)
        {
            SpawnWall(new Vector2Int(x, -1), wallPrefab, 180f); // Bottom
            SpawnWall(new Vector2Int(x, _height), wallPrefab, 0f); // Top
        }
    
        for (var y = 0; y < _height; y++)
        {
            SpawnWall(new Vector2Int(-1, y), wallPrefab, 270f); // Left
            SpawnWall(new Vector2Int(_width, y), wallPrefab, 90f); // Right
        }
        
        SpawnWall(new Vector2Int(_width, _height), cornerPrefab, 0f); // Top Right
        SpawnWall(new Vector2Int(_width, -1), cornerPrefab, 90); // Bottom Right
        SpawnWall(new Vector2Int(-1, -1), cornerPrefab, 180f); // Bottom Left
        SpawnWall(new Vector2Int(-1, _height), cornerPrefab, 270f); // Top Left
    }
    
    private void SpawnWall(Vector2Int cellPosition, Transform prefab, float rotationY)
    {
        var wall = Instantiate(prefab, _wallParent);
        wall.transform.position = GridToWorld(cellPosition);
        wall.transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
    }

    private void ClearOccupancyMap()
    {
        for (var x = 0; x < _width; x++)
        for (var y = 0; y < _height; y++)
            _occupancyMap[x, y] = -1;
    }

    private void RegisterBlock(BlockData blockData)
    {
        foreach (var blockCellPosition in _blockManager.GetBlockCellPositions(blockData))
        {
            _occupancyMap[blockCellPosition.x, blockCellPosition.y] = blockData.BlockID;
        }
    }
    
    public Vector3 GridToWorld(Vector2Int cell)
    {
        var offsetX = (_width - 1) * cellSize * 0.5f;
        var offsetZ = (_height - 1) * cellSize * 0.5f;
        return new Vector3(cell.x * cellSize - offsetX, 0f, cell.y * cellSize - offsetZ);
    }
}