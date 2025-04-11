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
    
    private float startTime;
    
    private bool inputActive = false;
    private bool inputOnCooldown = false;

    //40 x 25 images represent C64 1000 bytes "Screen ram" characters (equal to petscii square char)
    private Image[,] characterGrid = new Image[40,25];

    public override DemoEffectBase Init(float parTime, string tutorialText)
    {
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

        //Instantiate characters to 40 x 25 character grid
        for (int y = 0; y < characterGrid.GetLength(1); y++)
        {
            for (int x = 0; x < characterGrid.GetLength(0); x++)
            {
                float xCoord = x / 40f * 2f; //x, width, scale
                float yCoord = y / 25f * 2f;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                Color color = new Color(sample, sample, sample, 1f);

                int index = Mathf.FloorToInt(sample * 16f);
                Debug.Log(index);
                RectTransform charRect = ApplicationController.Instance.UI.CreateRectTransformObject("Image_char_" + x + "_" + y, new Vector2(8f, 8f), new Vector3(x * 8f, y * 8f , 0), Vector2.zero, Vector2.zero, Vector2.zero);
                charRect.pivot = new Vector2(0f, 0f);
                charRect.sizeDelta = new Vector2(8f, 8f);
                charRect.anchoredPosition3D = new Vector3(x * 8f, y * 8f, 0);

                charRect.SetAsLastSibling();
                img = charRect.AddComponent<Image>();
                img.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("white_32x32"));
                img.color = ApplicationController.Instance.C64PaletteArr[Mathf.FloorToInt(sample * 16f)];
                AddToGeneratedObjectsDict(charRect.gameObject.name, charRect.gameObject);
            }
        }

        return base.Init(parTime, tutorialText);
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
        
        yield return new WaitForSeconds(1f);
        ApplicationController.Instance.FadeImageInOut(.2f, ApplicationController.Instance.C64PaletteArr[1], null, () => 
        {
            startTime = Time.time;
            ExecuteInUpdate = true;
            AudioController.Instance.PlayTrack("Track1");
        });

        txt2.gameObject.SetActive(true);
        
        /*
        ApplicationController.Instance.FadeImageInOut(1f, ApplicationController.Instance.C64PaletteArr[0], () =>
        {
            base.End(true);
        }, null);
        */
    }

    public override void DoUpdate()
    {
        TextFunctions.TextMeshEffect(txt, startTime, new TextEffectSettings { EffectType = TextEffectSettings.TextEffectType.Explode, ExplosionPoint = new Vector3(0, 30f, 0), ExplosionSpeed = 50f });
        base.DoUpdate();
    }




}