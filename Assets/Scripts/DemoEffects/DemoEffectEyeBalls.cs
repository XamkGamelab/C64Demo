using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using DG.Tweening;
using System.Linq;
using UniRx;
public class DemoEffectEyeBalls : DemoEffectBase
{
    private Image img;
    private Image imgBall;
    private List<RectTransform> balls;
    //private Renderer ballRenderer;
    private List<Renderer> ballRenderers = new List<Renderer>();

    private List<Sprite> eyeSprites => TextureAndGaphicsFunctions.LoadSpriteSheet("EyeSheet");
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

        int amount = 5;
        

        //Eye ball sprites
        for (int i = 0; i < amount; i++)
        {
            SpriteRenderer ballRenderer = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("EyeBall_" + i, new Vector3(0f, 0f, 1f), eyeSprites[1]);
            ballRenderer.sortingOrder = 1000 + i;
            ballRenderers.Add(ballRenderer);
        }

        return base.Init();
    }

    public override IEnumerator Run(System.Action callbackEnd)
    {
        yield return base.Run(callbackEnd);

        float xpos = 0.64f * Mathf.Cos(Mathf.PI * 22.5f / 180f);
        float ypos = 0.64f * Mathf.Sin(Mathf.PI * 22.5f / 180f);

        Debug.Log("Point on circle: " + xpos + " y: " + ypos);

        Camera.main.backgroundColor = ApplicationController.Instance.C64PaletteArr[0];

        img.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        balls[0].gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);

        //Subscribe to input
        
        InputController.Instance.Fire1.Subscribe(b => HandleFireInput(b)).AddTo(Disposables);

        //45 deg - 0.4525483f
        //22.5 deg - 0,5912828 y: 0,2449174

        ballRenderers[0].gameObject.SetActive(true);
        ballRenderers[0].transform.DOLocalMove(new Vector3(0.64f, 0, 1f), 1f, false).OnComplete(() =>
        {
            ballRenderers[0].transform.DOLocalMove(new Vector3(-0.64f, 0, 1f), 1f, false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        });
        

        yield return new WaitForSeconds(2.5f);
        
        ballRenderers[1].gameObject.SetActive(true);
        ballRenderers[1].transform.DOLocalMove(new Vector3(0, 0.64f, 1f), 1f, false).OnComplete(() =>
        {
            ballRenderers[1].transform.DOLocalMove(new Vector3(0, -0.64f, 1f), 1f, false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        });

        yield return new WaitForSeconds(3.25f);
        
        ballRenderers[2].gameObject.SetActive(true);
        ballRenderers[2].transform.DOLocalMove(new Vector3(0.452f, -0.452f, 1f), 1f, false).OnComplete(() =>
        {
            ballRenderers[2].transform.DOLocalMove(new Vector3(-0.452f, 0.452f, 1f), 1f, false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        });


        yield return new WaitForSeconds(4.5f);
        
        ballRenderers[3].gameObject.SetActive(true);
        ballRenderers[3].transform.DOLocalMove(new Vector3(0.452f, 0.452f, 1f), 1f, false).OnComplete(() =>
        {
            ballRenderers[3].transform.DOLocalMove(new Vector3(-0.452f, -0.452f, 1f), 1f, false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        });

        yield return new WaitForSeconds(1f);

        ballRenderers[4].gameObject.SetActive(true);
        ballRenderers[4].transform.DOLocalMove(new Vector3(0.24491f, 0.591f, 1f), 1f, false).OnComplete(() =>
        {
            ballRenderers[4].transform.DOLocalMove(new Vector3(-0.24491f, -0.591f, 1f), 1f, false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        });
    }
    
    public override void DoUpdate()
    {
        base.DoUpdate();
    }

    private void HandleFireInput(bool b)
    {
        if (!FirePressed && b)
        {
            ApplicationController.Instance.FadeImageInOut(1f, ApplicationController.Instance.C64PaletteArr[0], () =>
            {
                //End the demo by exiting last coroutine and calling base.End();                
                base.End();
            }, null);
        }
        FirePressed = b;
    }
}
