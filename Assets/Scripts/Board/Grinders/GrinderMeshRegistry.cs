using System;
using UnityEngine;

[CreateAssetMenu]
public class GrinderMeshRegistry : ScriptableObject
{
    [SerializeField] private GrinderMeshData[] entries;
    
    [Serializable]
    public struct GrinderMeshData
    {
        [SerializeField] private GrinderSize size;
        [SerializeField] private Mesh mesh;
        
        public GrinderSize Size => size;
        public Mesh Mesh => mesh;
    }

    public GrinderMeshData Get(GrinderSize grinderSize) =>
        Array.Find(entries, entry => entry.Size == grinderSize);
}
