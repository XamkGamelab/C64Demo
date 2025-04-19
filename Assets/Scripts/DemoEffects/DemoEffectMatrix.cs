using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Linq;
using Unity.VisualScripting;
using UniRx;
using System.Threading.Tasks;

public class DemoEffectMatrix : DemoEffectBase
{
    private TMP_Text txt;
    private List<MatrixText> matrixTexts = new List<MatrixText>();

    private Image handRed;
    private Image handBlue;
    private Image catcherHand;
    private Image enterMatrixText;
    private Image enterMatrixTextLit;

    RectTransform rectCatcher;

    //Gameplay
    private Vector3 enterMatrixTextInitPosition = new Vector3(0, -20f, 0);
    private Vector3 enterMatrixTextHiddenPosition = new Vector3(0, 60f, 0);
    private Vector2 moveInput = Vector2.zero;
    private float handSpeed = 120f;
    private Rect collectableCharacterRect;
    private Queue<char> collectTextQueue = new Queue<char>();
    private char currentCollectChar;
    private bool collectOnCoolDown = false;
    private bool allCharactersCollected = false;
    private const float characterSize = 8;
    private const string collectText = "ENTERTHEMATRIX";

    private List<Sprite> laserSprites;
    private List<Sprite> matrixTextSpritesLit;

    public override DemoEffectBase Init(float parTime, string tutorialText)
    {
        laserSprites = TextureAndGaphicsFunctions.LoadSpriteSheet("MatrixLaserBeam");
        matrixTextSpritesLit = TextureAndGaphicsFunctions.LoadSpriteSheet("EnterTheMatrixText_lit_spriteSheet");

        float steps = ApplicationController.Instance.UI.GetCanvasSize().Value.x / characterSize / 2f;
        for (int i = 0; i < steps; i++)
            matrixTexts.Add(new MatrixText { TmpText = InstantiateMatrixText("MatrixText_" + i, i * characterSize * 2 - ApplicationController.Instance.UI.GetCanvasSize().Value.x * .5f), Speed = UnityEngine.Random.Range(1, 3), Letters = UnityEngine.Random.Range(22, 30) });

        //One random text is collectable and slow
        var rmt = matrixTexts[UnityEngine.Random.Range(0, matrixTexts.Count)];
        rmt.Collectable = true;
        rmt.Letters = 30;
        rmt.Speed = 1;

        RectTransform rectHandRed = ApplicationController.Instance.UI.CreateRectTransformObject("Hand_red", new Vector2(128, 128), new Vector3(-100f, 0, 0), Vector2.one * .5f, Vector2.one * .5f);
        handRed = rectHandRed.AddComponent<Image>();
        handRed.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("Images/HandRedPill"));
        AddToGeneratedObjectsDict(rectHandRed.gameObject.name, rectHandRed.gameObject);

        RectTransform rectHandBlue = ApplicationController.Instance.UI.CreateRectTransformObject("Hand_blue", new Vector2(128, 128), new Vector3(100f, 0, 0), Vector2.one * .5f, Vector2.one * .5f);
        handBlue = rectHandBlue.AddComponent<Image>();
        handBlue.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("Images/HandBluePill"));
        AddToGeneratedObjectsDict(rectHandBlue.gameObject.name, rectHandBlue.gameObject);

        rectCatcher = ApplicationController.Instance.UI.CreateRectTransformObject("CatchingHand2", new Vector2(24, 21), new Vector3(0, 0, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
        rectCatcher.pivot = new Vector2(0.5f, 0f);
        catcherHand = rectCatcher.AddComponent<Image>();
        catcherHand.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("CatchingHand"));
        AddToGeneratedObjectsDict(rectCatcher.gameObject.name, rectCatcher.gameObject);

        RectTransform enterMatrixRect = ApplicationController.Instance.UI.CreateRectTransformObject("EnterTheMatrixText", new Vector2(212, 60), enterMatrixTextInitPosition, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
        enterMatrixRect.pivot = new Vector2(0.5f, 1f);
        enterMatrixText = enterMatrixRect.AddComponent<Image>();
        enterMatrixText.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("EnterTheMatrixText_eroded"));
        AddToGeneratedObjectsDict(enterMatrixRect.gameObject.name, enterMatrixRect.gameObject);

        RectTransform enterMatrixLitRect = ApplicationController.Instance.UI.CreateRectTransformObject("EnterTheMatrixTextLit", new Vector2(212, 60), enterMatrixTextInitPosition, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
        enterMatrixLitRect.pivot = new Vector2(0.5f, 1f);
        enterMatrixTextLit = enterMatrixLitRect.AddComponent<Image>();
        enterMatrixTextLit.sprite = matrixTextSpritesLit.First();
        enterMatrixTextLit.rectTransform.SetParent(enterMatrixRect, true);
        AddToGeneratedObjectsDict(enterMatrixLitRect.gameObject.name, enterMatrixLitRect.gameObject);


        return base.Init(parTime, tutorialText);
    }

    public override IEnumerator Run(Action endDemoCallback)
    {
        yield return base.Run(endDemoCallback);

        //Enqueue the text to be collected and init first character
        char[] charArray = collectText.ToCharArray();
        collectTextQueue = new Queue<char>();
        for (int i = 0; i < charArray.Length; i++)
            collectTextQueue.Enqueue(charArray[i]);
        currentCollectChar = collectTextQueue.Dequeue();
        
        //Reset all other values
        rectCatcher.anchoredPosition3D = Vector3.zero;
        enterMatrixTextLit.sprite = matrixTextSpritesLit.First();
        //Text falls to position from top of the screen
        enterMatrixText.rectTransform.anchoredPosition3D = enterMatrixTextHiddenPosition;

        matrixTexts.ForEach(mt => ResetTextFallPosition(mt));

        allCharactersCollected = false;

        ExecuteInUpdate = true;

        Camera.main.backgroundColor = ApplicationController.Instance.C64PaletteArr[0];

        AudioController.Instance.PlayTrack("Jing3");

        /*
        yield return new WaitForSeconds(4f);

        handRed.color = new Color(1f, 1f, 1f, 0f);
        handBlue.color = new Color(1f, 1f, 1f, 0f);

        handRed.gameObject.SetActive(true);
        handBlue.gameObject.SetActive(true);

        handRed.DOFade(1f, 2f).SetDelay(1f);
        handBlue.DOFade(1f, 2f).SetDelay(3f).OnComplete(() => 
        {
            handBlue.DOFade(0f, 2f);
            handRed.rectTransform.DOLocalMove(new Vector3(-75f, -50f, 0), 4f, true);
            handRed.DOFade(0, 2f).SetDelay(6f);
        });

        yield return new WaitForSeconds(12f);
        */

        //Enable all generated objects        
        GeneratedObjectsSetActive(true);

        //Animate text drop
        enterMatrixText.rectTransform.DOAnchorPos3DY(enterMatrixTextInitPosition.y, 2f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            //Start hovering text
            enterMatrixText.rectTransform.DOAnchorPos3DY(-40f, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        });

        handBlue.gameObject.SetActive(false);
        handRed.gameObject.SetActive(false);

        //Subscribe to input        
        InputController.Instance.Horizontal.Subscribe(f => moveInput.x = f).AddTo(Disposables);


        yield return AnimateMatrixTexts();
    }

    public override void DoUpdate()
    {
        MoveHand(moveInput);
        base.DoUpdate();
    }

    public override void End(bool dispose = true)
    {
        moveInput = Vector2.zero;
        base.End(dispose);
    }

    private void MoveHand(Vector2 input)
    {
        Vector3 nextPosition = rectCatcher.anchoredPosition3D + new Vector3(input.x * handSpeed * Time.deltaTime, input.y * handSpeed * Time.deltaTime, 0f);

        Vector2? canvasSize = ApplicationController.Instance.UI.GetCanvasSize();
        float halfSize = canvasSize.Value.x * 0.5f;

        if (nextPosition.x < -halfSize || nextPosition.x > halfSize)
            nextPosition.x = rectCatcher.anchoredPosition3D.x;

        rectCatcher.anchoredPosition3D = nextPosition;

        Rect rect = rectCatcher.rect;
        rect.position = rectCatcher.anchoredPosition3D;

        if (rect.Overlaps(collectableCharacterRect) && !collectOnCoolDown)
        {
            int collectedIndex = collectText.Length - collectTextQueue.Count;
            enterMatrixTextLit.sprite = matrixTextSpritesLit[collectedIndex];

            ShowCollectEffectAndGiveScore();

            //if (collectTextQueue.Count > 0)
            if (collectTextQueue.Count > 12) //<-- DEBUG REMOVE DEBUG REMOVE DEBUG REMOVE
            {
                ResetTextFallPosition(matrixTexts.Where(mt => mt.Collectable).First());
                currentCollectChar = collectTextQueue.Dequeue();
                collectOnCoolDown = true;
                Task.Delay(1000).ContinueWith(_ => collectOnCoolDown = false);
            }
            else
            {
                Disposables.Dispose();
                ExecuteInUpdate = false;
                
                allCharactersCollected = true;

                //Stop blinking text
                enterMatrixTextLit.gameObject.SetActive(true);

                //Kill hovering and start moving up
                DOTween.Kill(enterMatrixText.transform);
                enterMatrixText.rectTransform.DOShakeAnchorPos(2f, 5).OnComplete(() => 
                {
                    enterMatrixText.rectTransform.DOAnchorPos3DY(enterMatrixTextHiddenPosition.y, 5f).SetEase(Ease.InSine);
                });
                
                //Delay and start fading out
                Task.Delay(4000).ContinueWith(_ =>
                {
                    ApplicationController.Instance.FadeImageInOut(1f, ApplicationController.Instance.C64PaletteArr[1], () =>
                    {
                        //End the demo by exiting last coroutine and calling base.End();                    
                        base.End();
                    }, null);
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
    }

    private void ShowCollectEffectAndGiveScore()
    {
        //Give score
        Score.Value += 100;

        //Play collect sound
        AudioController.Instance.PlaySoundEffect("CollectLetter");

        Vector3 charSP = Camera.main.WorldToScreenPoint(collectableCharacterRect.position);

        //Get hand screen pos and...
        Vector3 uiElementPosition = Camera.main.WorldToScreenPoint(matrixTexts.Where(mt => mt.Collectable).First().TmpText.transform.position);
        uiElementPosition.y = collectableCharacterRect.y;

        //Final world point for laser beam
        Vector3 wp = Camera.main.ScreenToWorldPoint(uiElementPosition);

        wp.z = 1f;
        InstantiateLaserBeam(wp);
    }

    private SimpleSpriteAnimator InstantiateLaserBeam(Vector3 pos)
    {
        //Laser beam
        SpriteRenderer asteroidRenderer = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("LaserBeam", pos, laserSprites.First());
        SimpleSpriteAnimator laserSpriteAnimator = asteroidRenderer.gameObject.AddComponent<SimpleSpriteAnimator>();
        laserSpriteAnimator.Sprites = laserSprites;
        laserSpriteAnimator.DontAutoPlay = true;
        laserSpriteAnimator.StopToLastFrame = true;
        laserSpriteAnimator.AnimationFrameDelay = 0.06f;
        laserSpriteAnimator.Loops = 1;
        laserSpriteAnimator.Play(true, () =>
        {
            GameObject.Destroy(laserSpriteAnimator.gameObject);
        }, 0, false);
        return laserSpriteAnimator;
    }

    private IEnumerator AnimateMatrixTexts()
    {
        int counter = 0;
        while (true)
        {
            foreach (MatrixText mt in matrixTexts)
            {
                string mtString = mt.TmpText.text;

                if (mtString.Length < mt.Letters && mt.Shorten == false)
                {
                    mtString += (char)UnityEngine.Random.Range(32, 90);
                }
                else if (mtString.Length > 1)
                {
                    mt.Shorten = true;

                    if (counter % 2 == 0)
                        mtString = mtString.Substring(1);

                    
                    char addChar = mt.Collectable ? currentCollectChar : (char)UnityEngine.Random.Range(32, 90);
                    mtString = mtString.Remove(mtString.Length - 1, 1) + addChar;

                    if (allCharactersCollected && mtString.Length == 1)
                        mtString = "";

                    mt.TmpText.transform.localPosition += new Vector3(0, -mt.Speed * characterSize, 0);
                }
                else
                {
                    if (!allCharactersCollected)
                        ResetTextFallPosition(mt);

                    //TODO: This doesn't work but can't figure out why. FIX THIS AND REMEMBER TO SET QUEUE CHECK COUNT TO zero 00000
                    /*
                    if (collectTextQueue.Count > 12)
                    {
                        ResetTextFallPosition(mt);
                    }
                    else
                    {

                    }
                    */
                }

                mt.TmpText.text = mtString;

                //Default green color combo
                (Color dark, Color light) colorCombo = (ApplicationController.Instance.C64PaletteArr[11], ApplicationController.Instance.C64PaletteArr[10]);

                if (mt.Collectable)
                {
                    //Collectable character flickers white/yellow
                    colorCombo = (ApplicationController.Instance.C64PaletteArr[10], counter % 2 == 0 ? ApplicationController.Instance.C64PaletteArr[1] : ApplicationController.Instance.C64PaletteArr[9]);

                    //Collectable character collision rect
                    collectableCharacterRect = TextFunctions.GetTextMeshLastCharacterLocalBounds(mt.TmpText);

                    float offset = collectableCharacterRect.y + collectableCharacterRect.height;
                    collectableCharacterRect.position = new Vector2(mt.TmpText.transform.localPosition.x, mt.TmpText.rectTransform.anchoredPosition3D.y + offset);
                }

                if (mtString.Length >= 1)
                    TextFunctions.TmpTextColor(mt.TmpText, mtString.Length - 1, colorCombo.dark, colorCombo.light);
            }

            //Flash lit text randomly            
            if (!allCharactersCollected)
                enterMatrixTextLit.gameObject.SetActive(UnityEngine.Random.Range(0, 2) == 0 ? true : false);

            counter++;
            yield return new WaitForSeconds(.1f);
        }
    }

    private void ResetTextFallPosition(MatrixText matrixText)
    {
        int newRandomX = 0;

        //Randomize new x position for text that is not used by any other text
        while (matrixTexts.Any(text => text.TmpText.GetComponent<RectTransform>().anchoredPosition3D.x == (float)newRandomX))
            newRandomX = UnityEngine.Random.Range(0, 36) * 8 - 144;

        matrixText.TmpText.transform.localPosition = new Vector3(newRandomX, ApplicationController.Instance.UI.GetCanvasSize().Value.y + UnityEngine.Random.Range(0, 20) * characterSize, matrixText.TmpText.transform.localPosition.z);
        matrixText.TmpText.text = "";
        matrixText.Shorten = false;
    }

    private TMP_Text InstantiateMatrixText(string goName, float xpos)
    {
        RectTransform txtRect = ApplicationController.Instance.UI.CreateRectTransformObject(goName, new Vector2(characterSize, characterSize), new Vector3(xpos, ApplicationController.Instance.UI.GetCanvasSize().Value.y + UnityEngine.Random.Range(0, 40) * characterSize, 0), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
        txtRect.pivot = new Vector2(0.5f, 0f);
        txt = TextFunctions.AddTextMeshProTextComponent(txtRect, "C64_Pro_Mono-STYLE", (int)characterSize, ApplicationController.Instance.C64PaletteArr[1]);
        txt.alignment = TextAlignmentOptions.Top;
        txt.lineSpacing = characterSize;
        txt.text = "";
        AddToGeneratedObjectsDict(txtRect.gameObject.name, txtRect.gameObject);
        return txt;
    }
}

public class MatrixText
{
    public bool Shorten;
    public bool Collectable;
    public int Speed;
    public int Letters;
    public TMP_Text TmpText;
}
