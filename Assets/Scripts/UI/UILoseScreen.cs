using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILoseScreen : UIWindowLocatable
{
    private GameManager _gameManager;
    
    protected override void OnInitialized()
    {
        _gameManager = ManagerLocator.Instance.GetLocatable<GameManager>();
        _gameManager.OnFail += Open;
    }

    private void OnDestroy()
    {
        _gameManager.OnFail -= Open;
    }
}
