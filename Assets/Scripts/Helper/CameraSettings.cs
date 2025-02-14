using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraSettings
{
    public bool Orthographic { get; set; } = false;
    public float OrthographicSize { get; set; } = 1.2f;

    public float FOV = 60f;
    
}
