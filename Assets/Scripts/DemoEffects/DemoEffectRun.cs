using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
using TMPro;
using DG.Tweening;

public class DemoEffectRun : DemoEffectBase
{
    private float startTime;
    private Vector3 quadPos = new Vector3(0, 0.5f, 1.45f);
    private GameObject quad;
    
    private SpriteRenderer runningManRenderer;
    private SpriteRenderer runningManRendererClone;
    private SpriteRenderer groundRenderer;

    private SimpleSpriteAnimator manSpriteAnimator;
    //Text scroller
    private RectTransform txtRect;    
    private TMP_Text txt;    
    public float scrollSpeed = 2000f;
    
    private MeshRenderer quadRenderer;
    private Material mat;

    private float groundHalfWidth = 9.6f;
    private float groundScrollSpeedMin = .5f;
    private float groundScrollSpeedMax = 2f;

    private float minSpeedPercent = .1f;
    private float currentSpeedPercent = .1f;
    private float speedDecrement = 0.3f;
    private float speedIncrecrement = 0.05f;

    private float rmTransformSpeed = 0f;
    private float rmMaxTransformSpeed = .1f;

    private Vector3 groundStartPos;    
    private bool nextInputLeft = true;
    private bool goalReached = false;

    private VertexGradient gradientLeft = new VertexGradient(ApplicationController.Instance.C64PaletteArr[6], ApplicationController.Instance.C64PaletteArr[0], ApplicationController.Instance.C64PaletteArr[9], ApplicationController.Instance.C64PaletteArr[0]);
    
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
        manSpriteAnimator = runningManRenderer.gameObject.AddComponent<SimpleSpriteAnimator>();
        manSpriteAnimator.Sprites = runningManSprites;
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

        //Waving text
        txtRect = ApplicationController.Instance.UI.CreateRectTransformObject("Text_scroller", new Vector2(UIController.GetCanvasSize().Value.x, 8), new Vector3(-30f, -30f, 0), Vector2.one * .5f, Vector2.one * .5f);
        txtRect.pivot = new Vector2(1f, 0.5f);
        txt = TextFunctions.AddTextMeshProTextComponent(txtRect, "C64_Pro_Mono-STYLE", 8, ApplicationController.Instance.C64PaletteArr[1]);
        txt.alignment = TextAlignmentOptions.MidlineRight;
        txt.enableVertexGradient = true;        
        txt.colorGradient = gradientLeft;
        txt.text = "RIGHT LEFT RIGHT LEFT RIGHT SPEED: " + (int)(currentSpeedPercent * 100) + "%";                
        AddToGeneratedObjectsDict(txtRect.gameObject.name, txtRect.gameObject);

        return base.Init();
    }

    public override IEnumerator Run(Action endDemoCallback)
    {
        yield return base.Run(endDemoCallback);

        goalReached = false;
        startTime = Time.time;
        currentSpeedPercent = minSpeedPercent = .1f;

        Camera.main.backgroundColor = ApplicationController.Instance.C64PaletteArr[0];
        CameraFunctions.SetCameraSettings(Camera.main, ApplicationController.Instance.CameraSettings["orthoPixel"]);

        GeneratedObjectsSetActive(true);
        
        groundStartPos = groundRenderer.transform.position = new Vector3(CameraFunctions.GetCameraRect(Camera.main, Camera.main.transform.position).min.x, groundRenderer.transform.position.y, groundRenderer.transform.position.z);
        
        ExecuteInUpdate = true;

        nextInputLeft = true;
        //Subscribe to input
        InputController.Instance.Horizontal.Subscribe(f => HandleHorizontalInput(f)).AddTo(Disposables);
    }

    public override void DoUpdate()
    {
        //Decrement speed
        currentSpeedPercent = Mathf.Clamp(currentSpeedPercent - speedDecrement * Time.deltaTime, minSpeedPercent, 1f);
        float animDelay = Mathf.Lerp(0.01f, 0.1f, 1f - currentSpeedPercent);
        manSpriteAnimator.AnimationFrameDelay = animDelay;
        runningManRendererClone.GetComponent<SimpleSpriteAnimator>().AnimationFrameDelay = animDelay;

        float s = (Mathf.Sin(Time.time * 2f) + 1) * .5f;
        quadRenderer.sharedMaterial.SetTextureOffset("_BaseMap", new Vector2(0f, s));
        quadRenderer.sharedMaterial.SetTextureOffset("_DetailAlbedoMap", new Vector2(0f, 1f-s));

        if (currentSpeedPercent > .5f)
        {
            rmTransformSpeed = Mathf.Lerp(0.01f, rmMaxTransformSpeed, currentSpeedPercent);
            runningManRenderer.gameObject.transform.Translate(Vector3.right * rmTransformSpeed * Time.deltaTime);
            runningManRendererClone.gameObject.transform.Translate(Vector3.right * rmTransformSpeed * Time.deltaTime);
            txtRect.gameObject.transform.Translate(Vector3.right * rmTransformSpeed * Time.deltaTime);
        }

        //Scroll ground
        if (groundRenderer.transform.position.x < -groundHalfWidth + groundStartPos.x)
            groundRenderer.transform.position = groundStartPos;

        float groundScrollSpeed = Mathf.Lerp(groundScrollSpeedMin, groundScrollSpeedMax, currentSpeedPercent);
        groundRenderer.transform.position += Vector3.left * groundScrollSpeed * Time.deltaTime;

        //Wave text
        txt.text = "RIGHT LEFT RIGHT LEFT RIGHT SPEED: " + ((int)(currentSpeedPercent * 100f)).ToString("000") + "%";
        TextFunctions.TextMeshEffect(txt, startTime, new TextEffectSettings { EffectType = TextEffectSettings.TextEffectType.SinCurve, SinCurveSpeed = 4f, SinCurveMagnitude = 8f, SinCurveScale = 0.05f });

        if (!goalReached && runningManRenderer.gameObject.transform.position.x > CameraFunctions.GetCameraRect(Camera.main, Camera.main.transform.position).max.x)
        {
            goalReached = true;
            quad.transform.DOLocalMoveY(quadPos.y - 1f, .5f).SetEase(Ease.OutBounce);
            ApplicationController.Instance.FadeImageInOut(1f, ApplicationController.Instance.C64PaletteArr[0], () =>
            {
                //End the demo by exiting last coroutine and calling base.End();
                ExecuteInUpdate = false;
                base.End();
            }, null);
        }
        base.DoUpdate();
    }

    private void HandleHorizontalInput(float f)
    {
        if (f < 0 && nextInputLeft)
        {
            nextInputLeft = false;            
            currentSpeedPercent += speedIncrecrement;
        }
        else if (f > 0 && !nextInputLeft)
        {
            nextInputLeft = true;            
            currentSpeedPercent += speedIncrecrement;
        }
    }
}
