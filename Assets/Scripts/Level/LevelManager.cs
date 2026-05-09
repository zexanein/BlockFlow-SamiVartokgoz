using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : ManagerLocatable
{
    private BoardManager _boardManager;
    
    protected override void OnInitialized()
    {
        _boardManager = Locator.GetLocatable<BoardManager>();
        var testLevelData = new LevelData(10, 10);
        _boardManager.CreateBoard(testLevelData);
    }
}
