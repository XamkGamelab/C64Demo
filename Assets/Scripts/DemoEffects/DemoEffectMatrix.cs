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

public class DemoEffectMatrix : DemoEffectBase
{
    private TMP_Text txt;
    private List<MatrixText> matrixTexts = new List<MatrixText>();

    private Image handRed;
    private Image handBlue;
    private Image catcherHand;

    RectTransform rectCatcher;

    //Gameplay
    private Vector2 moveInput = Vector2.zero;
    private float handSpeed = 120f;

    private const float characterSize = 8;


    public override DemoEffectBase Init()
    {
        float steps = UIController.GetCanvasSize().Value.x / characterSize / 2f;
        for (int i = 0; i < steps; i++)
            matrixTexts.Add(new MatrixText { TmpText = InstantiateMatrixText("MatrixText_" + i, i * characterSize * 2 - UIController.GetCanvasSize().Value.x * .5f), Speed = UnityEngine.Random.Range(1, 4), Letters = UnityEngine.Random.Range(20, 30) });

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

        
        rectCatcher = ApplicationController.Instance.UI.CreateRectTransformObject("CatchingHand2", new Vector2(24, 21), new Vector3(0, -UIController.GetCanvasSize().Value.y*0.5f + 8f, 0), Vector2.one * .5f, Vector2.one * .5f);
        catcherHand = rectCatcher.AddComponent<Image>();
        catcherHand.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("CatchingHand"));
        AddToGeneratedObjectsDict(rectCatcher.gameObject.name, rectCatcher.gameObject);

        return base.Init();
    }

    public override IEnumerator Run(Action endDemoCallback)
    {
        yield return base.Run(endDemoCallback);

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

        //THESE NEED TO BE UI CANVAS BOUNDS
        /*
        Rect rect = CameraFunctions.GetCameraRect(Camera.main, Camera.main.transform.position);        
        rect.height *= 0.6f;

        if (nextPosition.x < rect.xMin || nextPosition.x > rect.xMax)
            nextPosition.x = catcherRenderer.transform.position.x;
        */
        rectCatcher.anchoredPosition3D = nextPosition;

        /*
         * - Okay, and this is how this shit is going to go down. Now we need be sure
         * - that both text and cather anchoredPositions match (or offset in rect).
         * - Then we can simply do rect overlaps: https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Rect.Overlaps.html
         * - This shoud be working way to check for collisions, but the rects just need to pivoted correctly!!!
         * 
         */

    }

    private IEnumerator AnimateMatrixTexts()
    {
        int counter = 0;
        while(true)
        {

            foreach (MatrixText mt in matrixTexts)
            {
                /*
                if (mt.Collectable)
                {
                    Rect rect = TextFunctions.GetTextMeshCharacterWorldBounds(mt.TmpText, mt.TmpText.text.Length - 1);
                    Debug.Log("TEXT LENGH: " + mt.TmpText.text.Length);
                }
                */

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

                    char addChar = mt.Collectable ? 'E' : (char)UnityEngine.Random.Range(32, 90);
                    mtString = mtString.Remove(mtString.Length - 1, 1) + addChar;
                    mt.TmpText.transform.localPosition += new Vector3(0, -mt.Speed * characterSize, 0);
                }
                else
                {
                    //RESET POSITION BACK AND SHORTEN TO FALSE!!!

                    mt.TmpText.transform.localPosition = new Vector3(mt.TmpText.transform.localPosition.x, UIController.GetCanvasSize().Value.y, mt.TmpText.transform.localPosition.z);                    
                    mt.TmpText.text = "";
                    mt.Shorten = false;
                }

                mt.TmpText.text = mtString;

                //Default green color combo
                (Color dark, Color light) colorCombo = (ApplicationController.Instance.C64PaletteArr[11], ApplicationController.Instance.C64PaletteArr[10]);

                if (mt.Collectable)
                { 
                    //collectable flickers white/yellow
                    colorCombo = (ApplicationController.Instance.C64PaletteArr[10], counter % 2 == 0 ? ApplicationController.Instance.C64PaletteArr[1] : ApplicationController.Instance.C64PaletteArr[9]);
                    Rect rect = TextFunctions.GetTextMeshLastCharacterLocalBounds(mt.TmpText);
                    
                    rect.position = mt.TmpText.transform.localPosition;
                    //Debug.DrawLine(new Vector3(rect.xMin, rect.yMin, 0), new Vector3(rect.xMax, rect.yMin, 0), Color.cyan, 1f);
                    Debug.Log("RECT: " + rect);
                    // ^ and this needs to in scope of MoveHand function, so that we can check the collision there!
                }

                TextFunctions.TmpTextColor(mt.TmpText, mtString.Length - 1, colorCombo.dark, colorCombo.light);

            }

            counter++;
            yield return new WaitForSeconds(.1f);
        }
    }

    private TMP_Text InstantiateMatrixText(string goName, float xpos)
    {
        RectTransform txtRect = ApplicationController.Instance.UI.CreateRectTransformObject(goName, new Vector2(characterSize, characterSize), new Vector3(xpos, UIController.GetCanvasSize().Value.y + UnityEngine.Random.Range(0,40)* characterSize, 0), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
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
