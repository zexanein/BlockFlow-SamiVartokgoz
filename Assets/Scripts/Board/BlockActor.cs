using UnityEngine;

public class BlockActor : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    
    public void Initialize(BlockData data, BoardManager boardManager, ShapeRegistry shapeRegistry)
    {
        transform.position = boardManager.GridToWorld(data.Position);
        transform.rotation = Quaternion.Euler(0f, data.RotationSteps * 90, 0f);
        
        var shapeData = shapeRegistry.Get(data.ShapeType);
        meshFilter.mesh = shapeData.Mesh;
        meshRenderer.material.color = StaticMethods.GetColorFromName(data.BlockColor);
        meshRenderer.transform.localPosition = shapeData.MeshPosOffset;
        meshRenderer.transform.localRotation = Quaternion.Euler(shapeData.MeshRotOffset);
    }
}