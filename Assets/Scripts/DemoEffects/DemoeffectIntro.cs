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

    private TMP_Text txt;
    private TMP_Text txt2;
    private Image img;
    private Image imgGirl;
    private Image imgGirlMouth;
    private Image imgGirlSpeech1;
    private Image imgGirlSpeech2;

    private RectTransform rectGirl;

    private int pressSpaceCount = 0;         
    private float startTime;
    
    private bool inputActive = false;
    private bool inputOnCooldown = false;
    private bool loopSnake = true;

    private List<Sprite> girlMouthSprites => TextureAndGaphicsFunctions.LoadSpriteSheet("GirlMouthSheet");

    public override DemoEffectBase Init()
    {
        loopSnake = true;

        RectTransform rect = ApplicationController.Instance.UI.CreateRectTransformObject("Image_bg", new Vector2(320f, 200f), new Vector3(0, 0, 0), Vector2.zero, Vector2.one, new Vector2(8, 8), new Vector2(-8, -8));
        rect.SetAsFirstSibling();
        img = rect.AddComponent<Image>();
        img.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("white_32x32"));
        img.color = ApplicationController.Instance.C64PaletteArr[14];
        AddToGeneratedObjectsDict(rect.gameObject.name, rect.gameObject);

        RectTransform rect2 = ApplicationController.Instance.UI.CreateRectTransformObject("Text_intro", new Vector2(320f, 200f), new Vector3(0, 0, 0), Vector2.one * .5f, Vector2.one * .5f);        
        txt = TextFunctions.AddTextMeshProTextComponent(rect2, "C64_Pro_Mono-STYLE", 8, ApplicationController.Instance.C64PaletteArr[13]);
        AddToGeneratedObjectsDict(rect2.gameObject.name, rect2.gameObject);

        txt2 = Object.Instantiate(txt.gameObject).GetComponent<TMP_Text>();
        txt2.gameObject.name = "Text_snake";
        txt2.alignment = TextAlignmentOptions.BottomJustified;
        ApplicationController.Instance.UI.ParentTransformToUI(txt2.transform, img.transform);
        txt2.gameObject.SetActive(false);
        AddToGeneratedObjectsDict(txt2.gameObject.name, txt2.gameObject);

        rectGirl = ApplicationController.Instance.UI.CreateRectTransformObject("Image_girl", new Vector2(320f, 400f), new Vector2(0f, -400f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
        rectGirl.pivot = new Vector2(0.5f, 0f);
        imgGirl = rectGirl.AddComponent<Image>();        
        imgGirl.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("Images/GirlYellow"));
        AddToGeneratedObjectsDict(rectGirl.gameObject.name, rectGirl.gameObject);

        RectTransform rectGirlMouth = ApplicationController.Instance.UI.CreateRectTransformObject("Image_girl_mouth", new Vector2(64f, 64f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f));
        rectGirlMouth.pivot = new Vector2(0f, 0f);
        rectGirlMouth.SetParent(rectGirl);
        rectGirlMouth.localScale = Vector3.one;
        rectGirlMouth.anchoredPosition3D = new Vector3(135f, 102f, 0f);
        imgGirlMouth = rectGirlMouth.AddComponent<Image>();
        imgGirlMouth.sprite = girlMouthSprites[0];

        RectTransform rectSpeech1 = ApplicationController.Instance.UI.CreateRectTransformObject("Image_girl_speech_1", new Vector2(70f, 26f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f));
        rectSpeech1.pivot = new Vector2(0f, 0f);
        rectSpeech1.SetParent(rectGirl);
        rectSpeech1.localScale = Vector3.one;
        rectSpeech1.anchoredPosition3D = new Vector3(192f, 136f, 0f);
        imgGirlSpeech1 = rectSpeech1.AddComponent<Image>();
        imgGirlSpeech1.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("GirlSpeechBubbleNow"));
        imgGirlSpeech1.gameObject.SetActive(false);

        RectTransform rectSpeech2 = ApplicationController.Instance.UI.CreateRectTransformObject("Image_girl_speech_2", new Vector2(70f, 26f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f));
        rectSpeech2.pivot = new Vector2(0f, 0f);
        rectSpeech2.SetParent(rectGirl);
        rectSpeech2.localScale = Vector3.one;
        rectSpeech2.anchoredPosition3D = new Vector3(208f, 144f, 0f);
        imgGirlSpeech2 = rectSpeech2.AddComponent<Image>();
        imgGirlSpeech2.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("GirlSpeechBubbleRun"));
        imgGirlSpeech2.gameObject.SetActive(false);

        return base.Init();
    }

    private void HandleFireInput(bool b)
    {
        if (!FirePressed && b && !inputOnCooldown)
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
                //Dispose input, rest is just demo ending
                Disposables?.Dispose();
                rectGirl.gameObject.SetActive(true);
                rectGirl.DOAnchorPos3DY(0f, 2f, true).SetEase(Ease.OutQuint).OnComplete(() => 
                {
                    //Start speech animation in RUN by stopping snake anim
                    loopSnake = false;
                });                
            }
        }
        FirePressed = b;        
    }

    public override IEnumerator Run(System.Action callbackEnd)
    {
        yield return base.Run(callbackEnd);

        //Disable input and reset all values
        inputActive = false;
        pressSpaceCount = 0;
        loopSnake = true;
        inputOnCooldown = false;

        AudioController.Instance.PlayTrack("Intro");

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
        yield return GirlAnimation();

        ApplicationController.Instance.FadeImageInOut(1f, ApplicationController.Instance.C64PaletteArr[0], () =>
        {
            base.End(true);
        }, null);
    }

    public override void DoUpdate()
    {
        TextFunctions.TextMeshEffect(txt, startTime, new TextEffectSettings { EffectType = TextEffectSettings.TextEffectType.Explode, ExplosionPoint = new Vector3(0, 30f, 0), ExplosionSpeed = 50f });
        base.DoUpdate();
    }

    private IEnumerator GirlAnimation()
    {
        Debug.Log("GIRL ANIMATION");
        imgGirlSpeech1.gameObject.SetActive(true);
        float startTime = Time.time;
        bool secondBubbleActive = false;
        while (Time.time - startTime < 6f)
        {
            imgGirlMouth.sprite = girlMouthSprites[UnityEngine.Random.Range(0, girlMouthSprites.Count)];
            //Like if time - start > 2f --> then show the other bubble...
            if (!secondBubbleActive && Time.time - startTime > 3f)
            {
                secondBubbleActive = true;
                imgGirlSpeech1.gameObject.SetActive(false);
                imgGirlSpeech2.gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(0.1f);
        }
        imgGirlMouth.sprite = girlMouthSprites[0];
    }
    private IEnumerator AnimateStartText()
    {
        string c64typewriter = "";
        for (int i = 0; i < c64default.Length; i++)
        {
            c64typewriter += c64default[i];
            txt.text = c64typewriter;
            yield return new WaitForSeconds(Random.Range(0.01f, 0.01f));
        }
    }

    private IEnumerator AnimateLoadingSnake()
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
