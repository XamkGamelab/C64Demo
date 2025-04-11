using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using System;

public class DemoeffectNoise : DemoEffectBase
{
    private TMP_Text txt;
    private TMP_Text txt2;
    private Image img;
    private Image noiseImage;

    private float startTime;
    
    private bool inputActive = false;
    private bool inputOnCooldown = false;

    //40 x 25 images represent C64 1000 bytes "Screen ram" characters (equal to petscii square char)
    (int width, int height) noiseSize = (45, 25);

    private Texture2D noiseTexture;
    private Color[] noisePixels;


    private float offset = 0f;

    public override DemoEffectBase Init(float parTime, string tutorialText)
    {
        //Init texture and arr for noise images        
        noiseTexture = new Texture2D(noiseSize.width, noiseSize.height);
        noiseTexture.filterMode = FilterMode.Point;
        noisePixels = new Color[noiseSize.width * noiseSize.height];

        RectTransform rect = ApplicationController.Instance.UI.CreateRectTransformObject("Image_bg", new Vector2(320f, 200f), new Vector3(0, 0, 0), Vector2.zero, Vector2.one, new Vector2(8, 8), new Vector2(-8, -8));
        rect.SetAsFirstSibling();
        img = rect.AddComponent<Image>();
        img.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("white_32x32"));
        img.color = ApplicationController.Instance.C64PaletteArr[14];
        AddToGeneratedObjectsDict(rect.gameObject.name, rect.gameObject);

        RectTransform rect2 = ApplicationController.Instance.UI.CreateRectTransformObject("Text_intro", new Vector2(320f, 200f), new Vector3(0, 0, 0), Vector2.one * .5f, Vector2.one * .5f);        
        txt = TextFunctions.AddTextMeshProTextComponent(rect2, "C64_Pro_Mono-STYLE", 8, ApplicationController.Instance.C64PaletteArr[13]);
        AddToGeneratedObjectsDict(rect2.gameObject.name, rect2.gameObject);

        txt2 = GameObject.Instantiate(txt.gameObject).GetComponent<TMP_Text>();
        txt2.gameObject.name = "Text_snake";
        txt2.alignment = TextAlignmentOptions.BottomJustified;
        ApplicationController.Instance.UI.ParentTransformToUI(txt2.transform, img.transform);
        txt2.gameObject.SetActive(false);
        AddToGeneratedObjectsDict(txt2.gameObject.name, txt2.gameObject);

        //Instantiate noise image        
        RectTransform noiseImageRect = ApplicationController.Instance.UI.CreateRectTransformObject("Image_noise", new Vector2(320f, 200f), new Vector3(0, 0, 0), Vector2.one * .5f, Vector2.one * .5f);
        noiseImageRect.pivot = new Vector2(0.5f, 0.5f);
        noiseImageRect.SetAsLastSibling();
        noiseImage = noiseImageRect.AddComponent<Image>();
        
        AddToGeneratedObjectsDict(noiseImageRect.gameObject.name, noiseImageRect.gameObject);

        for (int y = 0; y < noiseSize.height; y++)
        {
            for (int x = 0; x < noiseSize.width; x++)
            {
                float xCoord = offset + (float)x / noiseSize.width * (MathFunctions.GetSin(Time.time, 0.4f, 1f) + 2f); //x, width, scale
                float yCoord = offset + (float)y / noiseSize.height * (MathFunctions.GetSin(Time.time, 0.4f, 1f) + 2f);

                offset += 0.001f * Time.deltaTime;

                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                noisePixels[y * noiseSize.width + x] = ApplicationController.Instance.C64PaletteArr[Mathf.FloorToInt(Mathf.Clamp01(sample) * 15f)];
            }
        }

        noiseTexture.SetPixels(noisePixels);
        noiseTexture.Apply();

        noiseImage.sprite = SpriteFromTexture(noiseTexture);

        return base.Init(parTime, tutorialText);
    }

    private Sprite SpriteFromTexture(Texture2D texture2D)
    {
        return Sprite.Create(texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100.0f);
    }
    private void HandleFireInput(bool b)
    {
        if (!FirePressed && b && !inputOnCooldown)
        {
        }
        FirePressed = b;
    }

    public override IEnumerator Run(System.Action callbackEnd)
    {
        yield return base.Run(callbackEnd);

        Camera.main.backgroundColor = ApplicationController.Instance.C64PaletteArr[13];

        //Disable input and reset all values
        inputActive = false;
        
        inputOnCooldown = false;
        

        AudioController.Instance.PlayTrack("Intro");

        GeneratedObjectsSetActive(true);

        ExecuteInUpdate = true;

        

        yield return new WaitForSeconds(1f);
        ApplicationController.Instance.FadeImageInOut(.2f, ApplicationController.Instance.C64PaletteArr[1], null, () => 
        {
            startTime = Time.time;
            ExecuteInUpdate = true;
            AudioController.Instance.PlayTrack("Track1");
        });

        yield return UpdateNoise();

        txt2.gameObject.SetActive(true);

        

        /*
        ApplicationController.Instance.FadeImageInOut(1f, ApplicationController.Instance.C64PaletteArr[0], () =>
        {
            base.End(true);
        }, null);
        */
    }

    
    private IEnumerator UpdateNoise()
    {
        while (true)
        {
            yield return new WaitForSeconds(.05f);

            for (int y = 0; y < noiseSize.height; y++)
            {
                for (int x = 0; x < noiseSize.width; x++)
                {
                    float xCoord = offset + (float)x / noiseSize.width * (MathFunctions.GetSin(Time.time, 1f, 1f) + 2f); //x, width, scale
                    float yCoord = offset + (float)y / noiseSize.height * (MathFunctions.GetSin(Time.time, 1f, 1f) + 2f);

                    offset = (MathFunctions.GetSin(Time.time, 3f, 1f) + 2f);

                    float sample = Mathf.PerlinNoise(xCoord, yCoord);

                    noisePixels[y * noiseSize.width + x] = ApplicationController.Instance.C64PaletteArr[Mathf.FloorToInt(Mathf.Clamp01(sample) * 15f)];
                }
            }

            noiseTexture.SetPixels(noisePixels);
            noiseTexture.Apply();

            noiseImage.sprite = SpriteFromTexture(noiseTexture);
        }
    }
    
    public override void DoUpdate()
    {
        TextFunctions.TextMeshEffect(txt, startTime, new TextEffectSettings { EffectType = TextEffectSettings.TextEffectType.Explode, ExplosionPoint = new Vector3(0, 30f, 0), ExplosionSpeed = 50f });

        

        base.DoUpdate();
    }
}