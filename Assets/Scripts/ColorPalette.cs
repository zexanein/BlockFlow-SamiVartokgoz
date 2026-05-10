using System;
using UnityEngine;

[CreateAssetMenu]
public class ColorPalette : ScriptableObject
{
    [SerializeField] private ColorEntry[] entries;
    
    [Serializable]
    public struct ColorEntry
    {
        [SerializeField] private string name;
        [SerializeField] private Color color;
        
        public string Name => name;
        public Color Color => color;
    }
    
    public Color Get(string colorName) => Array.Find(entries, entry => entry.Name == colorName).Color;
}
