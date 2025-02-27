using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UniRx;

public class DemoEffectTimeBomb : DemoEffectBase
{
    private RectTransform headingRect;
    private TMP_Text headingTxt;
    public override DemoEffectBase Init()
    {
        //Timer clock text
        headingRect = ApplicationController.Instance.UI.CreateRectTransformObject("Text_timer_clock", new Vector2(82f, 82f), new Vector3(0, 32f, 0), new Vector2(.5f, .5f), new Vector2(.5f, .5f));        
        headingTxt = TextFunctions.AddTextMeshProTextComponent(headingRect, "G7_Segment_7", 82, ApplicationController.Instance.C64PaletteArr[11]);
        headingTxt.alignment = TextAlignmentOptions.Center;
        headingTxt.enableWordWrapping = false;
        headingTxt.text = "00:12";
        AddToGeneratedObjectsDict(headingRect.gameObject.name, headingRect.gameObject);

        return base.Init();
    }

    public override IEnumerator Run(Action endDemoCallback)
    {
        yield return base.Run(endDemoCallback);
        Camera.main.backgroundColor = ApplicationController.Instance.C64PaletteArr[0];
        headingTxt.gameObject.SetActive(true);

        //Subscribe to input
        InputController.Instance.Fire1.Subscribe(b => HandleFireInput(b)).AddTo(Disposables);
    }

    private void HandleFireInput(bool b)
    {
        if (!FirePressed && b)
        {
            ApplicationController.Instance.FadeImageInOut(1f, ApplicationController.Instance.C64PaletteArr[0], () =>
            {
                //End the demo by exiting last coroutine and calling base.End();                
                base.End();
            }, null);
        }
        FirePressed = b;
    }
}
