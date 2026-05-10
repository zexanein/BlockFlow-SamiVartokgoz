using System.Linq;
using UnityEngine;

public static class StaticMethods
{
    private static ColorPalette _colorPalette;
    private static bool _isColorPaletteLoaded;
    
    public static Color GetColorFromName(string colorName)
    {
        if (_isColorPaletteLoaded)
            return _colorPalette.Get(colorName);
        
        _colorPalette = Resources.Load<ColorPalette>("ColorPalette");
            
        if (_colorPalette == null)
        {
            Debug.LogError("No ColorPalette found in Resources!");
            return Color.white;
        }
            
        _isColorPaletteLoaded = true;

        return _colorPalette.Get(colorName);
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

    public static Direction GetGrinderDirection(GrinderData grinderData, int boardWidth, int boardHeight)
    {
        if (grinderData.Position.x < 0)
            return Direction.Right;
        
        if (grinderData.Position.x >= boardWidth)
            return Direction.Left;
        
        if (grinderData.Position.y < 0)
            return Direction.Up;
        
        if (grinderData.Position.y >= boardHeight)
            return Direction.Down;

        Debug.LogError("Grinder is not placed outside the board!");
        return Direction.Up;
    }
}
