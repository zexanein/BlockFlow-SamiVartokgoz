using System.Collections;
using UnityEngine;

public class GrinderActor : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    
    public void Initialize(GrinderData data, BoardManager boardManager, GrinderMeshRegistry meshRegistry)
    {
        var grinderDirection = StaticMethods.GetGrinderDirection(data, boardManager.Width, boardManager.Height);
        transform.position = boardManager.GridToWorld(data.Position);
        transform.rotation = Quaternion.Euler(0f, (int)grinderDirection * 90, 0f);

        meshFilter.mesh = meshRegistry.Get(data.Size).Mesh;
        var localX = -0.5f * (int)data.Size;
        meshRenderer.transform.localPosition = meshRenderer.transform.localPosition.WithX(localX);
        meshRenderer.sharedMaterial = ColorManager.Palette.GetMaterial(data.GrinderColor);
    }
}
