using System;
using System.Collections.Generic;
using UnityEngine;

public class Noise
{
    /*
        No need for texture here, but just a reminder:

        - Normalizing and scale could be parameters
        - Add scale also to FBM function

        public int width = 500;
        public int height = 500;
        public float scale = 0.01f;

        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float n = PerlinNoise2D.Noise2D(x * scale, y * scale);
                n = (n + 1.0f) / 2.0f; // Normalize from [-1,1] to [0,1]
                byte c = (byte)Mathf.RoundToInt(255 * n);
                Color color = new Color32(c, c, c, 255);

                pixels[y * width + x] = color;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        */
    private static int[] permutation = MakePermutation();

    private static void Shuffle(List<int> arrayToShuffle)
    {
        System.Random rand = new System.Random();
        for (int e = arrayToShuffle.Count - 1; e > 0; e--)
        {
            int index = rand.Next(e);
            int temp = arrayToShuffle[e];
            arrayToShuffle[e] = arrayToShuffle[index];
            arrayToShuffle[index] = temp;
        }
    }

    private static int[] MakePermutation()
    {
        List<int> perm = new List<int>();
        for (int i = 0; i < 256; i++)
        {
            perm.Add(i);
        }

        Shuffle(perm);

        // Duplicate the array
        int[] fullPerm = new int[512];
        for (int i = 0; i < 256; i++)
        {
            fullPerm[i] = perm[i];
            fullPerm[256 + i] = perm[i];
        }

        return fullPerm;
    }

    private static Vector2 GetConstantVector(int v)
    {
        int h = v & 3;
        switch (h)
        {
            case 0: return new Vector2(1.0f, 1.0f);
            case 1: return new Vector2(-1.0f, 1.0f);
            case 2: return new Vector2(-1.0f, -1.0f);
            default: return new Vector2(1.0f, -1.0f);
        }
    }

    private static float Fade(float t)
    {
        return ((6f * t - 15f) * t + 10f) * t * t * t;
    }

    private static float Lerp(float t, float a1, float a2)
    {
        return a1 + t * (a2 - a1);
    }

    public static float Noise2D(float x, float y, float scale)
    {
        int X = Mathf.FloorToInt(x) & 255;
        int Y = Mathf.FloorToInt(y) & 255;

        float xf = x - Mathf.Floor(x);
        float yf = y - Mathf.Floor(y);

        Vector2 topRight = new Vector2(xf - 1.0f, yf - 1.0f);
        Vector2 topLeft = new Vector2(xf, yf - 1.0f);
        Vector2 bottomRight = new Vector2(xf - 1.0f, yf);
        Vector2 bottomLeft = new Vector2(xf, yf);

        int valueTopRight = permutation[permutation[X + 1] + Y + 1];
        int valueTopLeft = permutation[permutation[X] + Y + 1];
        int valueBottomRight = permutation[permutation[X + 1] + Y];
        int valueBottomLeft = permutation[permutation[X] + Y];

        float dotTopRight = Vector2.Dot(topRight,GetConstantVector(valueTopRight));
        float dotTopLeft = Vector2.Dot(topLeft, GetConstantVector(valueTopLeft));
        float dotBottomRight = Vector2.Dot(bottomRight, GetConstantVector(valueBottomRight));
        float dotBottomLeft = Vector2.Dot(bottomLeft, GetConstantVector(valueBottomLeft));

        float u = Fade(xf);
        float v = Fade(yf);

        return Lerp(u,
            Lerp(v, dotBottomLeft, dotTopLeft),
            Lerp(v, dotBottomRight, dotTopRight)
        );
    }

    public static float FractalBrownianMotion(float x, float y, float scale, int numOctaves)
    {
        float result = 0.0f;
        float amplitude = 1.0f;
        float frequency = 0.005f;

        for (int octave = 0; octave < numOctaves; octave++)
        {
            float n = amplitude * Noise2D(x * frequency, y * frequency, scale);
            result += n;

            amplitude *= 0.5f;
            frequency *= 2.0f;
        }

        return result;
    }
}