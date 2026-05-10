using UnityEngine;

public class ColorManager : MonoBehaviour
{
    private const string PalettePath = "ColorPalette";
    
    private static ColorPalette _palette;
    public static ColorPalette Palette
    {
        get
        {
            if (_palette == null)
            {
                _palette = Resources.Load<ColorPalette>(PalettePath);
            }
            
            return _palette;
        }
    }
}
