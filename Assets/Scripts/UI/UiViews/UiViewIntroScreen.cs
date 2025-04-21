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

    private string[] introTexts = new string[] {
        "Aeons ago, in a not-so-distant galaxy, programmers wielded machines called microcomputers.",
        "Upon them, they crafted programs with no purpose — no function — other than to defy the limits of their humble silicon. These creations were called demos.",
        "They served as beacons of creativity and raw code, pushing hardware beyond its bounds and proclaiming the brilliance of their makers.",
        "This is the tale of those bold explorers of the digital frontier…"
    };

    protected override void Awake()
    {
        base.Awake();
        TextPressFire.gameObject.SetActive(false);
        TextIntroText.gameObject.SetActive(false);
        TextIntroText.text = "";
    }

    public override void Show(UiView _showViewWhenThisHides = null)
    {
        base.Show(_showViewWhenThisHides);
        StartCoroutine(AnimateIntroText());
    }

    private IEnumerator AnimateIntroText()
    {
        //Start playing the opening credits music
        AudioController.Instance.PlayTrack("OpeningCredits", 1f, 1f);

        yield return new WaitForSeconds(1f);
        TextPressFire.gameObject.SetActive(true);
        TextIntroText.gameObject.SetActive(true);
        disposableInputFire = InputController.Instance.Fire1.Subscribe(b => HandleFireInput(b));

        for (int i = 0; i < introTexts.Length; i++)
        {
            TextIntroText.text = introTexts[i];
            GroupTextIntro.alpha = 0f;
            GroupTextIntro.DOFade(1f, 1f);
            yield return new WaitForSeconds(5f);
            GroupTextIntro.DOFade(0f, 1f);
            yield return new WaitForSeconds(1.2f);
        }

        //Automatically trigger moving to main menu, if player hasn't pressed fire:
        yield return new WaitForSeconds(2f);
        HandleFireInput(true);
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
