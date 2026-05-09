using UnityEngine;

public class BoardManager : ManagerLocatable
{
    [SerializeField] private Transform boardTransform;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private float cellSize = 1f;
    
    private int _width, _height;
    private int[,] _occupancyMap;
    
    private BlockManager _blockManager;

    protected override void OnInitialized()
    {
        _blockManager = Locator.GetLocatable<BlockManager>();
    }

    public void CreateBoard(LevelData levelData)
    {
        _width = levelData.Width;
        _height = levelData.Height;
        SpawnTiles();
        
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
            var tile = Instantiate(tilePrefab, boardTransform);
            tile.name = $"({x},{y})";
            tile.transform.localPosition = GridToWorld(new Vector2Int(x, y));
        }
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