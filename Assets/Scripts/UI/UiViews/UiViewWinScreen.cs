using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UniRx;
using System;
using DG.Tweening;

public class UiViewWinScreen : UiView
{
    public CanvasGroup GroupAll;
    public RectTransform GroupAllRect;
    public RectTransform ScoreContainer;
    public RectTransform TimeContainer;
    public TMP_Text TextScore;
    public TMP_Text TextTime;
    public TMP_Text TextToMainMenu;

    private bool firePressed = false;
    private IDisposable disposableInputFire;

    private float groupAllEndPositionY;
    private float groupAllHiddenPositionY;

    protected override void Awake()
    {
        base.Awake();
        GroupAllRect = GroupAll.GetComponent<RectTransform>();
        groupAllEndPositionY = GroupAllRect.anchoredPosition3D.y;
        groupAllHiddenPositionY = -GroupAllRect.sizeDelta.y;
    }

    public override void Show(UiView _showViewWhenThisHides = null)
    {

        ScoreContainer.gameObject.SetActive(false);
        TimeContainer.gameObject.SetActive(false);
        TextToMainMenu.gameObject.SetActive(false);
        base.Show(_showViewWhenThisHides);
    }

    public override void Hide(bool _stopCoroutines = false)
    {
        base.Hide(_stopCoroutines);
    }

    public void ShowScoreAndTime(int score, int hiscore, float totalTime)
    {
        StopAllCoroutines();

        TimeSpan timeSpan = TimeSpan.FromSeconds(totalTime);

        TextScore.text = score.ToString("00000000");
        //hi-score is not implemented yet

        TextTime.text = string.Format("{0:00}:{1:00}.{2:00}", (int)timeSpan.TotalMinutes, timeSpan.Seconds, timeSpan.Milliseconds / 10); totalTime.ToString("00:00.00");

        //Start fading out music (new track fades it back in)
        AudioController.Instance.FadeOutMusic(4f);

        GroupAllRect.anchoredPosition3D = new Vector3(GroupAllRect.anchoredPosition3D.x, groupAllHiddenPositionY, GroupAllRect.anchoredPosition3D.z);
        GroupAllRect.
            DOAnchorPos3DY(groupAllEndPositionY, 1f).
            SetDelay(1f).
            SetEase(Ease.OutSine).
            OnComplete(() =>
        {
            StartCoroutine(AnimateScoreAndTime());
        });
    }

    private IEnumerator AnimateScoreAndTime()
    {
        ScoreContainer.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        TimeContainer.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        TextToMainMenu.gameObject.SetActive(true);
        //Subscribe to input
        yield return new WaitForSeconds(1f);
        disposableInputFire = InputController.Instance.Fire1.Subscribe(b => HandleFireInput(b));
    }

    private void HandleFireInput(bool b)
    {
        if (!firePressed && b)
        {
            Hide();
            disposableInputFire?.Dispose();
            firePressed = false;
            ApplicationController.Instance.ReturnToMainMenu();
        }
        firePressed = b;
    }
}
