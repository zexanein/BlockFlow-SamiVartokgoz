using UnityEngine;

public static class ExtensionMethods
{
    public static Vector3 WithX(this Vector3 vector, float x)
    {
        return new Vector3(x, vector.y, vector.z);
    }
    
    public static Vector2Int ToVector(this Direction dir) => dir switch
    {
        Direction.Left => Vector2Int.left,
        Direction.Right => Vector2Int.right,
        Direction.Down => Vector2Int.down,
        Direction.Up => Vector2Int.up,
        _ => Vector2Int.zero
    };
    
    public static Vector3 ToVector3(this Direction dir) => dir switch
    {
        Direction.Left => Vector3.left,
        Direction.Right => Vector3.right,
        Direction.Down => Vector3.back,
        Direction.Up => Vector3.forward,
        _ => Vector3.zero
    };
}
