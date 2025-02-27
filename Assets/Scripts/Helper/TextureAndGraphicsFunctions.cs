using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextureAndGaphicsFunctions
{
    public static int[] MetalDarkColorGradient = new int[] {
        0, 0, 0, 0, 7, 8, 9, 1, 4, 3, 2, 0, 0, 0, 0, 0,
        0, 0, 7, 7, 8, 8, 9, 9, 1, 9, 1, 1, 4, 3, 2, 0
    };

    public static int[] MetalGoldColorGradient = new int[] {
        0, 7, 0, 7, 7, 8, 7, 8, 8, 9, 8, 9, 9, 1, 9, 1,
        1, 9, 1, 9, 9, 8, 9, 8, 8, 7, 8, 7, 7, 0, 7, 0
    };

    public static int[] MetalSteelColorGradient = new int[] {
        0, 2, 0, 2, 2, 3, 2, 3, 3, 4, 3, 4, 4, 1, 4, 1,
        1, 4, 1, 4, 4, 3, 4, 3, 3, 2, 8, 2, 2, 0, 2, 0
    };

    private static string[] asciiChars = { "#", "#", "@", "%", "=", "+", "*", ":", "-", ".", " " };

    public static string ConvertToAscii(Texture2D texture2D)
    {
        string ascii = "";
        for (int h = texture2D.height; h > 0; h--)
        {
            for (int w = 0; w < texture2D.width; w++)
            {
                Color pixelColor = texture2D.GetPixel(w, h);
                ascii += asciiChars[(int)(((pixelColor.r + pixelColor.g + pixelColor.b) / 3f) * (asciiChars.Length - 1))];
            }
            ascii += "\n";
        }
        return ascii;
    }

    public static string ConvertToAscii(Texture2D texture2D, Rect sprite)
    {
        string ascii = "";
        for (int h = (int)sprite.height; h > 0; h--)
        {
            for (int w = 0; w < (int)sprite.width; w++)
            {
                Color pixelColor = texture2D.GetPixel(w + (int)sprite.position.x, h + (int)sprite.position.y);
                ascii += asciiChars[(int)(((pixelColor.r + pixelColor.g + pixelColor.b) / 3f) * (asciiChars.Length - 1))];
            }
            ascii += "\n";
        }
        return ascii;
    }

    public static List<Sprite> LoadSpriteSheet(string imageName)
    {
        return Resources.LoadAll<Sprite>(imageName).ToList();
    }

    public static SpriteRenderer InstantiateSpriteRendererGO(string goName, Vector3 pos, string spritePath)
    {
        return InstantiateSpriteRendererGO(goName, pos, Resources.Load<Sprite>(spritePath));
    }
    public static SpriteRenderer InstantiateSpriteRendererGO(string goName, Vector3 pos, Sprite sprite)
    {
        GameObject go = new GameObject(goName);
        go.transform.position = pos;
        SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        return spriteRenderer;
    }
}
