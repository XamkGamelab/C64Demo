using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UniRx;
using System;

public class UiViewWinScreen : UiView
{
    public RectTransform ScoreContainer;
    public RectTransform TimeContainer;
    public TMP_Text TextScore;
    public TMP_Text TextTime;
    public TMP_Text TextToMainMenu;

    private bool firePressed = false;
    private IDisposable disposableInputFire;

    protected override void Awake()
    {
        base.Awake();        
    }

    public override void Show(UiView _showViewWhenThisHides = null)
    {
        disposableInputFire?.Dispose();
        ScoreContainer.gameObject.SetActive(false);
        TimeContainer.gameObject.SetActive(false);
        TextToMainMenu.gameObject.SetActive(false);
        base.Show(_showViewWhenThisHides);        
    }

    public void ShowScoreAndTime()
    {
        Debug.Log("START ANIMATE SCORE AND TIME COROUTINE ONLY ONCE!!!");
        StartCoroutine(AnimateScoreAndTime());
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
            ApplicationController.Instance.ReturnToMainMenu();
            Hide();
        }
        firePressed = b;
    }
}
