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
        }
    }
    
    public Vector2Int[] GetBlockCellPositions(BlockData blockData)
    {
        var baseShapeCells = blockData.GetShape(blockShapeRegistry);
        var returnValue = new Vector2Int[baseShapeCells.Length];

        for (var i = 0; i < baseShapeCells.Length; i++)
        {
            returnValue[i] = blockData.Position + baseShapeCells[i];
        }
        
        return returnValue;
    }
}