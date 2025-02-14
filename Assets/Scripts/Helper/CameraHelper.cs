using UnityEngine;
using System.Linq;

public static class CameraFunctions
{
    public static void SetCameraSettings(Camera cam, CameraSettings cameraSettings)
    {
        Debug.Log("CHANGING CAMERA SETTINGS FOR: " + cam.name);
        cam.orthographic = cameraSettings.Orthographic;
        cam.orthographicSize = cameraSettings.OrthographicSize;
        cam.fieldOfView = cameraSettings.FOV;
    }

    public static bool IsBoundsWithinViewport(Camera cam, Vector3 nextCameraPosition, Bounds _bounds)
    {
        Rect cameraRect = GetCameraRect(cam, nextCameraPosition);

        //Is min/max points both within camera viewport
        return cameraRect.Contains(_bounds.min) || cameraRect.Contains(_bounds.max);
    }
    /**
    * Check if position is within camera's viewport.
    * @param cam Camera to check.    
    * @param nextCameraPosition Camera's position in next frame.
    * @param point Vector2 point to check.
    */
    public static bool IsPointWithinViewport(Camera cam, Vector3 nextCameraPosition, Vector2 point)
    {
        Rect cameraRect = GetCameraRect(cam, nextCameraPosition);

        //Is point within camera viewport
        return cameraRect.Contains(point);
    }

    /**
    * Check if renderer bounds are within camera's viewport
    * @param cam Camera to check.    
    * @param nextCameraPosition Camera's position in next frame.
    * @param rendererBounds Renderer bounds to check.
    */
    public static bool IsRendererBoundsWithinViewport(Camera cam, Vector3 nextCameraPosition, Bounds rendererBounds)
    {
        Rect cameraRect = GetCameraRect(cam, nextCameraPosition);
        if (cameraRect.Contains(rendererBounds.min) || cameraRect.Contains(rendererBounds.max))
            return true;
        else
            return false;
    }

    /**
    * Return given camera's orthographicSize as Rect
    * @param cam Camera to use.    
    * @param nextCameraPosition Rect's center is camera's position in next frame.
    */
    public static Rect GetCameraRect(Camera cam, Vector3 nextCameraPosition)
    {
        Rect cameraRect = new Rect(nextCameraPosition, new Vector2(cam.orthographicSize * 2f * cam.aspect, cam.orthographicSize * 2f));
        cameraRect.center = nextCameraPosition;
        return cameraRect;
    }
}