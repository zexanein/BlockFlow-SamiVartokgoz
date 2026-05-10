using System;
using UnityEngine;

[CreateAssetMenu]
public class ColorPalette : ScriptableObject
{
    [SerializeField] private ColorEntry[] entries;
    
    [Serializable]
    public struct ColorEntry
    {
        [SerializeField] private Color color;
        [SerializeField] private Material material;
        
        public Color Color => color;
        public Material Material => material;
    }
    
    public Color GetColor(int id) => entries[id].Color;
    public Material GetMaterial(int id) => entries[id].Material;
}
