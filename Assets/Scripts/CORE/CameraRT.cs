using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class CameraRT : MonoBehaviour
{
    public GameObject RenderTextureObject;
    private Camera cameraRT => GetComponent<Camera>();

    public void AnimateIntro()
    {
        //TODO: It would be best to align camera and target "screen" object position as well, since we are animating the camera transform position...
        //Point being, that we wouldn't need to care neither about the camera's position nor the "screens" position.

        float distanceToObject = CameraFunctions.CameraDistanceFromObjectHeight(cameraRT, RenderTextureObject.GetComponent<Renderer>().bounds.size.y);
        Debug.Log("Target distance to object: " + distanceToObject);

        Vector3 cameraTargetPosition = RenderTextureObject.transform.position;
        cameraTargetPosition.z = RenderTextureObject.transform.position.z - distanceToObject;

        cameraRT.transform.DOMove(cameraTargetPosition, 5f);
    }
}
