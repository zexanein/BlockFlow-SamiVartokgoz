using UnityEngine;

public class GrinderData
{
    public int GrinderID { get; }
    public Vector2Int Position { get; }
    public GrinderSize Size { get; }
    public string GrinderColor { get; }

    public GrinderData(int grinderID, Vector2Int position, GrinderSize size, string grinderColor)
    {
        GrinderID = grinderID;
        Position = position;
        Size = size;
        GrinderColor = grinderColor;
    }
}