using UnityEngine;

public class BaseLocatable<TLocator> : MonoBehaviour where TLocator : MonoBehaviour
{
    protected TLocator Locator { get; private set; }

    public void Initialize(TLocator locator)
    {
        Locator = locator;
        OnInitialized();
    }

    protected virtual void OnInitialized() { }
}