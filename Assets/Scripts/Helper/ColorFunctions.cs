using UnityEngine;

public static class ColorFunctions
{
    

    public static Color[] ColorsFromPaletteStripTexture(Texture2D texture2D, int colorsCount)
    {
        Color[] colors = new Color[colorsCount];
        
        int step = texture2D.width / colorsCount;
        int end = texture2D.width / step;

        for (int w = 0; w < end; w++)
        {
            Color pixelColor = texture2D.GetPixel(w*step, 0);            
            colors[w] = pixelColor;
        }

        return colors;
    }


}
