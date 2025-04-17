using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathFunctions
{
    public static float GetSin(float time, float speed, float magnitude)
    {
        return Mathf.Sin(speed * time) * magnitude;
    }

    public static Vector3 RotateAroundPoint(Vector3 centerPoint, float angle, float orbitRadius, Vector3 orbitAxis)
    {
    

    //private float currentAngle = 0f;

    // Update the angle based on speed and time
        //currentAngle += orbitSpeed * Time.deltaTime;
        //currentAngle %= 360f; // Keep the angle between 0-360

        // Calculate the new position
        Vector3 offset = Quaternion.AngleAxis(angle, orbitAxis) * (Vector3.up * orbitRadius);
        return centerPoint + offset;
    }
}
