using System;

public class LevelManager : ManagerLocatable
{
    private BoardManager _boardManager;
    private SerializationManager _serializationManager;
    private int _currentLevelIndex;
    
    public event Action<int> OnLevelLoaded;

    protected override void OnInitialized()
    {
        _boardManager = Locator.GetLocatable<BoardManager>();
        _serializationManager = Locator.GetLocatable<SerializationManager>();
    }

    public void LoadLevel(int index)
    {
        _currentLevelIndex = index;
        var levelData = _serializationManager.GetLevel(index);
        if (levelData == null) return;
        _boardManager.CreateBoard(levelData);
        OnLevelLoaded?.Invoke(index);
    }

    public void LoadNextLevel()
    {
        LoadLevel(_currentLevelIndex + 1);
    }

    public void RestartLevel()
    {
        LoadLevel(_currentLevelIndex);
    }
    
    public float GetLevelTimeLimit(int levelIndex)
    {
        var levelData = _serializationManager.GetLevel(_currentLevelIndex);
        return levelData?.TimeLimit ?? 0f;
    }
}