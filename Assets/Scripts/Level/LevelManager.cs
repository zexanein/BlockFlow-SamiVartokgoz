using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : ManagerLocatable
{
    private BoardManager _boardManager;
    
    protected override void OnInitialized()
    {
        _boardManager = Locator.GetLocatable<BoardManager>();
    }

    private void Start()
    {
        _boardManager.CreateBoard(GetTestLevelData());
    }

    private LevelData GetTestLevelData()
    {
        var testBlocks = new List<BlockData>
        {
            new(
                blockID: 0,
                blockColor: "red",
                shapeType: ShapeType.T,
                position: Vector2Int.one,
                constraint: BlockData.AxisConstraint.None,
                rotationSteps: 2
            ),
            
            new(
                blockID: 1,
                blockColor: "green",
                shapeType: ShapeType.T,
                position: Vector2Int.one * 3,
                constraint: BlockData.AxisConstraint.None,
                rotationSteps: 0
            ),
        };
        
        var levelData = new LevelData(10, 10, testBlocks);
        return levelData;
    }
}
