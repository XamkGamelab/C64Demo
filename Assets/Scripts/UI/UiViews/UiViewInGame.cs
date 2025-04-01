using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
public class UiViewInGame : UiView
{
    public TMP_Text TextScore;
    public TMP_Text TextHiScore;
    public TMP_Text TextTime;
    public TMP_Text TextPar;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Show(UiView _showViewWhenThisHides = null)
    {
        base.Show(_showViewWhenThisHides);        
    }

    public void UpdateScoreAndTime(int score, int hiscore, float currentTime, float parTime)
    {
        Debug.Log("Update score and time values from DemoEffectBase tuple reactive values");
        TextScore.text = score.ToString();
        TextHiScore.text = hiscore.ToString();
        TextTime.text = currentTime.ToString("00:00.00");
        TextPar.text = parTime.ToString("00:00.00");
    }
}
