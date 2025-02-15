using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class DemoEffectRun : DemoEffectBase
{
    private Vector3 quadPos = new Vector3(0, 0.5f, 1.45f);
    private GameObject quad;
    
    private SpriteRenderer runningManRenderer;
    private SpriteRenderer runningManRendererClone;
    private SpriteRenderer groundRenderer;

    private MeshRenderer quadRenderer;
    private Material mat;

    private float groundHalfWidth = 9.6f;
    private float groundScrollSpeed = .5f;
    private Vector3 groundStartPos;

    private List<Sprite> runningManSprites => TextureAndGaphicsFunctions.LoadSpriteSheet("RunningManSheetPSD");
    public override DemoEffectBase Init()
    {
        mat = GameObject.Instantiate<Material>(Resources.Load<Material>("RunMaterial"));

        quad = GameObject.CreatePrimitive(PrimitiveType.Quad);        
        quad.transform.position = quadPos;
        quad.transform.localScale = new Vector3(5f, 1f, 1f);                
        quadRenderer = quad.GetComponent<MeshRenderer>();
        quadRenderer.sharedMaterial = mat;
        AddToGeneratedObjectsDict(quad.name, quad);

        runningManRenderer = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("RunningMan", new Vector3(0, -0.64f, 1.5f), runningManSprites.First());
        SimpleSpriteAnimator simpleSpriteAnimator = runningManRenderer.gameObject.AddComponent<SimpleSpriteAnimator>();
        simpleSpriteAnimator.Sprites = runningManSprites;
        AddToGeneratedObjectsDict(runningManRenderer.gameObject.name, runningManRenderer.gameObject);

        runningManRendererClone = GameObject.Instantiate(runningManRenderer.gameObject).GetComponent<SpriteRenderer>();
        runningManRendererClone.color = Color.black;
        runningManRendererClone.transform.Rotate(180f, 0, 0);
        AddToGeneratedObjectsDict(runningManRendererClone.gameObject.name, runningManRendererClone.gameObject);

        groundRenderer = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("GroundBase", new Vector3(0, -1f, 1f), GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("RunnerGround")));
        groundRenderer.sortingOrder = -1000;
        groundRenderer.drawMode = SpriteDrawMode.Tiled;
        groundRenderer.size = new Vector2(groundHalfWidth * 2f, 0.42f);
        
        AddToGeneratedObjectsDict(groundRenderer.gameObject.name, groundRenderer.gameObject);

        return base.Init();
    }

    public override IEnumerator Run(Action endDemoCallback)
    {
        yield return base.Run(endDemoCallback);
        
        Camera.main.backgroundColor = ApplicationController.Instance.C64PaletteArr[0];
        CameraFunctions.SetCameraSettings(Camera.main, ApplicationController.Instance.CameraSettings["orthoPixel"]);

        quad.SetActive(true);
        runningManRenderer.gameObject.SetActive(true);
        runningManRendererClone.gameObject.SetActive(true);
        groundRenderer.gameObject.SetActive(true);
        groundStartPos = groundRenderer.transform.position = new Vector3(CameraFunctions.GetCameraRect(Camera.main, Camera.main.transform.position).min.x, groundRenderer.transform.position.y, groundRenderer.transform.position.z);
        Debug.Log("CAM RECT:" + CameraFunctions.GetCameraRect(Camera.main, Camera.main.transform.position));
        

        ExecuteInUpdate = true;

        //Subscribe to input
        InputController.Instance.Fire1.Subscribe(b => HandleFireInput(b)).AddTo(Disposables);
    }

    public override void DoUpdate()
    {
        float s = (Mathf.Sin(Time.time * 2f) + 1) * .5f;
        quadRenderer.sharedMaterial.SetTextureOffset("_BaseMap", new Vector2(0f, s));
        quadRenderer.sharedMaterial.SetTextureOffset("_DetailAlbedoMap", new Vector2(0f, 1f-s));

        //Scroll ground
        if (groundRenderer.transform.position.x < -groundHalfWidth + groundStartPos.x)
            groundRenderer.transform.position = groundStartPos;
        groundRenderer.transform.position += Vector3.left * groundScrollSpeed * Time.deltaTime;

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
