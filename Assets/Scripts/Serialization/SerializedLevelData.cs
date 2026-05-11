using System.Collections.Generic;

[System.Serializable]
public class SerializedLevelData
{
    public int gridWidth;
    public int gridHeight;
    public float timeLimit;
    public List<SerializedBlockData> blocks = new();
    public List<SerializedGrinderData> grinders = new();

    public LevelData ToLevelData()
    {
        return new LevelData(
            gridWidth,
            gridHeight,
            timeLimit,
            blocks.ConvertAll(b => b.ToBlockData()),
            grinders.ConvertAll(g => g.ToGrinderData())
        );
    }
}