using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class DemoEffectSunset : DemoEffectBase
{
    private Vector3 quadPos = new Vector3(-3f, -0.7f, 1.28f);
    private GameObject quad;
    private MeshRenderer quadRenderer;

    private Material spriteScrollMaterial;
    private Material gridMaterial;

    private SpriteRenderer sunRenderer;
    private SpriteRenderer mountainRenderer;
    private SpriteRenderer imgLogoTxtSS;
    private SpriteRenderer imgLogoTxtOverride;

    //Gameplay
    private Vector2 gridOffset = Vector2.zero;
    private Vector2 moveInput = Vector2.zero;
    private float steerSpeed = 1f;
    
    private float mountainsOffsetX = 0f;
    
    public override DemoEffectBase Init()
    {
        spriteScrollMaterial = GameObject.Instantiate<Material>(Resources.Load<Material>("CustomSpriteScrolling"));
        gridMaterial = GameObject.Instantiate<Material>(Resources.Load<Material>("GridMaterial"));
        
        gridMaterial.color = ApplicationController.Instance.C64PaletteArr[15];

        for (int i = 0; i < 7; i++)
        {
            quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = "Ground_" + i;
            quad.transform.position = quadPos;
            quad.transform.rotation = Quaternion.Euler(70f, 0, 0);
            quadRenderer = quad.GetComponent<MeshRenderer>();
            quadRenderer.sharedMaterial = gridMaterial;
            AddToGeneratedObjectsDict(quad.name, quad);
            quadPos.x++;
        }

        sunRenderer = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("Sun", new Vector3(0f, 0.16f, 1f), "Sunset/SunsetSun");
        AddToGeneratedObjectsDict(sunRenderer.gameObject.name, sunRenderer.gameObject);

        mountainRenderer = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("Mountains", new Vector3(0, -0.16f, 1f), "Sunset/SunsetMountains");
        mountainRenderer.material = spriteScrollMaterial;
        AddToGeneratedObjectsDict(mountainRenderer.gameObject.name, mountainRenderer.gameObject);

        imgLogoTxtSS = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("LogoTxtSS", new Vector3(0.8f, 0.72f, 1.2f), "Sunset/SunsetTextSS");
        AddToGeneratedObjectsDict(imgLogoTxtSS.gameObject.name, imgLogoTxtSS.gameObject);

        imgLogoTxtOverride = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("LogoTxtOverride", new Vector3(0.8f, 0.48f, 1.2f), "Sunset/SunsetTextOverride");
        AddToGeneratedObjectsDict(imgLogoTxtOverride.gameObject.name, imgLogoTxtOverride.gameObject);

        return base.Init();
    }

    public override IEnumerator Run(Action endDemoCallback)
    {
        yield return base.Run(endDemoCallback);

        Camera.main.backgroundColor = ApplicationController.Instance.C64PaletteArr[0];
        CameraFunctions.SetCameraSettings(Camera.main, ApplicationController.Instance.CameraSettings["perspectiveFov90"]);
        
        //Enable all generated objects        
        GeneratedObjectsSetActive(true);

        ExecuteInUpdate = true;

        //Subscribe to input        
        InputController.Instance.Horizontal.Subscribe(f => moveInput.x = f).AddTo(Disposables);

    }

    public override void DoUpdate()
    {
        gridOffset.y += Time.deltaTime;
        gridMaterial.SetTextureOffset("_BaseMap", gridOffset);
        Steer(moveInput);
        base.DoUpdate();
    }

    private void Steer(Vector2 input)
    {
        float currentSteer = input.x * steerSpeed * Time.deltaTime;

        mountainsOffsetX += currentSteer * .5f;
        gridOffset.x += currentSteer;

        mountainRenderer.material.SetVector("_Offset", new Vector4(mountainsOffsetX, 0, 0, 0));
    }
}
