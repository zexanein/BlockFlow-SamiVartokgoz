using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BlockActor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    
    [Header("Animation Settings")]
    [SerializeField] private float snapDuration = 0.1f;
    [SerializeField] private float exitDurationPerCell = 0.25f;
    
    [Header("Ice Effect")]
    [SerializeField] private MeshFilter iceEffectMeshFilter;
    [SerializeField] private TMP_Text iceEffectText;
    
    private readonly List<Collider> _colliders = new();

    private const float ExitOffset = 0.5f;
    
    public void RegisterCollider(Collider col) => _colliders.Add(col);

    public BlockData Data { get; private set; }
    private BoardManager _boardManager;
    private BlockManager _blockManager;

    public void Initialize(BlockData data, BoardManager boardManager, BlockManager blockManager, BlockShapeRegistry blockShapeRegistry)
    {
        Data = data;
        _boardManager = boardManager;
        _blockManager = blockManager;

        transform.position = boardManager.GridToWorld(data.Position);
        transform.rotation = Quaternion.Euler(0f, (int)data.Direction * 90, 0f);

        var shapeData = blockShapeRegistry.Get(data.ShapeType);
        meshFilter.mesh = shapeData.Mesh;
        meshRenderer.sharedMaterial = ColorManager.Palette.GetMaterial(data.BlockColor);
        meshRenderer.transform.localPosition = shapeData.MeshPosOffset;
        meshRenderer.transform.localRotation = Quaternion.Euler(shapeData.MeshRotOffset);

        if (data.IceEffectDuration > 0) ApplyIceVisuals();
        
        _blockManager.OnBlockCleared += OnBlockCleared;
    }
    
    private void  OnDestroy()
    {
        _blockManager.OnBlockCleared -= OnBlockCleared;
    }
    
    private void OnBlockCleared()
    {
        if (Data.IceEffectDuration > 0) UpdateIceEffectText();
        else RemoveIceVisuals();
    }

    private void ApplyIceVisuals()
    {
        iceEffectMeshFilter.gameObject.SetActive(true);
        iceEffectMeshFilter.mesh = meshFilter.mesh;
        iceEffectMeshFilter.transform.localPosition = meshRenderer.transform.localPosition;
        iceEffectMeshFilter.transform.localRotation = meshRenderer.transform.localRotation;
        UpdateIceEffectText();
    }

    private void RemoveIceVisuals()
    {
        iceEffectMeshFilter.gameObject.SetActive(false);
    }

    private void UpdateIceEffectText()
    {
        if (iceEffectText == null) return;
        iceEffectText.text = Data.IceEffectDuration.ToString();
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