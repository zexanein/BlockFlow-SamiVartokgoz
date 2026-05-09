using System.Collections.Generic;
using UnityEngine;

public class BlockManager : ManagerLocatable
{
    [SerializeField] private ShapeRegistry shapeRegistry;
    [SerializeField] private BlockActor blockActorPrefab;
    
    private BoardManager _boardManager;

    protected override void OnInitialized()
    {
        _boardManager = Locator.GetLocatable<BoardManager>();
    }

    public void SpawnBlocks(List<BlockData> blocks)
    {
        foreach (var blockData in blocks)
        {
            var actor = Instantiate(blockActorPrefab, transform);
            actor.Initialize(blockData, _boardManager, shapeRegistry);
        }
    }
    
    public Vector2Int[] GetBlockCellPositions(BlockData blockData)
    {
        var baseShapeCells = blockData.GetShape(shapeRegistry);
        var returnValue = new Vector2Int[baseShapeCells.Length];

        for (var i = 0; i < baseShapeCells.Length; i++)
        {
            returnValue[i] = blockData.Position + baseShapeCells[i];
        }
        
        return returnValue;
    }
}