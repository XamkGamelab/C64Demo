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
    private SpriteRenderer shipRenderer;    
    private List<GenericEnemy> ballEnemies = new List<GenericEnemy>();

    private List<Sprite> eyeSprites => TextureAndGaphicsFunctions.LoadSpriteSheet("EyeSheet");

    //Gameplay
    private Vector2 moveInput = Vector2.zero;
    private Rect playAreaRect;
    private float shipSpeed = 2f;
    private bool isEnding = false;

    public override DemoEffectBase Init()
    {
        RectTransform rect = ApplicationController.Instance.UI.CreateRectTransformObject("Image_lizard_eye", new Vector2(320, 200), Vector2.zero, Vector2.one * .5f, Vector2.one * .5f); 
        rect.SetAsFirstSibling();
        img = rect.AddComponent<Image>();
        img.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("Images/LizardEye"));        
        AddToGeneratedObjectsDict(rect.gameObject.name, rect.gameObject);

        int amount = 16;
        

        //Eye ball sprites
        for (int i = 0; i < amount; i++)
        {
            SpriteRenderer ballRenderer = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("EyeBall_" + i, new Vector3(0f, 0f, 1f), eyeSprites[0]);
            ballRenderer.sortingOrder = 100 + i;
            GenericEnemy enemy = ballRenderer.gameObject.
                AddComponent<GenericEnemy>().
                Init(null, typeof(CircleCollider2D), true, false).
                AddBulletHitAction(HandleBulletHitBall);

            SimpleSpriteAnimator ballSpriteAnimator = ballRenderer.gameObject.AddComponent<SimpleSpriteAnimator>();
            ballSpriteAnimator.DontAutoPlay = true;            
            ballSpriteAnimator.StopToLastFrame = true;
            ballSpriteAnimator.Loops = 1;
            ballSpriteAnimator.Sprites = eyeSprites;
            ballSpriteAnimator.Play(false);

            ballEnemies.Add(enemy);
            AddToGeneratedObjectsDict(ballRenderer.gameObject.name, ballRenderer.gameObject);
        }

        //Play are rect        
        playAreaRect = CameraFunctions.GetCameraRect(Camera.main, Camera.main.transform.position);        
        
        //Ship sprite        
        shipRenderer = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("SpaceShip", new Vector3(playAreaRect.center.x, playAreaRect.yMin + .16f, 1f), GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("SpaceShipTopDown")));
        shipRenderer.sortingOrder = 1000;
        AddToGeneratedObjectsDict(shipRenderer.gameObject.name, shipRenderer.gameObject);

        return base.Init();
    }

    public override IEnumerator Run(System.Action callbackEnd)
    {
        yield return base.Run(callbackEnd);

        Debug.Log("Start eye balls!");

        Camera.main.backgroundColor = ApplicationController.Instance.C64PaletteArr[0];

        moveInput = Vector2.zero;
        isEnding = false;

        ExecuteInUpdate = true;

        //Subscribe to input        
        InputController.Instance.Fire1.Subscribe(b => HandleFireInput(b)).AddTo(Disposables);
        InputController.Instance.Horizontal.Subscribe(f => moveInput.x = f).AddTo(Disposables);

        shipRenderer.gameObject.SetActive(true);

        img.gameObject.SetActive(true);
        
        //TODO: Make a proper function of this crap
        yield return AnimateBalls();
    }
    
    public override void DoUpdate()
    {
        MoveShip(moveInput);
        base.DoUpdate();
    }

    private void HandleFireInput(bool b)
    {
        Debug.Log("Eye balls FIRE -> " + b);

        if (!FirePressed && b)
        {
            AudioController.Instance.PlaySoundEffect("Laser_Shoot_1");
            InstantiateLaserShot(shipRenderer.transform.position);
        }
        FirePressed = b;
    }

    private void InstantiateLaserShot(Vector3 pos)
    {
        SpriteRenderer laserRenderer = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("Laser", pos, GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("LaserGreenVertical")));        
        laserRenderer.AddComponent<GenericBullet>().Init(new Vector2(0, 2f), true);
    }

    private void MoveShip(Vector2 input)
    {
        Vector3 nextPosition = shipRenderer.transform.position + new Vector3(input.x * shipSpeed * Time.deltaTime, 0f, 0f);

        if (nextPosition.x < playAreaRect.xMin || nextPosition.x > playAreaRect.xMax)
            nextPosition.x = shipRenderer.transform.position.x;
        
        shipRenderer.transform.position = nextPosition;
    }

    private void HandleBulletHitBall(GenericEnemy ballEnemy)
    {
        ballEnemy.GetComponent<SimpleSpriteAnimator>().Play(true);

        DOTween.Kill(ballEnemy.transform);

        ballEnemy.transform.DOLocalMove(new Vector3(0, 0, 1f), 2f, false);

        if (ballEnemies.All(be => be.BulletHitCount > 0) && !isEnding)
        {
            isEnding = true;

            //Should all hit actions for all balls be removed?

            Debug.Log("ALL BALLS HAVE BEEN HIT, END DEMO!!!!");
            //Unsubsribe from input and asteroid spawning
            Disposables.Dispose();
            moveInput = Vector2.zero;

            //Move ship to up? and fade in transition
            shipRenderer.transform.DOMoveY(playAreaRect.yMax, 2f, false).SetEase(Ease.InExpo).OnComplete(() => {

                ApplicationController.Instance.FadeImageInOut(1f, ApplicationController.Instance.C64PaletteArr[1], () =>
                {
                    base.End(false);
                }, null);
            });
        }
    }

    //TODO: Make a proper function of this crap
    private IEnumerator AnimateBalls()
    {
        //r * cos/sin of rad angle if movement step:
        //float ypos = 0.64f * Mathf.Cos(Mathf.PI * 67.5f / 180f);
        //float xpos = 0.64f * Mathf.Sin(Mathf.PI * 67.5f / 180f);
        
        /* correct time step is: full movement time (e.g. 2 sec) / amount of balls * 2 ( = e.g. 16) which gives current hard-coded step of 0.125f */

        
        float angleStep = 22.5f;
        float radius = 0.64f;        
        float fullMoveTime = .6f;
        
        for (int i = 0; i < ballEnemies.Count(); i++)
        {
            float ypos = radius * Mathf.Cos(Mathf.PI * angleStep * i / 180f);
            float xpos = radius * Mathf.Sin(Mathf.PI * angleStep * i / 180f);
            
            Vector2 posMovePoint = new Vector2(xpos, ypos);            
            GameObject currentBallRenderer = ballEnemies[i].gameObject;

            currentBallRenderer.SetActive(true);
            currentBallRenderer.transform.DOLocalMove(new Vector3(posMovePoint.x, posMovePoint.y, 1f), fullMoveTime, false).SetDelay(fullMoveTime / ballEnemies.Count() * 2f * i).OnComplete(() =>
            {
                currentBallRenderer.transform.DOLocalMove(new Vector3(posMovePoint.x * -1f, posMovePoint.y * -1f, 1f), fullMoveTime * 2f, false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);                  
            });
        }
        
        yield return null;
        
    }
}
