using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UniRx;
using TMPro;
using System;
using UnityEngine.Rendering;
using DG.Tweening;

public class UiViewInGame : UiView
{
    public TMP_Text TextScore;
    public TMP_Text TextHiScore;
    public TMP_Text TextTime;
    public TMP_Text TextPar;
    public TMP_Text TextTutorial;
    public TMP_Text TextTimeBonus;

    public RectTransform PanelTutorial;
    public RectTransform TimeBonusContainer;

    //Give this when effect starts (Run() in demo base )
    private float startTime;
    private Color textTimeBonusInitColor;
    private IDisposable disposableTimer;



    private float tutorialHiddenPositionY => PanelTutorial.sizeDelta.y;

    protected override void Awake()
    {
        //Hide tutorial panel
        PanelTutorial.gameObject.SetActive(false);
        TimeBonusContainer.gameObject.SetActive(false);

        textTimeBonusInitColor = TextTimeBonus.color;

        base.Awake();
    }

    public override void Show(UiView _showViewWhenThisHides = null)
    {
        base.Show(_showViewWhenThisHides);
    }

    public void ShowLastBonusAndNewTutorial(string tutorialText, int lastEffectTimeBonus, Action timeBonusCallback, int showSeconds = 5)
    {
        //Corner case for credits effect, that has now tutorial
        if (string.IsNullOrEmpty(tutorialText))
            return;

        //Corner case for first effect, that has no time bonus from the last effect to show
        if (lastEffectTimeBonus > 0)
        {
            TextTimeBonus.text = lastEffectTimeBonus.ToString("000000");
            TimeBonusContainer.gameObject.SetActive(true);
            TextTimeBonus.color = textTimeBonusInitColor;
            TextTimeBonus.
                DOColor(Color.white, 0.2f).
                SetLoops(10, LoopType.Yoyo).
                OnComplete(() =>
                {
                    TimeBonusContainer.gameObject.SetActive(false);
                    ShowTutorialText(tutorialText, showSeconds);

                    //Invoke callback to update effects scores in app controller
                    timeBonusCallback?.Invoke();
                });
        }
        else
        {
            //Only show tutorial when the first effect starts
            ShowTutorialText(tutorialText, showSeconds);
        }
    }
    
    public void ShowTutorialText(string tutorialText, int showSeconds = 5)
    {
        DOTween.Kill(PanelTutorial);

        //Dispose previous timer if any and start observing again
        disposableTimer?.Dispose();

        //Show tutorial panel animated with correct text        
        PanelTutorial.anchoredPosition3D = new Vector3(0, tutorialHiddenPositionY, 0);
        PanelTutorial.gameObject.SetActive(true);
        TextTutorial.text = tutorialText;
        PanelTutorial.ForceUpdateRectTransforms();

        PanelTutorial.DOAnchorPos3DY(0f, 1f).SetEase(Ease.OutSine);
        //Hide tutorial after showSeconds
        disposableTimer = Observable.Timer(TimeSpan.FromSeconds(showSeconds)).Subscribe(t =>
        {
            PanelTutorial.DOAnchorPos3DY(tutorialHiddenPositionY, 1f).SetEase(Ease.InSine).OnComplete(() => PanelTutorial.gameObject.SetActive(false));
        });
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
