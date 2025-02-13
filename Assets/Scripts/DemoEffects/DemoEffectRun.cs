using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class DemoEffectRun : DemoEffectBase
{
    private Vector3 quadPos = new Vector3(0, 0.5f, 1.45f);
    private GameObject quad;
    private MeshRenderer quadRenderer;
    Material mat;

    public override DemoEffectBase Init()
    {
        mat = GameObject.Instantiate<Material>(Resources.Load<Material>("RunMaterial"));

        quad = GameObject.CreatePrimitive(PrimitiveType.Quad);        
        quad.transform.position = quadPos;
        quad.transform.localScale = new Vector3(4f, 1f, 1f);                
        quadRenderer = quad.GetComponent<MeshRenderer>();
        quadRenderer.sharedMaterial = mat;
        AddToGeneratedObjectsDict(quad.name, quad);

        return base.Init();
    }

    public override IEnumerator Run(Action endDemoCallback)
    {
        yield return base.Run(endDemoCallback);
        Camera.main.backgroundColor = ApplicationController.Instance.C64PaletteArr[0];
        quad.SetActive(true);

        ExecuteInUpdate = true;

        //Subscribe to input
        InputController.Instance.Fire1.Subscribe(b => HandleFireInput(b)).AddTo(Disposables);
    }

    public override void DoUpdate()
    {
        float s = (Mathf.Sin(Time.time * 2f) + 1) * .5f;
        quadRenderer.sharedMaterial.SetTextureOffset("_BaseMap", new Vector2(0f, s));
        quadRenderer.sharedMaterial.SetTextureOffset("_DetailAlbedoMap", new Vector2(0f, 1f-s));
        //quadRenderer.sharedMaterial.SetTextureScale("_DetailAlbedoMap", new Vector2(1.1f, 1.1f));
        

        base.DoUpdate();
    }

    private void HandleFireInput(bool b)
    {
        if (!FirePressed && b)
        {
            ApplicationController.Instance.FadeImageInOut(1f, ApplicationController.Instance.C64PaletteArr[0], () =>
            {
                //End the demo by exiting last coroutine and calling base.End();                
                base.End();
            }, null);
        }
        FirePressed = b;
    }
}
