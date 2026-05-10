using System.Collections;
using UnityEngine;

public class BlockActor : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    
    public BlockData Data { get; private set; }
    private BoardManager _boardManager;
    
    public void Initialize(BlockData data, BoardManager boardManager, BlockShapeRegistry blockShapeRegistry)
    {
        Data = data;
        _boardManager = boardManager;
        
        transform.position = boardManager.GridToWorld(data.Position);
        transform.rotation = Quaternion.Euler(0f, (int) data.Direction * 90, 0f);
        
        var shapeData = blockShapeRegistry.Get(data.ShapeType);
        meshFilter.mesh = shapeData.Mesh;
        meshRenderer.material.color = StaticMethods.GetColorFromName(data.BlockColor);
        meshRenderer.transform.localPosition = shapeData.MeshPosOffset;
        meshRenderer.transform.localRotation = Quaternion.Euler(shapeData.MeshRotOffset);
    }
    
    public void SnapToGrid()
    {
        StopAllCoroutines();
        StartCoroutine(MoveToPosition(_boardManager.GridToWorld(Data.Position)));
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        var start = transform.position;
        var elapsed = 0f;
        var duration = 0.1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            var t = elapsed / duration;
            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.position = target;
    }
}