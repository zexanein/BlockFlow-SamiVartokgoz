using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockActor : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private float snapDuration = 0.1f;
    [SerializeField] private float exitDurationPerCell = 0.25f;
    
    private readonly List<Collider> _colliders = new();

    private const float ExitOffset = 0.5f;
    
    public void RegisterCollider(Collider col) => _colliders.Add(col);

    public BlockData Data { get; private set; }
    private BoardManager _boardManager;

    public void Initialize(BlockData data, BoardManager boardManager, BlockShapeRegistry blockShapeRegistry)
    {
        Data = data;
        _boardManager = boardManager;

        transform.position = boardManager.GridToWorld(data.Position);
        transform.rotation = Quaternion.Euler(0f, (int)data.Direction * 90, 0f);

        var shapeData = blockShapeRegistry.Get(data.ShapeType);
        meshFilter.mesh = shapeData.Mesh;
        meshRenderer.sharedMaterial = ColorManager.Palette.GetMaterial(data.BlockColor);
        meshRenderer.transform.localPosition = shapeData.MeshPosOffset;
        meshRenderer.transform.localRotation = Quaternion.Euler(shapeData.MeshRotOffset);
    }

    public void SnapToGrid()
    {
        StopAllCoroutines();
        StartCoroutine(MoveToPosition(_boardManager.GridToWorld(Data.Position)));
    }

    public void PlayExitAnimation(Vector2Int[] shape, Direction exitDir, Action onComplete)
    {
        StopAllCoroutines();
        
        foreach (var col in _colliders)
            col.enabled = false;

        var cellDepth = exitDir is Direction.Left or Direction.Right
            ? shape.Select(c => c.x).Distinct().Count()
            : shape.Select(c => c.y).Distinct().Count();

        var distance = (cellDepth + ExitOffset) * _boardManager.CellSize;
        var duration = exitDurationPerCell * cellDepth;

        StartCoroutine(ExitCoroutine(exitDir.ToVector3(), distance, duration, onComplete));
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        var start = transform.position;
        var elapsed = 0f;

        while (elapsed < snapDuration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, elapsed / snapDuration);
            yield return null;
        }

        transform.position = target;
    }

    private IEnumerator ExitCoroutine(Vector3 worldDir, float distance, float duration, Action onComplete)
    {
        yield return StartCoroutine(MoveToPosition(_boardManager.GridToWorld(Data.Position)));
        var start = transform.position;
        var target = start + worldDir * distance;
        var elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, elapsed / duration);
            yield return null;
        }

        onComplete?.Invoke();
    }
}