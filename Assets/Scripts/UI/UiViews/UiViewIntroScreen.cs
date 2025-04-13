using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UniRx;
using System;
using DG.Tweening;

public class UiViewIntroScreen : UiView
{
    public CanvasGroup GroupAll;
    public CanvasGroup GroupTextIntro;
    public TMP_Text TextIntroText;
    public TMP_Text TextPressFire;
    
    private bool firePressed = false;
    private IDisposable disposableInputFire;

    protected override void Awake()
    {
        base.Awake();        
    }

    public override void Show(UiView _showViewWhenThisHides = null)
    {
        base.Show(_showViewWhenThisHides);
        StartCoroutine(AnimateIntroText());
    }

    private IEnumerator AnimateIntroText()
    {
        GroupTextIntro.alpha = 0f;
        GroupTextIntro.DOFade(1f, 1f);

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
