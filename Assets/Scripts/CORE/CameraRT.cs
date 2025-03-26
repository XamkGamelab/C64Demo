using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraRT : MonoBehaviour
{
    private Camera cameraRT => GetComponent<Camera>();
    void Start()
    {
        //BECAUSE THE QUAD HEIGHT (SCALE) IS 2f, this would set the quad to correct place!!!
        float distance = 2f * 0.5f / Mathf.Tan(cameraRT.fieldOfView * 0.5f * Mathf.Deg2Rad);
        Debug.Log("Distance: " + distance);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
