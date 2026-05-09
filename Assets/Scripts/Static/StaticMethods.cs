using System.Linq;
using UnityEngine;

public static class StaticMethods
{
    public static Color GetColorFromName(string colorName)
    {
        return colorName.ToLower() switch
        {
            "red" => Color.red,
            "green" => Color.green,
            "blue" => Color.blue,
            _ => Color.white
        };
    }

    /// <summary>
    /// Rotates the shape by clockwise 90 degree increments
    /// </summary>
    /// <param name="shape"></param>
    /// <param name="steps">Number of 90 degree rotations</param>
    public static Vector2Int[] RotateShape(Vector2Int[] shape, int steps)
    {
        var result = new Vector2Int[shape.Length];

        for (var i = 0; i < shape.Length; i++)
            result[i] = shape[i];

        for (var s = 0; s < steps; s++)
        for (var i = 0; i < result.Length; i++)
            result[i] = new Vector2Int(result[i].y, -result[i].x);

        return result;
    }
}
