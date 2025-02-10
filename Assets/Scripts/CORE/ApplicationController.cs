using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ApplicationController : SingletonMono<ApplicationController>
{
    public UIController UI { get; private set; }
    public Color[] C64PaletteArr => ColorFunctions.ColorsFromPaletteStripTexture(Instantiate(Resources.Load<Texture2D>("C64PaletteStrrip32x2_16_colors")), 16);

    private Image flashFadeImage;
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
        flashFadeImage = InstantiateFlashFadeImage();
        flashFadeImage.gameObject.SetActive(false);

        demoEffects = new List<DemoEffectBase>()
        {
            new DemoeffectIntro().Init() ,
            new DemoeffectTextScroller().Init(),
            new DemoEffectEyeBalls().Init()
        };

        currentDemoEffect = demoEffects[2];
        StartCoroutine(currentDemoEffect.Run());
    }

    private void FlashWhite(float duration)
    {
        ActivateFlashFadeImage(C64PaletteArr[1]);
    }

    private void FadeDipToBlack(float duration)
    {
        ActivateFlashFadeImage(C64PaletteArr[0]);
    }

    private void ActivateFlashFadeImage(Color color)
    {
        flashFadeImage.gameObject.SetActive(true);
        flashFadeImage.transform.SetAsLastSibling();
        flashFadeImage.color = color;
    }

    private UIController InstantiateUIPrefab()
    {
        GameObject uiGo = Instantiate(Resources.Load("UI")) as GameObject;
        return uiGo.GetComponent<UIController>();
    }

    private Image InstantiateFlashFadeImage()
    {
        RectTransform r = UI.CreateRectTransformObject("Image_flash_fade", new Vector2(Screen.width, Screen.height), new Vector3(0, 0, 0), Vector2.zero, Vector2.one, new Vector2(0, 0), new Vector2(0, 0));        
        return UIController.AddResourcesImageComponent(r, "white_32x32", C64PaletteArr[1]);
    }

    private void Update()
    {
        if (currentDemoEffect?.ExecuteInUpdate == true)
        {
            currentDemoEffect.DoUpdate();
        }
    }
}
