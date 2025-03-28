using UnityEngine;
using System.Linq;
using UnityEditor.Rendering;

public static class CameraFunctions
{
    public static void SetCameraSettings(Camera cam, CameraSettings cameraSettings)
    {
        Debug.Log("CHANGING CAMERA SETTINGS FOR: " + cam.name);
        cam.orthographic = cameraSettings.Orthographic;
        cam.orthographicSize = cameraSettings.OrthographicSize;
        cam.fieldOfView = cameraSettings.FOV;
    }

    public static float CameraDistanceFromObjectHeight(Camera cam, float objectHeight)
    {
        float distance = objectHeight * 0.5f / Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);        
        return distance;
    }

    public static bool IsBoundsWithinViewport(Camera cam, Vector3 nextCameraPosition, Bounds _bounds)
    {
        Rect cameraRect = GetCameraRect(cam, nextCameraPosition);

        //Is min/max points both within camera viewport
        return cameraRect.Contains(_bounds.min) || cameraRect.Contains(_bounds.max);
    }

    public static bool IsPointWithinViewport(Camera cam, Vector3 nextCameraPosition, Vector2 point)
    {
        Rect cameraRect = GetCameraRect(cam, nextCameraPosition);

        //Is point within camera viewport
        return cameraRect.Contains(point);
    }

    public static bool IsPointWithinRect(Rect rect, Vector2 point)
    {
        //Is point within rect
        return rect.Contains(point);
    }

    public static bool IsRendererBoundsWithinViewport(Camera cam, Vector3 nextCameraPosition, Bounds rendererBounds)
    {
        Rect cameraRect = GetCameraRect(cam, nextCameraPosition);
        if (cameraRect.Contains(rendererBounds.min) || cameraRect.Contains(rendererBounds.max))
            return true;
        else
            return false;
    }

    public static bool IsRendererBoundsWithinRect(Rect rect, Bounds rendererBounds)
    {
        if (rect.Contains(rendererBounds.min) || rect.Contains(rendererBounds.max))
            return true;
        else
            return false;
    }

    public static Rect GetCameraRect(Camera cam, Vector3 nextCameraPosition)
    {
        Rect cameraRect = new Rect(nextCameraPosition, new Vector2(cam.orthographicSize * 2f * cam.aspect, cam.orthographicSize * 2f));
        cameraRect.center = nextCameraPosition;
        return cameraRect;
    }
}