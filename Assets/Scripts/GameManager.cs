using System;
using UnityEngine;

public class GameManager : ManagerLocatable
{
    private LevelManager _levelManager;
    private BlockManager _blockManager;
    private int _currentLevelIndex;
    private float _timer;
    private bool _isPlaying;

    public GameState State { get; private set; }
    
    public event Action<float> OnTimerUpdated;
    public event Action OnFail;
    public event Action OnWin;

    protected override void OnInitialized()
    {
        _levelManager = Locator.GetLocatable<LevelManager>();
        _blockManager = Locator.GetLocatable<BlockManager>();

        _blockManager.OnBlockCleared += OnBlockCleared;
    }

    private void OnDestroy()
    {
        _blockManager.OnBlockCleared -= OnBlockCleared;
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (!_isPlaying) return;

        _timer -= Time.deltaTime;
        OnTimerUpdated?.Invoke(_timer);

        if (_timer > 0f) return;
        _isPlaying = false;
        State = GameState.LevelFailed;
        OnFail?.Invoke();
    }

    public void StartGame()
    {
        _currentLevelIndex = 0;
        PlayCurrentLevel();
    }

    private void PlayCurrentLevel()
    {
        _levelManager.LoadLevel(_currentLevelIndex);
        _timer = _levelManager.GetLevelTimeLimit(_currentLevelIndex);
        _isPlaying = true;
        State = GameState.Playing;
    }

    private void OnBlockCleared()
    {
        if (_blockManager.ActiveBlockCount > 0) return;

        _isPlaying = false;
        State = GameState.LevelComplete;
        OnWin?.Invoke();
    }
}