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
        
        var levelData = new LevelData(10, 10, GetTestBlocks(), GetTestGrinders());
        return levelData;
    }

    private List<BlockData> GetTestBlocks()
    {
        return new List<BlockData>
        {
            new(
                blockID: 0,
                blockColor: "red",
                shapeType: ShapeType.T,
                position: Vector2Int.one,
                constraint: AxisConstraint.None,
                direction: Direction.Down
            ),
            
            new(
                blockID: 2,
                blockColor: "red",
                shapeType: ShapeType.T,
                position: new Vector2Int(1, 5),
                constraint: AxisConstraint.None,
                direction: Direction.Left
            ),
            
            new(
                blockID: 1,
                blockColor: "green",
                shapeType: ShapeType.T,
                position: Vector2Int.one * 3,
                constraint: AxisConstraint.None,
                direction: Direction.Up
            )
        };
    }

    private List<GrinderData> GetTestGrinders()
    {
        return new List<GrinderData>
        {
            new(
                grinderID: 0,
                position: new Vector2Int(2, -1),
                grinderColor: "green",
                size: GrinderSize.X1
            ),
            
            new(
                grinderID: 1,
                position: new Vector2Int(10, 8),
                grinderColor: "red",
                size: GrinderSize.X3
            )
        };
    }
}
