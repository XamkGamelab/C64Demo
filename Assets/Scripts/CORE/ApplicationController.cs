using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using UniRx;

public class ApplicationController : SingletonMono<ApplicationController>
{
    public UIController UI { get; private set; }
    public Dictionary<string, CameraSettings> CameraSettings { get; private set; }
    public Color[] C64PaletteArr => ColorFunctions.ColorsFromPaletteStripTexture(Instantiate(Resources.Load<Texture2D>("C64PaletteStrrip32x2_16_colors")), 16);

    private Image flashFadeImage;
    private DemoEffectBase currentDemoEffect;
    private int currentEffecIndex = 0;
 
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

        CameraSettings = new Dictionary<string, CameraSettings>()
        {
            { "orthoPixel", new CameraSettings { Orthographic = true, OrthographicSize = 1.2f } },
            { "perspectiveFov60", new CameraSettings { Orthographic = false, FOV = 60f } },
            { "perspectiveFov90", new CameraSettings { Orthographic = false, FOV = 90f } }
        };

        demoEffects = new List<DemoEffectBase>()
        {
            new DemoeffectIntro().Init() ,
            new DemoEffectRun().Init(),            
            new DemoeffectTextScroller().Init(),
            new DemoEffectEyeBalls().Init(), //<-- MIKSI SKIPATAAN!=?!=!=         
            new DemoEffectSunset().Init(),
            new DemoEffectMatrix().Init(),
            new DemoEffectTimeBomb().Init()

        };

        RunAllDemoEffects(0);

        InputController.Instance.EscDown.Subscribe(b => { if (b) QuitApp(); });
    }

    public void FadeImageInOut(float duration, Color color, System.Action callBack, System.Action callBackEnd)
    {
        ActivateFlashFadeImage(color);
        Color transparent = new Color(color.r, color.g, color.b, 0);
        flashFadeImage.color = transparent;
        flashFadeImage.DOColor(color, duration).OnComplete(() =>
        {
            //Invoke callback method before fade out
            callBack?.Invoke();
            flashFadeImage.DOColor(transparent, duration).OnComplete(() =>
            {
                //Invoke callback method after fade out complete
                callBackEnd?.Invoke();
                flashFadeImage.gameObject.SetActive(false);
            });
        });
    }

    public void RunAllDemoEffects(int startFrom)
    {
        if (startFrom >= demoEffects.Count)
        {
            //This is just for debugging
            Debug.LogWarning("INVALID INDEX OR DEMOS RAN THROUGH, STARTING AGAIN.");
            startFrom = 0;
        }

        currentEffecIndex = startFrom;
        currentDemoEffect = demoEffects[currentEffecIndex];

        StopAllCoroutines();
        StartCoroutine(currentDemoEffect.Run(() =>
        {
            //Move to next effect when this effect ends
            currentEffecIndex++;
            RunAllDemoEffects(currentEffecIndex);
        }));
    }

    private void QuitApp()
    {
        Debug.Log("QUIT");
        Application.Quit();
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
