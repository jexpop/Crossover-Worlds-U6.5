using UnityEngine;

namespace CrossoverWorlds.UI.Utilities
{
    public static class ColorUtilities
    {
        public static UnityEngine.Color SetAlpha(UnityEngine.Color color, float alpha)
        {
            UnityEngine.Color newColor = color;
            newColor.a = Mathf.Clamp(alpha, 0, 1);
           return newColor;
        }
    }
}
