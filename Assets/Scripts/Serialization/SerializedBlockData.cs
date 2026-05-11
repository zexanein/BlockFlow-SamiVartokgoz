using UnityEngine;

[System.Serializable]
public class SerializedBlockData
{
    public int id;
    public int color;
    public int shapeType;
    public Vector2Int position;
    public int constraint;
    public int direction;
    public int iceEffectDuration;

    public BlockData ToBlockData()
    {
        return new BlockData(
            blockID: id,
            blockColor: color,
            position: position,
            shapeType: (ShapeType)shapeType,
            constraint: (AxisConstraint)constraint,
            direction: (Direction)direction,
            iceEffectDuration: iceEffectDuration
        );
    }
}