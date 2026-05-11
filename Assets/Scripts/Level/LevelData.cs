using System.Collections.Generic;

public class LevelData
{
    public int Width { get; }
    public int Height { get; }
    public float TimeLimit { get; }
    public List<BlockData> Blocks { get; }
    public List<GrinderData> Grinders { get; }
    
    public LevelData(int width, int height, float timeLimit, List<BlockData> blocks, List<GrinderData> grinders)
    {
        Width = width;
        Height = height;
        TimeLimit = timeLimit;
        Blocks = blocks;
        Grinders = grinders;
        
    }
}