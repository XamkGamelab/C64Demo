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
using static UnityEngine.GraphicsBuffer;
using System.Threading.Tasks;
using UnityEditor.Search;

public class DemoeffectNoise : DemoEffectBase
{
    private TMP_Text txt;
    private TMP_Text txtClone;
    private TMP_Text txtPressFirePrompt;

    private Image img;
    private Image noiseImage;
    private Image TheEnd_the;
    private Image TheEnd_end;
    private Image TheEnd_heart;

    //40 x 25 images represent C64 1000 bytes "Screen ram" characters (equal to petscii square char)
    (int width, int height) noiseSize = (45, 25);

    private Texture2D noiseTexture;
    private Color[] noisePixels;
    private Color[] gradient = new Color[]
    {
        ApplicationController.Instance.C64PaletteArr[1],
        ApplicationController.Instance.C64PaletteArr[9],
        ApplicationController.Instance.C64PaletteArr[10],
        ApplicationController.Instance.C64PaletteArr[12],
        ApplicationController.Instance.C64PaletteArr[13],
        ApplicationController.Instance.C64PaletteArr[15],
        ApplicationController.Instance.C64PaletteArr[6],
        ApplicationController.Instance.C64PaletteArr[9],
    };

    private string[] credits = new string[]
    {
        "[ programming and\ngraphics ]\np3v1",
        "[ music jing3 ]\nSpring Spring",
        "[ music C64 Level 1 ]\ntcarisland",
        "[ music C64_action_loop ]\n©2017 Emma Andersson\n(Emma_MA)",
        "[ music Battle Intro ]\ncelestialghost8",
        "[ music\nC64_uptempo_chiptune ]\nSkrjablin",
        "[ 3D C64 model ]\ndark_igorek",
        "[ 3D book models ]\nspookyghostboo",
        "[ 3D joystick model ]\ncontraryk",
    };


    private Queue<string> creditsQueue = new Queue<string>();
    private string currentCredits;

    private Vector3 theInitPos;
    private Vector3 endInitPos;
    private Vector3 heartInitPos;
    private Vector3 txtInitPos;
    private float currentAngle = 0f;
    private float orbitSpeed = 180f;
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

        //Instantiate the end images
        RectTransform endRect1 = ApplicationController.Instance.UI.CreateRectTransformObject("Image_the", new Vector2(92f, 59f), new Vector3(-84f, 40f, 0), Vector2.one * .5f, Vector2.one * .5f);
        endRect1.pivot = new Vector2(0.5f, 0.5f);
        endRect1.SetAsLastSibling();
        Image shadowThe = endRect1.AddComponent<Image>();
        shadowThe.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("TheEnd_the"));
        AddToGeneratedObjectsDict(endRect1.gameObject.name, endRect1.gameObject);

        RectTransform endRect3 = ApplicationController.Instance.UI.CreateRectTransformObject("Image_heart", new Vector2(68f, 57f), new Vector3(0f, 40f, 0), Vector2.one * .5f, Vector2.one * .5f);
        endRect3.pivot = new Vector2(0.5f, 0.5f);
        endRect3.SetAsLastSibling();
        Image shadowHeart = endRect3.AddComponent<Image>();
        shadowHeart.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("TheEnd_heart"));
        AddToGeneratedObjectsDict(endRect3.gameObject.name, endRect3.gameObject);

        RectTransform endRect2 = ApplicationController.Instance.UI.CreateRectTransformObject("Image_end", new Vector2(92f, 59f), new Vector3(84f, 40f, 0), Vector2.one * .5f, Vector2.one * .5f);
        endRect2.pivot = new Vector2(0.5f, 0.5f);
        endRect2.SetAsLastSibling();
        Image shadowEnd = endRect2.AddComponent<Image>();
        shadowEnd.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("TheEnd_end"));
        AddToGeneratedObjectsDict(endRect2.gameObject.name, endRect2.gameObject);

        TheEnd_the = GameObject.Instantiate(endRect1.gameObject, endRect1).GetComponent<Image>();
        TheEnd_the.rectTransform.localPosition = new Vector3(-4f, 4f, 0f);
        AddToGeneratedObjectsDict(TheEnd_the.gameObject.name, TheEnd_the.gameObject);

        TheEnd_end = GameObject.Instantiate(endRect2.gameObject, endRect2).GetComponent<Image>();
        TheEnd_end.rectTransform.localPosition = new Vector3(-4f, 4f, 0f);
        AddToGeneratedObjectsDict(TheEnd_end.gameObject.name, TheEnd_end.gameObject);

        TheEnd_heart = GameObject.Instantiate(endRect3.gameObject, endRect3).GetComponent<Image>();
        TheEnd_heart.rectTransform.localPosition = new Vector3(-4f, 4f, 0f);
        AddToGeneratedObjectsDict(TheEnd_heart.gameObject.name, TheEnd_heart.gameObject);

        theInitPos = endRect1.anchoredPosition3D;
        endInitPos = endRect2.anchoredPosition3D;
        heartInitPos = endRect3.anchoredPosition3D;



        //Credits text
        RectTransform txtRect = ApplicationController.Instance.UI.CreateRectTransformObject("Text_intro", new Vector2(320f, 26f), new Vector3(0, -40f, 0), Vector2.one * .5f, Vector2.one * .5f);
        txtRect.pivot = new Vector2(0.5f, 0.5f);
        txt = TextFunctions.AddTextMeshProTextComponent(txtRect, "8-BIT_WONDER", 12, ApplicationController.Instance.C64PaletteArr[1]);
        txt.alignment = TextAlignmentOptions.Center;
        AddToGeneratedObjectsDict(txtRect.gameObject.name, txtRect.gameObject);

        txtClone = GameObject.Instantiate(txtRect.gameObject, txtRect).GetComponent<TMP_Text>();
        txtClone.rectTransform.localPosition = new Vector3(-4f, 4f, 0f);
        AddToGeneratedObjectsDict(txtClone.gameObject.name, txtClone.gameObject);

        //Shadow color for all shadow clones
        txt.color = shadowHeart.color = shadowEnd.color = shadowThe.color = new Color(0f, 0f, 0f, 0.9f);

        //
        RectTransform txtFirePromptRect = ApplicationController.Instance.UI.CreateRectTransformObject("Text_fire_prompt", new Vector2(320f, 16f), new Vector3(0, 8f, 0), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
        txtFirePromptRect.pivot = new Vector2(0.5f, 0f);
        txtPressFirePrompt = TextFunctions.AddTextMeshProTextComponent(txtFirePromptRect, "C64_Pro_Mono-STYLE", 8, ApplicationController.Instance.C64PaletteArr[1]);
        txtPressFirePrompt.alignment = TextAlignmentOptions.Center;
        txtPressFirePrompt.text = "[PRESS FIRE TO QUIT]";
        AddToGeneratedObjectsDict(txtFirePromptRect.gameObject.name, txtFirePromptRect.gameObject);

        return base.Init(parTime, tutorialText);
    }

    private Sprite SpriteFromTexture(Texture2D texture2D)
    {
        return Sprite.Create(texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100.0f);
    }

    public override IEnumerator Run(System.Action callbackEnd)
    {
        yield return base.Run(callbackEnd);

        //Enqueue the credits and init first        
        for (int i = 0; i < credits.Length; i++)
            creditsQueue.Enqueue(credits[i]);

        Camera.main.backgroundColor = ApplicationController.Instance.C64PaletteArr[0];

        AudioController.Instance.FadeOutMusic(2f);
        //Hold on, hold on...
        yield return new WaitForSeconds(2f);
        AudioController.Instance.FadeInMusic(.1f);
        AudioController.Instance.PlayTrack("Credits");

        //GO! Flash white and start running stuff
        ApplicationController.Instance.FadeImageInOut(1f, ApplicationController.Instance.C64PaletteArr[1], () =>
        {
            //Enable everything but "press fire" prompt
            GeneratedObjectsSetActive(true, new List<string> { "Text_fire_prompt" });

            RotateCreditsTexts();

            ExecuteInUpdate = true;


            //Delay and start fading out
            Task.Delay(3000).ContinueWith(_ =>
            {
                ApplicationController.Instance.FadeImageInOut(.3f, ApplicationController.Instance.C64PaletteArr[1], null, null);

                txtPressFirePrompt.gameObject.SetActive(true);

                InputController.Instance.Fire1.Subscribe(b =>
                {
                    //Dispose input subscription and end demo when fire is pressed
                    if (b)
                    {
                        Disposables?.Dispose();
                        EndDemo();
                    }
                }).AddTo(Disposables);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }, null);

        yield return UpdateNoise();
    }

    public override void DoUpdate()
    {
        // Update the angle based on speed and time
        currentAngle += orbitSpeed * Time.deltaTime;
        currentAngle %= 360f; // Keep the angle between 0-360

        TheEnd_the.rectTransform.parent.transform.localPosition = MathFunctions.RotateAroundPoint(theInitPos, currentAngle, 4f, Vector3.forward);
        TheEnd_heart.rectTransform.parent.transform.localPosition = MathFunctions.RotateAroundPoint(heartInitPos, -currentAngle, 6f, -Vector3.forward);
        TheEnd_end.rectTransform.parent.transform.localPosition = MathFunctions.RotateAroundPoint(endInitPos, -currentAngle, 4f, Vector3.forward);

        base.DoUpdate();
    }

    private void RotateCreditsTexts()
    {
        if (creditsQueue.Count > 0)
        {
            currentCredits = creditsQueue.Dequeue();
            txt.text = txtClone.text = currentCredits;
            txt.transform.rotation = Quaternion.Euler(new Vector3(-90f, 0f, 0f));
            txt.transform.DORotate(Vector3.zero, 1f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                //Show credits a while, then rotate it away and run next credits
                txt.transform.DORotate(new Vector3(90f, 0f, 0f), 1f).SetDelay(2f).SetEase(Ease.InSine).OnComplete(() =>
                {
                    RotateCreditsTexts();
                });
            });
        }
        else
            EndDemo();
    }

    private void EndDemo()
    {
        //TODO: Because there's option to end credits by pressing space (delay input subscription like 3 seconds, so that it's not accidently pressed)
        DOTween.Kill(txt.transform);

        ApplicationController.Instance.FadeImageInOut(1f, ApplicationController.Instance.C64PaletteArr[0], () =>
        {
            base.End(true);
        }, null);
    }

    private IEnumerator UpdateNoise()
    {
        float arrayOffset = 0;

        while (true)
        {
            yield return new WaitForSeconds(.05f);

            gradient = ShiftArray<Color>(gradient, 1);

            for (int y = 0; y < noiseSize.height; y++)
            {
                for (int x = 0; x < noiseSize.width; x++)
                {
                    float xCoord = offset + (float)x / noiseSize.width * (MathFunctions.GetSin(Time.time, 1f, 1f) + 2f); //x, width, scale
                    float yCoord = offset + (float)y / noiseSize.height * (MathFunctions.GetSin(Time.time, 1f, 1f) + 2f);
                    offset = arrayOffset; // (MathFunctions.GetSin(Time.time, 3f, 1f) + 2f);
                    float sample = Mathf.PerlinNoise(xCoord, yCoord);
                    noisePixels[y * noiseSize.width + x] = gradient[Mathf.FloorToInt(Mathf.Clamp01(sample) * (gradient.Length - 1))];
                }
            }

            arrayOffset += Time.deltaTime * 15f;

            noiseTexture.SetPixels(noisePixels);
            noiseTexture.Apply();

            noiseImage.sprite = SpriteFromTexture(noiseTexture);
        }
    }


    private static T[] ShiftArray<T>(T[] array, int offset)
    {
        T[] result = new T[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            result[(i + offset) % array.Length] = array[i];
        }
        return result;
    }
}