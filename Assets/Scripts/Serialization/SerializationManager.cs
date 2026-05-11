using System.Collections.Generic;
using UnityEngine;

public class SerializationManager : ManagerLocatable
{
    [SerializeField] private TextAsset levelsJson;

    private List<LevelData> _levels = new();

    public int LevelCount => _levels.Count;

    protected override void OnInitialized()
    {
        LoadAllLevels();
    }

    private void LoadAllLevels()
    {
        var wrapper = JsonUtility.FromJson<LevelDataListWrapper>(levelsJson.text);
        _levels = wrapper.levelDataList.ConvertAll(s => s.ToLevelData());
    }

    public LevelData GetLevel(int index)
    {
        if (index < 0 || index >= _levels.Count) return null;
        return _levels[index];
    }
}