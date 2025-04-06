using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class CameraRT : MonoBehaviour
{
    public GameObject RenderTextureObject;
    private Camera cameraRT => GetComponent<Camera>();

    private Vector3 cameraInitPosition;
    private Vector3 cameraTargetScreenPosition;
    public CameraRT Init()
    {
        cameraInitPosition = transform.position;
        //TODO: It would be best to align camera and target "screen" object position as well, since we are animating the camera transform position...
        //Point being, that we wouldn't need to care neither about the camera's position nor the "screens" position.

        //TODO: WHY THE FUCK THIS RETURNS WRONG POSITION?!?!?!?!?!?!?!?!?!?! Again some Unity piece of shit timing issue?
        float distanceToObject = CameraFunctions.CameraDistanceFromObjectHeight(cameraRT, RenderTextureObject.GetComponent<Renderer>().bounds.size.y);
        Vector3 cameraTargetScreenPosition = RenderTextureObject.transform.position;
        cameraTargetScreenPosition.z = RenderTextureObject.transform.position.z - distanceToObject;
        return this;
    }

    public void AnimateScreenIn()
    {
        float distanceToObject = CameraFunctions.CameraDistanceFromObjectHeight(cameraRT, RenderTextureObject.GetComponent<Renderer>().bounds.size.y);
        Vector3 cameraTargetScreenPosition = RenderTextureObject.transform.position;
        cameraTargetScreenPosition.z = RenderTextureObject.transform.position.z - distanceToObject;
        cameraRT.transform.DOMove(cameraTargetScreenPosition, 5f);
    }

    public void AnimateScreenOut()
    {
        Debug.Log("DO MOVE TO : " + cameraInitPosition);
        cameraRT.transform.DOMove(cameraInitPosition, 5f);
    }
}
