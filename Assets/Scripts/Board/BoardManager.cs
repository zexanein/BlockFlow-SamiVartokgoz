using System;
using UnityEngine;

public class BoardManager : ManagerLocatable
{
    [SerializeField] private float cellSize = 1f;
    
    
    private int _width, _height;
    private int[,] _occupancyMap;
    
    private BlockManager _blockManager;
    private GrinderManager _grinderManager;
    private BoardBuilder _boardBuilder;
    
    public float CellSize => cellSize;
    public int Width => _width;
    public int Height => _height;

    protected override void OnInitialized()
    {
        _blockManager = Locator.GetLocatable<BlockManager>();
        _grinderManager = Locator.GetLocatable<GrinderManager>();
        _boardBuilder = Locator.GetLocatable<BoardBuilder>();
    }

    public void CreateBoard(LevelData levelData)
    {
        _width = levelData.Width;
        _height = levelData.Height;
        
        _occupancyMap = new int[_width, _height];
        ClearOccupancyMap();
        foreach (var blockData in levelData.Blocks)
        {
            RegisterBlock(blockData);
        }
        
        _blockManager.SpawnBlocks(levelData.Blocks);
        _grinderManager.SpawnGrinders(levelData.Grinders);
        _boardBuilder.Build(levelData, this);
    }

    private void ClearOccupancyMap()
    {
        for (var x = 0; x < _width; x++)
        for (var y = 0; y < _height; y++)
            _occupancyMap[x, y] = -1;
    }

    public void RegisterBlock(BlockData blockData)
    {
        foreach (var blockCellPosition in _blockManager.GetBlockCellGridPositions(blockData))
        {
            _occupancyMap[blockCellPosition.x, blockCellPosition.y] = blockData.BlockID;
        }
    }

    public void UnregisterBlock(BlockData block)
    {
        foreach (var blockCellPosition in _blockManager.GetBlockCellGridPositions(block))
        {
            _occupancyMap[blockCellPosition.x, blockCellPosition.y] = -1;
        }
    }
    
    public Vector3 GridToWorld(Vector2Int cell)
    {
        return GridToWorld(cell.x, cell.y);
    }
    
    public Vector3 GridToWorld(float x, float y)
    {
        var offsetX = (_width - 1) * cellSize * 0.5f;
        var offsetZ = (_height - 1) * cellSize * 0.5f;
        return new Vector3(x * cellSize - offsetX, 0f, y * cellSize - offsetZ);
    }
    
    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        var offsetX = (_width - 1) * cellSize * 0.5f;
        var offsetZ = (_height - 1) * cellSize * 0.5f;
        var x = Mathf.RoundToInt((worldPosition.x + offsetX) / cellSize);
        var y = Mathf.RoundToInt((worldPosition.z + offsetZ) / cellSize);
        return new Vector2Int(x, y);
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < _width && y >= 0 && y < _height;
    }

    public bool IsCellOccupied(int x, int y)
    {
        return _occupancyMap[x, y] != -1;
    }
}