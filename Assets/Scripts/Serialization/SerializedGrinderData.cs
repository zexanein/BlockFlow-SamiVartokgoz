using UnityEngine;

[System.Serializable]
public class SerializedGrinderData
{
    public int id;
    public Vector2Int position;
    public int color;
    public int size;

    public GrinderData ToGrinderData()
    {
        return new GrinderData(
            grinderID: id,
            position: position,
            size: (GrinderSize)size,
            grinderColor: color
        );
    }
}