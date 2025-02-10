using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using DG.Tweening;
public class DemoEffectEyeBalls : DemoEffectBase
{
    private Image img;
    private Image imgBall;
    private List<RectTransform> balls;
    public override DemoEffectBase Init()
    {
        balls = new List<RectTransform>();

        RectTransform rect = ApplicationController.Instance.UI.CreateRectTransformObject("Image_lizard_eye", new Vector2(320, 200), Vector2.zero, Vector2.one * .5f, Vector2.one * .5f); 
        rect.SetAsFirstSibling();
        img = rect.AddComponent<Image>();
        img.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("Images/LizardEye"));        
        AddToGeneratedObjectsDict(rect.gameObject.name, rect.gameObject);

        RectTransform rectBall_1 = ApplicationController.Instance.UI.CreateRectTransformObject("Ball_0", new Vector2(24, 24), Vector2.zero, Vector2.one * .5f, Vector2.one * .5f);        
        imgBall = rectBall_1.AddComponent<Image>();
        imgBall.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("whiteBall"));
        AddToGeneratedObjectsDict(rectBall_1.gameObject.name, rectBall_1.gameObject);

        balls.Add(rectBall_1);

        int amount = 4;
        for (int i = 1; i < amount; i++)
        {
            RectTransform ball = GameObject.Instantiate(imgBall.gameObject).GetComponent<RectTransform>();
            ball.gameObject.name = "Ball_" + i;
            ApplicationController.Instance.UI.ParentTransformToUI(ball.transform, null, new Vector3(0, 0, 0));
            AddToGeneratedObjectsDict(ball.gameObject.name, ball.gameObject);
            balls.Add(ball);
        }

        return base.Init();
    }

    public override IEnumerator Run(System.Action callbackEnd)
    {
        Camera.main.backgroundColor = ApplicationController.Instance.C64PaletteArr[0];

        img.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        balls[0].gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);

        
        balls[0].gameObject.SetActive(true);
        balls[0].DOLocalMove(new Vector3(64, 0, 0), 1f, true).OnComplete(() =>
        {
            balls[0].DOLocalMove(new Vector3(-64, 0, 0), 1f, true).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        });

        float d = Vector3.Distance(new Vector3(-64, 0, 0), new Vector3(64, 0, 0));
        Debug.Log("VECT S DIST: " + d);

        d = Vector3.Distance(new Vector3(43, 43, 0), new Vector3(-43, -43, 0));
        Debug.Log("VECT DIST: " + d);

        yield return new WaitForSeconds(2.5f);
        balls[1].gameObject.SetActive(true);
        balls[1].DOLocalMove(new Vector3(0, 64f, 0), 1f, true).OnComplete(() => 
        {
            balls[1].DOLocalMove(new Vector3(0, -64f, 0), 1f, true).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        });

        yield return new WaitForSeconds(3.25f);
        balls[2].gameObject.SetActive(true);
        balls[2].DOLocalMove(new Vector3(50, -50f, 0), 1f, true).OnComplete(() =>
        {
            balls[2].DOLocalMove(new Vector3(-50, 50f, 0), 1f, true).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        });

        
        yield return new WaitForSeconds(4.5f);
        balls[3].gameObject.SetActive(true);
        balls[3].DOLocalMove(new Vector3(50, 50, 0), 1f, true).OnComplete(() =>
        {
            balls[3].DOLocalMove(new Vector3(-50, -50, 0), 1f, true).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        });

        base.Run(callbackEnd);
    }

    public override void End(System.Action callbackEnd)
    {
        Debug.Log("end effect");
        base.End(callbackEnd);
    }

    public override void DoUpdate()
    {
        base.DoUpdate();
    }
}
