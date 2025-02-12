using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
public class DemoeffectIntro : DemoEffectBase
{
    private string c64default = "\r\n\r\n\r\n     **** COMMODORE 64 BASIC V2 ****\r\n  64K RAM SYSTEM 38911 BASIC BYTES FREE\r\n\r\n READY\r\n LOAD\"*\",B,1:\r\n\r\n SEARCHING FOR *\r\n LOADING\r\n READY.\r\n RUN";
    private string[] pressSpaceStrings = new string[] { " [PRESS SPACE TO START]\n\n", " [PRESS SPACE AGAIN]\n\n", " [AGAIN!!!]\n\n", " [COME ON. PRESS THAT SPACE. TO START]\n\n", " [ONE MORE TIME]\n\n" };
    private int pressSpaceCount = 0;
    private TMP_Text txt;
    private TMP_Text txt2;    
    private Image img;        
    private float startTime;
    private bool firePressed = false;
    private bool inputActive = false;
    private bool inputOnCooldown = false;
    private bool loopSnake = true;

    public override DemoEffectBase Init()
    {
        loopSnake = true;

        RectTransform rect = ApplicationController.Instance.UI.CreateRectTransformObject("Image_bg", new Vector2(320, 200), new Vector3(0, 0, 0), Vector2.zero, Vector2.one, new Vector2(24, 8), new Vector2(-24, -8));
        rect.SetAsFirstSibling();
        img = rect.AddComponent<Image>();
        img.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("white_32x32"));
        img.color = ApplicationController.Instance.C64PaletteArr[14];
        AddToGeneratedObjectsDict(rect.gameObject.name, rect.gameObject);

        RectTransform rect2 = ApplicationController.Instance.UI.CreateRectTransformObject("Text_intro", new Vector2(320, 200), new Vector3(0, 0, 0), Vector2.one * .5f, Vector2.one * .5f);        
        txt = TextFunctions.AddTextMeshProTextComponent(rect2, "C64_Pro_Mono-STYLE", 8, ApplicationController.Instance.C64PaletteArr[13]);
        AddToGeneratedObjectsDict(rect2.gameObject.name, rect2.gameObject);

        txt2 = Object.Instantiate(txt.gameObject).GetComponent<TMP_Text>();
        txt2.gameObject.name = "Text_snake";
        txt2.alignment = TextAlignmentOptions.BottomJustified;
        ApplicationController.Instance.UI.ParentTransformToUI(txt2.transform, img.transform);
        txt2.gameObject.SetActive(false);
        AddToGeneratedObjectsDict(txt2.gameObject.name, txt2.gameObject);

        return base.Init();
    }

    private void HandleFireInput(bool b)
    {
        if (!firePressed && b && !inputOnCooldown)
        {
            if (pressSpaceCount < pressSpaceStrings.Length - 1)
            {
                //Shake when space pressed
                inputOnCooldown = true;
                img.transform.DOShakePosition(1.0f, strength: new Vector3(0, 20f, 0), vibrato: 5, randomness: 1, snapping: false, fadeOut: true).OnComplete(() => 
                {
                    inputOnCooldown = false;
                });
                pressSpaceCount++;
            }
            else
            {
                ApplicationController.Instance.FadeImageInOut(1f, ApplicationController.Instance.C64PaletteArr[0], () =>
                {
                    //End the demo by exiting last coroutine and calling base.End();
                    loopSnake = false;
                    base.End();
                }, null);
            }
        }
        firePressed = b;        
    }

    public override IEnumerator Run(System.Action callbackEnd)
    {
        yield return base.Run(callbackEnd);

        //Disable input
        inputActive = false;

        img.gameObject.SetActive(true);
        txt.gameObject.SetActive(true);
        yield return AnimateStartText();
        
        yield return new WaitForSeconds(1f);
        ApplicationController.Instance.FadeImageInOut(.2f, ApplicationController.Instance.C64PaletteArr[1], null, () => 
        {
            startTime = Time.time;
            ExecuteInUpdate = true;
            AudioController.Instance.PlayTrack("Track1");
        });

        txt2.gameObject.SetActive(true);
        yield return AnimateLoadingSnake();
    }

    public override void DoUpdate()
    {
        TextFunctions.ExplodeTextMesh(txt, startTime, new Vector3(0, 30f, 0), 50f);
        base.DoUpdate();
    }

    IEnumerator AnimateStartText()
    {
        string c64typewriter = "";
        for (int i = 0; i < c64default.Length; i++)
        {
            c64typewriter += c64default[i];
            txt.text = c64typewriter;
            yield return new WaitForSeconds(Random.Range(0.01f, 0.01f));
        }
    }

    IEnumerator AnimateLoadingSnake()
    {
        List<string> spriteStrings = TextureAndGaphicsFunctions.LoadSpriteSheet("IntroSnakeSpriteSheetPSD").Select(s => TextureAndGaphicsFunctions.ConvertToAscii(s.texture, s.textureRect)).ToList();

        int letterCount = spriteStrings[0].Length;
        int showLettersCount = 40;
        int imgIndex = 0;
        string animatedString = "";
        while (loopSnake)
        {
            animatedString = spriteStrings[imgIndex].Substring(0, showLettersCount);
            string replacement = inputOnCooldown? " ...\n\n" : pressSpaceStrings[pressSpaceCount];

            if (showLettersCount >= letterCount)
            {
                animatedString.Remove(spriteStrings[imgIndex].Length - replacement.Length, replacement.Length);
                animatedString += replacement;

                if (!inputActive)
                {
                    //Start playable part of demo and subscribe to input
                    InputController.Instance.Fire1.Subscribe(b => HandleFireInput(b)).AddTo(Disposables);
                    pressSpaceCount = 0;
                    inputActive = true;
                }
            }

            txt2.text = animatedString; //spriteStrings[imgIndex].Substring(0, showLettersCount);
            Camera.main.backgroundColor = ApplicationController.Instance.C64PaletteArr[Random.Range(0, ApplicationController.Instance.C64PaletteArr.Length)];
            yield return new WaitForSeconds(0.1f);
            imgIndex = imgIndex < spriteStrings.Count - 1 ? ++imgIndex : 0;
            showLettersCount = Mathf.Clamp(showLettersCount+=40, 0, letterCount);
        }
    }
}
