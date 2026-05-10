using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GrinderMeshRegistry : ScriptableObject
{
    [SerializeField] private GrinderMeshData[] entries;

    private Dictionary<GrinderSize, GrinderMeshData> _lookup;

    private void OnEnable()
    {
        _lookup = new Dictionary<GrinderSize, GrinderMeshData>(entries.Length);
        foreach (var entry in entries)
            _lookup[entry.Size] = entry;
    }

    [Serializable]
    public struct GrinderMeshData
    {
        [SerializeField] private GrinderSize size;
        [SerializeField] private Mesh mesh;

        public GrinderSize Size => size;
        public Mesh Mesh => mesh;
    }

    public GrinderMeshData Get(GrinderSize grinderSize) => _lookup[grinderSize];
}
