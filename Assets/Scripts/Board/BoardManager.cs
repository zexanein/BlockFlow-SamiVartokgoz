using UnityEngine;

public class BoardManager : ManagerLocatable
{
    [SerializeField] private Transform boardTransform;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private float cellSize = 1f;
    
    private int _width, _height;

    public void CreateBoard(LevelData levelData)
    {
        _width = levelData.Width;
        _height = levelData.Height;
        SpawnTiles();
    }

    private void SpawnTiles()
    {
        for (var x = 0; x < _width; x++)
        for (var y = 0; y < _height; y++)
        {
            var tile = Instantiate(tilePrefab, boardTransform);
            tile.name = $"({x},{y})";
            tile.transform.localPosition = new Vector3(x * cellSize, 0, y * cellSize);
        }
    }
}