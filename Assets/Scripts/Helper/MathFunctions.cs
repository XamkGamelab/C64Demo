using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathFunctions
{
    public static float GetSin(float time, float speed, float magnitude)
    {
        return Mathf.Sin(speed * time) * magnitude;
    }
}
