using System.Collections.Generic;
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

    private float OffsetX => (_width - 1) * cellSize * 0.5f;
    private float OffsetZ => (_height - 1) * cellSize * 0.5f;
    
    private HashSet<Vector2Int> _inactiveCells = new();

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
        
        _inactiveCells.Clear();
        if (levelData.InactiveCells != null)
            foreach (var cell in levelData.InactiveCells)
                _inactiveCells.Add(cell);
        
        ClearOccupancyMap();

        _blockManager.SpawnBlocks(levelData.Blocks);
        foreach (var blockData in levelData.Blocks)
            RegisterBlock(blockData);

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
        foreach (var pos in _blockManager.GetBlockCellGridPositions(blockData))
            _occupancyMap[pos.x, pos.y] = blockData.BlockID;
    }

    public void UnregisterBlock(BlockData block)
    {
        foreach (var pos in _blockManager.GetBlockCellGridPositions(block))
            _occupancyMap[pos.x, pos.y] = -1;
    }

    public Vector3 GridToWorld(Vector2Int cell) => GridToWorld(cell.x, cell.y);

    public Vector3 GridToWorld(float x, float y)
    {
        return new Vector3(x * cellSize - OffsetX, 0f, y * cellSize - OffsetZ);
    }

    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        var pos = WorldToGridFloat(worldPosition);
        return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }

    public Vector2 WorldToGridFloat(Vector3 worldPosition)
    {
        return new Vector2(
            (worldPosition.x + OffsetX) / cellSize,
            (worldPosition.z + OffsetZ) / cellSize
        );
    }

    public bool IsInBounds(int x, int y) => x >= 0 && x < _width && y >= 0 && y < _height;

    public bool IsCellOccupied(int x, int y) => !IsCellActive(new Vector2Int(x, y)) || _occupancyMap[x, y] != -1;
    
    public bool IsCellActive(Vector2Int cell) => IsInBounds(cell.x, cell.y) && !_inactiveCells.Contains(cell);

    public bool IsShapeAtEdge(Vector2Int[] shape, Vector2Int pos)
    {
        foreach (var cell in shape)
        {
            var gridPos = pos + cell;
            if (gridPos.x == 0 || gridPos.x == _width - 1 ||
                gridPos.y == 0 || gridPos.y == _height - 1)
                return true;
        }
        return false;
    }
}