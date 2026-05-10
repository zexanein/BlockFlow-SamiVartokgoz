using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockData
{
    public int BlockID { get; }
    public string BlockColor { get; }
    public Vector2Int Position { get; set; }
    public Direction Direction { get; }
    public ShapeType ShapeType { get; }
    public AxisConstraint Constraint { get; }

    public BlockData(int blockID, string blockColor, Vector2Int position, ShapeType shapeType, AxisConstraint constraint, Direction direction)
    {
        BlockID = blockID;
        BlockColor = blockColor;
        Position = position;
        Direction = direction;
        ShapeType = shapeType;
        Constraint = constraint;
    }
}