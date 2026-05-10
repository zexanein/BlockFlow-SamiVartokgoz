using System.Collections.Generic;
using UnityEngine;

public class GrinderManager : ManagerLocatable
{
    [SerializeField] private GrinderActor grinderActorPrefab;
    [SerializeField] private GrinderMeshRegistry grinderMeshRegistry;
    
    private BoardManager _boardManager;

    protected override void OnInitialized()
    {
        _boardManager = Locator.GetLocatable<BoardManager>();
    }

    public void SpawnGrinders(List<GrinderData> grinders)
    {
        foreach (var grinderData in grinders)
        {
            var grinderActor = Instantiate(grinderActorPrefab, transform);
            grinderActor.Initialize(grinderData, _boardManager, grinderMeshRegistry);
        }
    }
}