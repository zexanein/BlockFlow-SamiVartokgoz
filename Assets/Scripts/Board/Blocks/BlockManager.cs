using System.Collections.Generic;
using UnityEngine;

public class BlockManager : ManagerLocatable
{
    [SerializeField] private BlockActor blockActorPrefab;
    [SerializeField] private BlockShapeRegistry blockShapeRegistry;
    
    private BoardManager _boardManager;

    protected override void OnInitialized()
    {
        _boardManager = Locator.GetLocatable<BoardManager>();
    }

    public void SpawnBlocks(List<BlockData> blocks)
    {
        foreach (var blockData in blocks)
        {
            var blockActor = Instantiate(blockActorPrefab, transform);
            blockActor.Initialize(blockData, _boardManager, blockShapeRegistry);
            GenerateBoxCollidersForBlock(blockActor, blockData, _boardManager.CellSize);
        }
    }

    private void GenerateBoxCollidersForBlock(BlockActor actor, BlockData blockData, float cellSize)
    {
        var shape = blockData.GetBaseShape(blockShapeRegistry);
        var visited = new HashSet<int>();

        for (var i = 0; i < shape.Length; i++)
        {
            if (visited.Contains(i)) continue;

            var start = shape[i];
            var count = 1;
            visited.Add(i);

            while (true)
            {
                var next = new Vector2Int(start.x + count, start.y);
                var found = false;

                for (var j = 0; j < shape.Length; j++)
                {
                    if (visited.Contains(j) || shape[j] != next) continue;
                    
                    visited.Add(j);
                    count++;
                    found = true;
                    break;
                }

                if (!found) break;
            }

            var box = actor.gameObject.AddComponent<BoxCollider>();
            box.center = new Vector3(
                (start.x + (count - 1) * 0.5f) * cellSize,
                0.5f,
                start.y * cellSize
            );
            
            box.size = new Vector3(
                count * cellSize,
                cellSize,
                cellSize
            );
        }
    }
    
    public Vector2Int[] GetBlockRotatedShape(BlockData blockData)
    {
        return blockData.GetRotatedShape(blockShapeRegistry);
    }
    
    public Vector2Int[] GetBlockCellGridPositions(BlockData blockData)
    {
        var baseShapeCells = blockData.GetRotatedShape(blockShapeRegistry);
        var returnValue = new Vector2Int[baseShapeCells.Length];

        for (var i = 0; i < baseShapeCells.Length; i++)
        {
            returnValue[i] = blockData.Position + baseShapeCells[i];
        }
        
        return returnValue;
    }
}