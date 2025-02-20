using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Linq;
public class DemoEffectMatrix : DemoEffectBase
{
    private TMP_Text txt;
    private List<MatrixText> matrixTexts = new List<MatrixText>();
    
    private const float characterSize = 8;
    public override DemoEffectBase Init()
    {
        float steps = UIController.GetCanvasSize().Value.x / characterSize * 2f;
        for (int i = 0; i < steps; i++)
            matrixTexts.Add(new MatrixText { TmpText = InstantiateMatrixText("MatrixText_" + i, i * characterSize * 2 - UIController.GetCanvasSize().Value.x * .5f), Speed = UnityEngine.Random.Range(1, 4), Letters = UnityEngine.Random.Range(16, 24) });

        return base.Init();
    }

    public override IEnumerator Run(Action endDemoCallback)
    {
        yield return base.Run(endDemoCallback);

        ExecuteInUpdate = true;

        Camera.main.backgroundColor = ApplicationController.Instance.C64PaletteArr[0];

        //Enable all generated objects        
        GeneratedObjectsSetActive(true);

        yield return AnimateMatrixTexts();
    }

    public override void DoUpdate()
    {
        base.DoUpdate();
    }

    private IEnumerator AnimateMatrixTexts()
    {
        int counter = 0;
        while(true)
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
                    mtString = mtString.Remove(mtString.Length - 1, 1) + (char)UnityEngine.Random.Range(32, 90);
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

                //Colorize
                TextFunctions.TmpTextColor(mt.TmpText, mtString.Length - 1, ApplicationController.Instance.C64PaletteArr[11], ApplicationController.Instance.C64PaletteArr[10]);

            }

            counter++;
            yield return new WaitForSeconds(.08f);
        }
    }

    private TMP_Text InstantiateMatrixText(string goName, float xpos)
    {
        RectTransform txtRect = ApplicationController.Instance.UI.CreateRectTransformObject(goName, new Vector2(characterSize, characterSize), new Vector3(xpos, UIController.GetCanvasSize().Value.y + UnityEngine.Random.Range(0,32)* characterSize, 0), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
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
    public int Speed;
    public int Letters;
    public TMP_Text TmpText;
}
