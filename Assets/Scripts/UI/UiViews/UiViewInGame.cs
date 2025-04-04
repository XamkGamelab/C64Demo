using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UniRx;
using TMPro;
using System;
using UnityEngine.Rendering;

public class UiViewInGame : UiView
{
    public TMP_Text TextScore;
    public TMP_Text TextHiScore;
    public TMP_Text TextTime;
    public TMP_Text TextPar;
    public TMP_Text TextTutorial;

    public RectTransform PanelTutorial;

    //Give this when effect starts (Run() in demo base )
    private float startTime;
    
    private IDisposable disposableTimer;

    protected override void Awake()
    {
        //Hide tutorial panel
        PanelTutorial.gameObject.SetActive(false);

        base.Awake();
    }

    public override void Show(UiView _showViewWhenThisHides = null)
    {
        base.Show(_showViewWhenThisHides);        
    }

    public void ShowTutorial(string tutorialText, int showSeconds = 5)
    {
        //Dispose previous timer if any and start observing again
        disposableTimer?.Dispose();

        //Show tutorial panel with correct text
        TextTutorial.text = tutorialText;
        PanelTutorial.gameObject.SetActive(true);

        //Hide tutorial after showSeconds
        disposableTimer = Observable.Timer(TimeSpan.FromSeconds(showSeconds)).Subscribe(t =>
        {
            Debug.Log("SHOW TUTORIAL AFTER " + showSeconds + " secs");
            PanelTutorial.gameObject.SetActive(false);

        });
    }

    public void UpdateScoreAndTime(int score, int hiscore, float currentTime, float parTime)
    {
        Debug.Log("Update score and time values from DemoEffectBase tuple reactive values");
        TextScore.text = score.ToString();
        TextHiScore.text = hiscore.ToString();
        TextTime.text = currentTime.ToString("00:00.00");
        TextPar.text = parTime.ToString("00:00.00");
    }

    public void UpdateScores(int score, int hiscore)
    {
        TextScore.text = score.ToString("00000000");
        TextHiScore.text = hiscore.ToString("00000000");
    }
    public void UpdateNewEffect(float parTime)
    {
        TextPar.text = parTime.ToString("00:00.00");
        //Update and show tutorial text
    }
    public void UpdateRunningTime(float runningTime)
    {
        TextTime.text = runningTime.ToString("00:00.00");
    }
}
