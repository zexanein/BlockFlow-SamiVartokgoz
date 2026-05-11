using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGameOverlay : UIWindowLocatable
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text levelText;
    
    private GameManager _gameManager;
    private LevelManager _levelManager;

    protected override void OnInitialized()
    {
        _gameManager = ManagerLocator.Instance.GetLocatable<GameManager>();
        _levelManager = ManagerLocator.Instance.GetLocatable<LevelManager>();
        
        _gameManager.OnTimerUpdated += OnTimerUpdated;
        _levelManager.OnLevelLoaded += OnLevelLoaded;
    }

    private void OnDestroy()
    {
        if (_gameManager != null)
            _gameManager.OnTimerUpdated -= OnTimerUpdated;
        
        if (_levelManager != null)
            _levelManager.OnLevelLoaded -= OnLevelLoaded;
    }

    private void OnTimerUpdated(float time)
    {
        if (timerText == null) return;
        var minutes = Mathf.FloorToInt(time / 60f);
        var seconds = Mathf.FloorToInt(time % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void OnLevelLoaded(int level)
    {
        if (levelText == null) return;
        levelText.text = $"LEVEL\n{level + 1}";
    }
}
