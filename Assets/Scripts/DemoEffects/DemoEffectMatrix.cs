using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class DemoEffectMatrix : DemoEffectBase
{
    private TMP_Text txt;
    private List<TMP_Text> matrixTexts = new List<TMP_Text>();
    public override DemoEffectBase Init()
    {

        for (int i = 0; i < 8; i++)
            matrixTexts.Add(InstantiateMatrixText("TextMatrix_" + i));

        return base.Init();
    }

    public override IEnumerator Run(Action endDemoCallback)
    {
        yield return base.Run(endDemoCallback);

        ExecuteInUpdate = true;

        //Subscribe to input        
        //InputController.Instance.Horizontal.Subscribe(f => moveInput.x = f).AddTo(Disposables);

        matrixTexts.ForEach(m => m.transform.DOLocalMoveY(-UIController.GetCanvasSize().Value.y, 5f,true).SetEase(Ease.Linear).OnComplete(() => Debug.Log("done")));

        Camera.main.backgroundColor = ApplicationController.Instance.C64PaletteArr[0];

        //Enable all generated objects        
        GeneratedObjectsSetActive(true);
    }

    public override void DoUpdate()
    {
        base.DoUpdate();
    }

    private TMP_Text InstantiateMatrixText(string goName)
    {
        float rndSteps = UIController.GetCanvasSize().Value.x / 16f;
        RectTransform txtRect = ApplicationController.Instance.UI.CreateRectTransformObject(goName, new Vector2(8, 8), new Vector3(UnityEngine.Random.Range(-rndSteps,rndSteps) * 8f, 0f, 0), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
        txtRect.pivot = new Vector2(0.5f, 1f);
        txt = TextFunctions.AddTextMeshProTextComponent(txtRect, "C64_Pro_Mono-STYLE", 8, ApplicationController.Instance.C64PaletteArr[1]);
        txt.alignment = TextAlignmentOptions.Bottom;
        txt.text = "45kjfh%gg56#&%kg";
        AddToGeneratedObjectsDict(txtRect.gameObject.name, txtRect.gameObject);
        return txt;
    }
}
