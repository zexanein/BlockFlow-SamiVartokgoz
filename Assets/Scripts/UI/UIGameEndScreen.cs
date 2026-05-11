using UnityEngine;
using UnityEngine.UI;

public class UIGameEndScreen : UIWindowLocatable
{
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private Button nextLevelButton;
    private GameManager _gameManager;
    
    protected override void OnInitialized()
    {
        _gameManager = ManagerLocator.Instance.GetLocatable<GameManager>();
        _gameManager.OnWin += ShowWinScreen;
        _gameManager.OnFail += ShowLoseScreen;
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
    }

    private void OnNextLevelClicked()
    {
        var levelManager = ManagerLocator.Instance.GetLocatable<LevelManager>();
        levelManager.LoadNextLevel();
        Close();
    }

    private void ShowWinScreen()
    {
        if (winPanel != null) winPanel.SetActive(true);
        if (losePanel != null) losePanel.SetActive(false);
        Open();
    }
    
    private void ShowLoseScreen()
    {
        if (losePanel != null) losePanel.SetActive(true);
        if (winPanel != null) winPanel.SetActive(false);
        Open();
    }
}
