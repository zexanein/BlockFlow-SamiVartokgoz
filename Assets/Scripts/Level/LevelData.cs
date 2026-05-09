using System.Collections.Generic;

public class LevelData
{
    public int Width { get; }
    public int Height { get; }
    public List<BlockData> Blocks { get; }
    
    public LevelData(int width, int height, List<BlockData> blocks)
    {
        Width = width;
        Height = height;
        Blocks = blocks;
    }
}