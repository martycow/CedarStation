using UnityEngine;

namespace Game.General
{
    public static class Utilities
    {
        public static class Vectors
        {
            
        }

        public static class Colors
        {
            public static Color Darken(Color c, float factor = 0.75f)
            {
                return new Color(c.r * factor, c.g * factor, c.b * factor);
            }

            public static Color FromHex(string hex, Color fallbackColor = default)
            {
                return ColorUtility.TryParseHtmlString(hex, out var color) ? color : fallbackColor;
            }
        }
    }
}