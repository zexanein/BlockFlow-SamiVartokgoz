using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseLocator<TLocator, TLocatable> : BehaviourSingleton<TLocator>
    where TLocator : MonoBehaviour
    where TLocatable : BaseLocatable<TLocator>
{
    private List<TLocatable> _locatables = new();
    private bool _locatablesLoaded;
    public event Action OnLocatablesInitialized;

    private void Awake()
    {
        LoadAllLocatables();
        InitializeLocatables();
    }

    private void LoadAllLocatables()
    {
        if (_locatablesLoaded) return;
        _locatables = GetComponentsInChildren<TLocatable>(includeInactive: false).ToList();
        _locatablesLoaded = true;
    }

    private void InitializeLocatables()
    {
        foreach (var locatable in _locatables)
        {
            locatable.Initialize(this as TLocator);
        }
    
        OnLocatablesInitialized?.Invoke();
    }

    public T GetLocatable<T>() where T : TLocatable
    {
        if (!_locatablesLoaded) LoadAllLocatables();
        return (T) _locatables.FirstOrDefault(l => l is T);
    }
}