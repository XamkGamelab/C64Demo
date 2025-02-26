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
    private SpriteRenderer palm_0;

    //This probably needs to be a list for all palms or even a Palm class!
    private Vector3 palmInitPos;

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

        //Move z from 2 to 0 x is like 0.64 and y = quadpos.y
        palm_0 = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("PalmTree", new Vector3(0.64f, quadPos.y, 2f), "Sunset/Palm_0");
        palm_0.sortingOrder = 10000;
        palmInitPos = palm_0.transform.position;

        AddToGeneratedObjectsDict(palm_0.gameObject.name, palm_0.gameObject);

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
        Steer(moveInput);
        MoveGrid();        
        MovePalmTrees();
        base.DoUpdate();
    }

    private void MoveGrid()
    {
        gridOffset.y += Time.deltaTime;
        gridMaterial.SetTextureOffset("_BaseMap", gridOffset);
    }

    private void MovePalmTrees()
    {
        if (palm_0.transform.position.z > -1f)
        {
            palm_0.transform.position = new Vector3(palmInitPos.x + mountainsOffsetX * 0.01f, palm_0.transform.position.y, palm_0.transform.position.z - Time.deltaTime);
        }
        else
            palm_0.transform.position = palmInitPos;
    }

    private void Steer(Vector2 input)
    {
        float currentSteer = input.x * steerSpeed * Time.deltaTime;

        mountainsOffsetX += currentSteer * .5f;
        gridOffset.x += currentSteer;

        mountainRenderer.material.SetVector("_Offset", new Vector4(mountainsOffsetX, 0, 0, 0));
    }
}

public class PalmTree
{
    public Vector3 InitPosition;
}
