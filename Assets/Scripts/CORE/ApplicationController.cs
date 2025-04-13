using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using UniRx;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using System;

public class ApplicationController : SingletonMono<ApplicationController>
{
    public UIController UI { get; private set; }
    public Dictionary<string, CameraSettings> CameraSettings { get; private set; }
    public Color[] C64PaletteArr => ColorFunctions.ColorsFromPaletteStripTexture(Instantiate(Resources.Load<Texture2D>("C64PaletteStrrip32x2_16_colors")), 16);

    private Image flashFadeImage;
    private DemoEffectBase currentDemoEffect;
    private int currentEffectIndex = 0;
 
    //Instantiate and init effect to this list after UI is instantiated
    private List<DemoEffectBase> demoEffects;

    private UiViewInGame uiViewInGame;

    private CameraRT cameraRT;

    private CompositeDisposable disposables = new CompositeDisposable();
    private IDisposable disposableMonitorTimer;

    private double turnMonitorOnDelayMs = 1500.0;
    private float effectStartedTime;
    private float runningTime;
    private float totalTime;
    private GameObject selectedObject;

    [RuntimeInitializeOnLoadMethod]
    static void OnInit()
    {
        Instance.Init();
    }
    public async void Init()
    {
        //Hide cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //Init render texture cam
        cameraRT = InitSceneCameraRT().Init();
        //Istantiate and initialize UI
        UI = InstantiateUIPrefab().Init();
        selectedObject = EventSystem.current.currentSelectedGameObject;

        flashFadeImage = InstantiateFlashFadeImage();
        flashFadeImage.gameObject.SetActive(false);

        //Because piece of shit unity haven't set CORRECT Canvas size until unknown delay
        //DON'T SHOW UI BEFORE THIS!!!!!
        await Task.Delay(1000);

        Debug.Log("Ready");

        //Play music
        AudioController.Instance.PlayTrack("MenuIntro", 1f, 4f);

        CameraSettings = new Dictionary<string, CameraSettings>()
        {
            { "orthoPixel", new CameraSettings { Orthographic = true, OrthographicSize = 1.2f } },
            { "perspectiveFov60", new CameraSettings { Orthographic = false, FOV = 60f } },
            { "perspectiveFov90", new CameraSettings { Orthographic = false, FOV = 90f } }
        };

        demoEffects = new List<DemoEffectBase>()
        {
            //new DemoeffectIntro().Init(20f, "Press Space or Fire") ,
            new DemoEffectRun().Init(20f, "Toggle left/right rapidly to run"),                        
            //new DemoeffectTextScroller().Init(20f, "Control ship with left/right and up/down.\nPress fire to shoot."),
            //new DemoEffectEyeBalls().Init(30f, "Left/right to control the ship.\nPress fire to shoot."),
            //new DemoeffectNoise().Init(20f, ""),
            //new DemoEffectSunset().Init(30f, "Left/right to control the character. Press fire to shoot"),
            new DemoEffectMatrix().Init(30f, "Left/right to control the hand.\nCatch highlighted falling letters"),
            //new DemoEffectTimeBomb().Init(30f, "Defuse the bomb")
            
        };

        demoEffects.ForEach(effect => 
        {
            //Instead subscribe to Running (bool reactive, from demo base!) time when effect Runs!

            //Also update here updates the time value in UI (in game), if the effect is running
            effect.Score.Subscribe(score => 
            {
                //Update runtime hiscore (TODO: I know, side effect)
                if (score > effect.HiScore.Value)
                    effect.HiScore.Value = score;

                uiViewInGame?.UpdateScores(demoEffects.Select(effect => effect.Score.Value).Sum(), demoEffects.Select(effect => effect.HiScore.Value).Sum());
            });
            effect.Started.Subscribe(b =>
            {
                if (b)
                {
                    totalTime += runningTime;
                    runningTime = 0;
                    effectStartedTime = Time.time;

                    //Update in-game UI when new effect starts running
                    if (uiViewInGame != null)
                    {
                        uiViewInGame.UpdateNewEffect(effect.ParTime);
                        uiViewInGame.ShowTutorial(effect.TutorialText);
                    }
                }
                else
                {
                    //Show final time and score in UI when effect is finished
                }
            }).AddTo(disposables); //TODO: this is never disposed, is it actually needed...
        });
    }

    public void StartNewGame()
    {
        //Animate camera in towards screen
        cameraRT.AnimateScreenIn();

        //Reset scores of all effects
        demoEffects.ForEach(e => e.Score.Value = 0);

        //Reset total time
        totalTime = 0f;

        //Delay "turning monitor on"
        disposableMonitorTimer?.Dispose();
        disposableMonitorTimer = Observable.Timer(TimeSpan.FromMilliseconds(turnMonitorOnDelayMs)).Subscribe(_ => 
        {
            //Get instance to update scores and times before running any effects
            uiViewInGame = UI.ShowUiView<UiViewInGame>() as UiViewInGame;

            //Start first effect
            RunAllDemoEffects(0);
        });
    }

    public void ShowCredits(bool show)
    {
        //Animate camera in towards credits note
        if (show)
            cameraRT.AnimateToCredits();
        else
            cameraRT.AnimateScreenOut();
    }

    public void ReturnToMainMenu()
    {
        //This needs delay for camera animation, but then show the menu after do tween complete ->
        UI.ShowUiView<UiViewMainMenu>();
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
            //Hide in-game UI and show win screen, reset demo index and return
            startFrom = 0;
            uiViewInGame.Hide();

            //Animate camera OUT and AFTER THAT show the main menu again
            cameraRT.AnimateScreenOut();

            UiViewWinScreen winScreen = UI.ShowUiView<UiViewWinScreen>() as UiViewWinScreen;
            winScreen.ShowScoreAndTime(demoEffects.Select(effect => effect.Score.Value).Sum(), demoEffects.Select(effect => effect.HiScore.Value).Sum(), totalTime); //<-- TODO: Total time is NOT tracked yet.

            return;
        }

        currentEffectIndex = startFrom;
        currentDemoEffect = demoEffects[currentEffectIndex];

        StopAllCoroutines();
        StartCoroutine(currentDemoEffect.Run(() =>
        {
            //Move to next effect when this effect ends
            currentEffectIndex++;
            RunAllDemoEffects(currentEffectIndex);
        }));
    }

    public void QuitApp()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }

    private CameraRT InitSceneCameraRT()
    {
        return FindObjectOfType<CameraRT>(true);        
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

    private void UpdateRunningTime()
    {
        runningTime = Time.time - effectStartedTime;

        if (currentDemoEffect.Started.Value)
            uiViewInGame?.UpdateRunningTime(runningTime);
    }

    private void Update()
    {
        if (currentDemoEffect != null)
        {
            if (currentDemoEffect.ExecuteInUpdate)
                currentDemoEffect.DoUpdate();

            UpdateRunningTime();
        }

        //Piece of shit Unity, piece of shit event system workaround
        if (EventSystem.current.currentSelectedGameObject == null)        
            EventSystem.current.SetSelectedGameObject(selectedObject);        
        else        
            selectedObject = EventSystem.current.currentSelectedGameObject;        
    }
}
