using System.Collections.Generic;
using UnityEngine;

public class ApplicationController : SingletonMono<ApplicationController>
{
    public UIController UI { get; private set; }
    public Color[] C64PaletteArr => ColorFunctions.ColorsFromPaletteStripTexture(Instantiate(Resources.Load<Texture2D>("C64PaletteStrrip32x2_16_colors")), 16);

    private DemoEffectBase currentDemoEffect;

    //Instantiate and init effect to this list after UI is instantiated
    private List<DemoEffectBase> demoEffects;

    [RuntimeInitializeOnLoadMethod]
    static void OnInit()
    {
        Instance.Init();
    }
    public void Init()
    {
        UI = InstantiateUIPrefab().Init();

        demoEffects = new List<DemoEffectBase>()
        {
            new DemoeffectIntro().Init() ,
            new DemoeffectTextScroller().Init(),
            new DemoEffectEyeBalls().Init()
        };

        currentDemoEffect = demoEffects[2];
        StartCoroutine(currentDemoEffect.Run());
    }

    private UIController InstantiateUIPrefab()
    {
        GameObject uiGo = Instantiate(Resources.Load("UI")) as GameObject;
        return uiGo.GetComponent<UIController>();
    }

    private void Update()
    {
        if (currentDemoEffect?.ExecuteInUpdate == true)
        {
            currentDemoEffect.DoUpdate();
        }
    }
}
