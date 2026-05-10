using System.Collections.Generic;
using UnityEngine;

public class GrinderManager : ManagerLocatable
{
    [SerializeField] private GrinderActor grinderActorPrefab;
    [SerializeField] private GrinderMeshRegistry grinderMeshRegistry;
    
    private BoardManager _boardManager;
    private BlockManager _blockManager;
    
    private List<GrinderData> _grinders = new();

    protected override void OnInitialized()
    {
        _boardManager = Locator.GetLocatable<BoardManager>();
        _blockManager = Locator.GetLocatable<BlockManager>();
    }

    public void SpawnGrinders(List<GrinderData> grinders)
    {
        foreach (var grinderData in grinders)
        {
            var grinderActor = Instantiate(grinderActorPrefab, transform);
            grinderActor.Initialize(grinderData, _boardManager, grinderMeshRegistry);
            _grinders.Add(grinderData);
        }
    }
    public bool TryGrindBlock(BlockData block)
    {
        var cells = _blockManager.GetBlockCellGridPositions(block);

        foreach (var grinder in _grinders)
        {
            if (grinder.GrinderColor != block.BlockColor) continue;

            var direction = GetBlockExitDirection(grinder);
            var exitCells = GetGrinderCells(grinder);

            if (!BlockContactsGrinder(cells, exitCells, DirectionToVector(direction))) continue;
            if (BlockFitsGrinder(cells, exitCells, direction)) return true;
        }

        return false;
    }

    private bool BlockContactsGrinder(Vector2Int[] blockCells, Vector2Int[] exitCells, Vector2Int dirVec)
    {
        foreach (var cell in blockCells)
            foreach (var exitCell in exitCells)
                if (cell + dirVec == exitCell) return true;
        return false;
    }

    private bool BlockFitsGrinder(Vector2Int[] blockCells, Vector2Int[] exitCells, Direction direction)
    {
        var horizontal = IsHorizontal(direction);
        foreach (var cell in blockCells)
        {
            var fits = false;
            foreach (var exitCell in exitCells)
            {
                if (horizontal ? cell.y != exitCell.y : cell.x != exitCell.x) continue;
                
                fits = true;
                break;
            }
            if (!fits) return false;
        }
        return true;
    }

    private bool IsHorizontal(Direction dir) => dir == Direction.Left || dir == Direction.Right;

    private Vector2Int[] GetGrinderCells(GrinderData grinder)
    {
        var direction = GetBlockExitDirection(grinder);
        var dirVec = DirectionToVector(direction);
        var parallel = new Vector2Int(Mathf.Abs(dirVec.y), Mathf.Abs(dirVec.x));

        var size = (int)grinder.Size;
        var cells = new Vector2Int[size];
        var startOffset = -(size / 2);
        for (var i = 0; i < size; i++)
            cells[i] = grinder.Position + parallel * (startOffset + i);

        return cells;
    }

    private Direction GetBlockExitDirection(GrinderData grinder)
    {
        if (grinder.Position.x < 0) return Direction.Left;
        if (grinder.Position.x >= _boardManager.Width) return Direction.Right;
        if (grinder.Position.y < 0) return Direction.Down;
        return Direction.Up;
    }

    private Vector2Int DirectionToVector(Direction dir) => dir switch
    {
        Direction.Left => Vector2Int.left,
        Direction.Right => Vector2Int.right,
        Direction.Down => Vector2Int.down,
        Direction.Up => Vector2Int.up,
        _ => Vector2Int.zero
    };
}