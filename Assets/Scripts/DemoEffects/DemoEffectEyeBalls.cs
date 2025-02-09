using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
public class DemoEffectEyeBalls : DemoEffectBase
{
    private Image img;
    public override DemoEffectBase Init()
    {
        RectTransform rect = ApplicationController.Instance.UI.CreateRectTransformObject("Image_lizard_eye", new Vector2(320, 200), Vector2.zero, Vector2.one * .5f, Vector2.one * .5f); 
        rect.SetAsFirstSibling();
        img = rect.AddComponent<Image>();
        img.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("Images/LizardEye"));        
        AddToGeneratedObjectsDict(rect.gameObject.name, rect.gameObject);

        return base.Init();
    }

    public override IEnumerator Run()
    {
        Camera.main.backgroundColor = ApplicationController.Instance.C64PaletteArr[0];

        img.gameObject.SetActive(true);
        return base.Run();
    }

    public override void DoUpdate()
    {
        base.DoUpdate();
    }
}
