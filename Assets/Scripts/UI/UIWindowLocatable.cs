using System;
using UnityEngine;

public class UIWindowLocatable : BaseLocatable<UIWindowLocator>
{
    public event Action<bool> OnVisibilityChanged;
    [SerializeField] private GameObject panelContent;
    
    public void Open()
    {
        var objectToActivate = panelContent != null ? panelContent : gameObject;
        objectToActivate.SetActive(true);
        
        OnOpened();
        OnVisibilityChanged?.Invoke(true);
    }
    
    public void Close()
    {
        var objectToDeactivate = panelContent != null ? panelContent : gameObject;
        objectToDeactivate.SetActive(false);
        
        OnClosed();
        OnVisibilityChanged?.Invoke(false);
    }
    
    protected virtual void OnOpened() { }
    protected virtual void OnClosed() { }
}