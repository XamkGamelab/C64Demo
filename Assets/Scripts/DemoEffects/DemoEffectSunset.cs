using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using System.Linq;

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

    private Dictionary<GameObject, Vector3> palms = new Dictionary<GameObject, Vector3>();
    private (float MinZ, float MaxZ) palmDistance = (-1f, 2f);
    private Vector3 sunInitPos;

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
        sunRenderer.sortingOrder = 1;
        sunInitPos = sunRenderer.transform.position;
        AddToGeneratedObjectsDict(sunRenderer.gameObject.name, sunRenderer.gameObject);

        mountainRenderer = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("Mountains", new Vector3(0, -0.16f, 1f), "Sunset/SunsetMountains");
        mountainRenderer.sortingOrder = 2;
        mountainRenderer.material = spriteScrollMaterial;
        AddToGeneratedObjectsDict(mountainRenderer.gameObject.name, mountainRenderer.gameObject);

        imgLogoTxtSS = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("LogoTxtSS", new Vector3(0.8f, 0.8f, 1.2f), "Sunset/SunsetTextSS");
        imgLogoTxtSS.sortingOrder = 3;
        AddToGeneratedObjectsDict(imgLogoTxtSS.gameObject.name, imgLogoTxtSS.gameObject);

        imgLogoTxtOverride = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("LogoTxtOverride", new Vector3(0.8f, 0.72f, 1.2f), "Sunset/SunsetTextOverride");
        imgLogoTxtOverride.sortingOrder = 4;
        AddToGeneratedObjectsDict(imgLogoTxtOverride.gameObject.name, imgLogoTxtOverride.gameObject);

        InstantiatePalms(5, -2.56f, 1.28f, true);

        return base.Init();
    }

    public override IEnumerator Run(Action endDemoCallback)
    {
        yield return base.Run(endDemoCallback);

        Camera.main.backgroundColor = ApplicationController.Instance.C64PaletteArr[0];
        CameraFunctions.SetCameraSettings(Camera.main, ApplicationController.Instance.CameraSettings["perspectiveFov90"]);

        AudioController.Instance.PlayTrack("BattleIntro");

        //Enable all generated objects        
        GeneratedObjectsSetActive(true);

        ExecuteInUpdate = true;

        //Subscribe to input        
        InputController.Instance.Horizontal.Subscribe(f => moveInput.x = f).AddTo(Disposables);

        //Start animating logo tweens
        imgLogoTxtSS.transform.DOLocalMoveY(0.96f, 1f, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        imgLogoTxtOverride.transform.DOLocalMoveY(0.64f, 1f, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    public override void DoUpdate()
    {
        Steer(moveInput);
        MoveGrid();        
        MovePalmTrees();
        MoveSun();
        base.DoUpdate();
    }

    private void InstantiatePalms(int count, float startPos, float step, bool skipCenterPalm)
    {
        int center = (int)(count * .5f);
        float xPos = startPos;
        for (int i = 0; i < count; i++)
        {
            if (skipCenterPalm && i == center)
                continue;

            float palmPosZ = Mathf.Clamp(UnityEngine.Random.Range(palmDistance.MinZ, palmDistance.MaxZ), palmDistance.MinZ, palmDistance.MaxZ);
            SpriteRenderer palm = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("PalmTree_" + i, new Vector3(xPos + i * step, quadPos.y, palmPosZ), "Sunset/Palm_0");
            palm.sortingOrder = 1000 + i;            
            palms.Add(palm.gameObject, new Vector3(palm.transform.position.x, palm.transform.position.y, palmDistance.MaxZ));             
            AddToGeneratedObjectsDict(palm.gameObject.name, palm.gameObject);
        }
    }

    private void MoveGrid()
    {
        gridOffset.y += Time.deltaTime;
        gridMaterial.SetTextureOffset("_BaseMap", gridOffset);
    }

    private void MoveSun()
    {
        sunRenderer.transform.position = new Vector3(sunInitPos.x + mountainsOffsetX * 1f, sunInitPos.y, sunInitPos.z);
    }

    private void MovePalmTrees()
    {
        palms.ToList().ForEach(kvp =>
        {
            if (kvp.Key.transform.position.z > palmDistance.MinZ)            
                kvp.Key.transform.position = new Vector3(kvp.Value.x + mountainsOffsetX * 0.01f, kvp.Key.transform.position.y, kvp.Key.transform.position.z - Time.deltaTime);            
            else
                kvp.Key.transform.position = kvp.Value;
        });        
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
