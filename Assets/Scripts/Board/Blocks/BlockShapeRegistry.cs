using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BlockShapeRegistry : ScriptableObject
{
    [SerializeField] private ShapeData[] entries;

    private Dictionary<ShapeType, ShapeData> _lookup;

    private void OnEnable()
    {
        _lookup = new Dictionary<ShapeType, ShapeData>(entries.Length);
        foreach (var entry in entries)
            _lookup[entry.Shape] = entry;
    }

    [Serializable]
    public struct ShapeData
    {
        [SerializeField] private ShapeType shape;
        [SerializeField] private Mesh mesh;
        [SerializeField] private Vector3 meshPosOffset;
        [SerializeField] private bool centerVisuals;
        [SerializeField] private Vector2Int[] baseShape;

        public ShapeType Shape => shape;
        public Mesh Mesh => mesh;
        public Vector3 MeshPosOffset => meshPosOffset;
        public Vector2Int[] BaseShape => baseShape;
        public bool CenterVisuals => centerVisuals;
    }

    public ShapeData Get(ShapeType type) => _lookup[type];
}