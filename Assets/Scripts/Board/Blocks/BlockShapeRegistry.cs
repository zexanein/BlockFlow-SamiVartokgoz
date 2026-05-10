using System;
using UnityEngine;

[CreateAssetMenu]
public class BlockShapeRegistry : ScriptableObject
{
    [SerializeField] private ShapeData[] entries;
    
    [Serializable]
    public struct ShapeData
    {
        [SerializeField] private ShapeType shape;
        [SerializeField] private Mesh mesh;
        [SerializeField] private Vector3 meshPosOffset;
        [SerializeField] private Vector3 meshRotOffset;
        [SerializeField] private Vector2Int[] baseShape;
        
        public ShapeType Shape => shape;
        public Mesh Mesh => mesh;
        public Vector3 MeshPosOffset => meshPosOffset;
        public Vector3 MeshRotOffset => meshRotOffset;
        public Vector2Int[] BaseShape => baseShape;
    }
    
    public ShapeData Get(ShapeType type) => Array.Find(entries, entry => entry.Shape == type);
}