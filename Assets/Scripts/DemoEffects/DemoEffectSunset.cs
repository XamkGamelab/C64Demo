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
    private Material spriteScrollMaterial;
    private SpriteRenderer sunRenderer;
    private SpriteRenderer mountainRenderer;
    private SpriteRenderer imgLogoTxtSS;
    private SpriteRenderer imgLogoTxtOverride;

    //Gameplay
    private Vector2 moveInput = Vector2.zero;
    private float steerSpeed = 1f;
    
    private float currentOffsetX = 0f;
    
    public override DemoEffectBase Init()
    {
        spriteScrollMaterial = GameObject.Instantiate<Material>(Resources.Load<Material>("CustomSpriteScrolling"));

        sunRenderer = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("Sun", new Vector3(0f, 0f, 1f), "Sunset/SunsetSun");
        AddToGeneratedObjectsDict(sunRenderer.gameObject.name, sunRenderer.gameObject);

        mountainRenderer = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("Mountains", new Vector3(0, -0.32f, 1f), "Sunset/SunsetMountains");
        mountainRenderer.material = spriteScrollMaterial;
        AddToGeneratedObjectsDict(mountainRenderer.gameObject.name, mountainRenderer.gameObject);

        imgLogoTxtSS = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("LogoTxtSS", new Vector3(0.8f, 0.72f, 1f), "Sunset/SunsetTextSS");
        AddToGeneratedObjectsDict(imgLogoTxtSS.gameObject.name, imgLogoTxtSS.gameObject);

        imgLogoTxtOverride = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("LogoTxtOverride", new Vector3(0.8f, 0.48f, 1f), "Sunset/SunsetTextOverride");
        AddToGeneratedObjectsDict(imgLogoTxtOverride.gameObject.name, imgLogoTxtOverride.gameObject);

        return base.Init();
    }

    public override IEnumerator Run(Action endDemoCallback)
    {
        yield return base.Run(endDemoCallback);

        Camera.main.backgroundColor = ApplicationController.Instance.C64PaletteArr[0];
        CameraFunctions.SetCameraSettings(Camera.main, ApplicationController.Instance.CameraSettings["orthoPixel"]);
        
        //Enable all generated objects        
        GeneratedObjectsSetActive(true);

        ExecuteInUpdate = true;

        //Subscribe to input        
        InputController.Instance.Horizontal.Subscribe(f => moveInput.x = f).AddTo(Disposables);

    }

    public override void DoUpdate()
    {
        Steer(moveInput);
        base.DoUpdate();
    }

    private void Steer(Vector2 input)
    {
        //Debug.Log("INPUT STEER SPEED: " + input.x * steerSpeedMax);

        currentOffsetX += input.x * steerSpeed * Time.deltaTime;

        mountainRenderer.material.SetVector("_Offset", new Vector4(currentOffsetX, 0, 0, 0));
    }
}
