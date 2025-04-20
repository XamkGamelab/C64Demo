using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using DG.Tweening;
using System.Linq;
using UniRx;
using static UnityEditor.PlayerSettings;

public class DemoEffectEyeBalls : DemoEffectBase
{
    private Image img;
    private Image leftTextImg;
    private Image rightTextImg;

    private SpriteRenderer shipRenderer;
    private SpriteRenderer bigEyeRenderer;
    private List<GenericEnemy> ballEnemies = new List<GenericEnemy>();

   

    private Material spriteScrollMaterial;

    //Gameplay
    private Vector2 moveInput = Vector2.zero;
    private Rect playAreaRect;
    private float shipSpeed = 2f;
    private float textImgOffsetY = 0f;
    private float textImgScrollSpeed = 0.4f;
    private bool isEnding = false;
    private Vector3 shipAppearPosition;
    private Vector3 shipStartPosition;
    private SimpleSpriteAnimator bigEyeAnimator;
    private SimpleSpriteAnimator bloodSpriteAnimator;

    private List<Sprite> eyeSprites; // => TextureAndGaphicsFunctions.LoadSpriteSheet("EyeSheet");
    private List<Sprite> bigEyeOpenSprites; // => TextureAndGaphicsFunctions.LoadSpriteSheet("BigEyeOpenSpriteSheetPSD");
    private List<Sprite> eyeBloodSplashSprites;

    public override DemoEffectBase Init(float parTime, string tutorialText)
    {
        eyeSprites = TextureAndGaphicsFunctions.LoadSpriteSheet("EyeSheet");
        bigEyeOpenSprites = TextureAndGaphicsFunctions.LoadSpriteSheet("BigEyeOpenSpriteSheetPSD");
        eyeBloodSplashSprites = TextureAndGaphicsFunctions.LoadSpriteSheet("EyeBloodSplashSpriteSheet");

        spriteScrollMaterial = GameObject.Instantiate<Material>(Resources.Load<Material>("CustomSpriteScrolling"));

        //Play are rect and ship start position
        playAreaRect = CameraFunctions.GetCameraRect(Camera.main, Camera.main.transform.position);
        shipAppearPosition = shipStartPosition = new Vector3(playAreaRect.center.x - .64f, playAreaRect.yMin + .16f, 1f);
        
        //Main bg image
        RectTransform rect = ApplicationController.Instance.UI.CreateRectTransformObject("Image_lizard_eye", new Vector2(320, 200), Vector2.zero, Vector2.one * .5f, Vector2.one * .5f); 
        rect.SetAsFirstSibling();
        img = rect.AddComponent<Image>();
        img.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("Images/LizardEye"));        
        AddToGeneratedObjectsDict(rect.gameObject.name, rect.gameObject);

        //Left text image
        RectTransform rectImgLeft = ApplicationController.Instance.UI.CreateRectTransformObject("Image_text_left", new Vector2(24, 200), Vector2.zero, new Vector2(0f,0f), new Vector2(0f, 0f));
        rectImgLeft.pivot = new Vector2(0f, 0f);
        rectImgLeft.SetAsLastSibling();
        leftTextImg = rectImgLeft.AddComponent<Image>();
        leftTextImg.material = spriteScrollMaterial;
        leftTextImg.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("OpenYourEyesText"));
        AddToGeneratedObjectsDict(rectImgLeft.gameObject.name, rectImgLeft.gameObject);

        //Right text image
        RectTransform rectImgRight = ApplicationController.Instance.UI.CreateRectTransformObject("Image_text_right", new Vector2(24, 200), new Vector3(320f, 0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f));
        rectImgRight.pivot = new Vector2(1f, 0f);
        rectImgRight.SetAsLastSibling();
        rightTextImg = rectImgRight.AddComponent<Image>();
        rightTextImg.material = spriteScrollMaterial;
        rightTextImg.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("OpenYourEyesText"));
        AddToGeneratedObjectsDict(rectImgRight.gameObject.name, rectImgRight.gameObject);

        int amount = 16;        
        //Eye ball sprites
        for (int i = 0; i < amount; i++)
        {
            SpriteRenderer ballRenderer = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("EyeBall_" + i, new Vector3(0f, 0f, 1f), eyeSprites[0]);
            ballRenderer.sortingOrder = 100 + i;
            GenericEnemy enemy = ballRenderer.gameObject.
                AddComponent<GenericEnemy>().
                Init(null, typeof(CircleCollider2D), true, false).
                AddBulletHitAction(HandleBulletHitBall) as GenericEnemy;

            SimpleSpriteAnimator ballSpriteAnimator = ballRenderer.gameObject.AddComponent<SimpleSpriteAnimator>();
            ballSpriteAnimator.DontAutoPlay = true;            
            ballSpriteAnimator.StopToLastFrame = true;            
            ballSpriteAnimator.Loops = 1;
            ballSpriteAnimator.Sprites = eyeSprites;
            ballSpriteAnimator.Play(false);

            ballEnemies.Add(enemy);
            AddToGeneratedObjectsDict(ballRenderer.gameObject.name, ballRenderer.gameObject);
        }

        //Ship sprite        
        shipRenderer = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("SpaceShip", shipStartPosition, GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("SpaceShipTopDown")));
        shipRenderer.sortingOrder = 1000;
        AddToGeneratedObjectsDict(shipRenderer.gameObject.name, shipRenderer.gameObject);

        shipAppearPosition.y = playAreaRect.yMin - shipRenderer.size.y;
        shipRenderer.transform.position = shipAppearPosition;

        bigEyeRenderer = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("BigEyeOpenSprite", new Vector3(0f,0f,1f), bigEyeOpenSprites.First());
        bigEyeRenderer.sortingOrder = 1000;
        bigEyeAnimator = bigEyeRenderer.gameObject.AddComponent<SimpleSpriteAnimator>();
        bigEyeAnimator.Sprites = bigEyeOpenSprites;
        bigEyeAnimator.DontAutoPlay = true;
        bigEyeAnimator.StopToLastFrame = true;
        bigEyeAnimator.AnimationFrameDelay = 0.2f;
        bigEyeAnimator.Loops = 1;        
        bigEyeAnimator.Play(false);

        AddToGeneratedObjectsDict(bigEyeRenderer.gameObject.name, bigEyeRenderer.gameObject);

        return base.Init(parTime, tutorialText);
    }

    public override IEnumerator Run(System.Action callbackEnd)
    {
        yield return base.Run(callbackEnd);

        Camera.main.backgroundColor = ApplicationController.Instance.C64PaletteArr[0];

        moveInput = Vector2.zero;
        isEnding = false;

        img.gameObject.SetActive(true);
        leftTextImg.gameObject.SetActive(true);
        rightTextImg.gameObject.SetActive(true);

        bigEyeAnimator.Play(false, 0, true);
        bigEyeRenderer.gameObject.SetActive(true);

        bigEyeAnimator.Play(true, () =>
        {
            shipRenderer.transform.position = shipAppearPosition;
            shipRenderer.gameObject.SetActive(true);
            shipRenderer.transform.DOMoveY(shipStartPosition.y, 2f).SetDelay(2f);

            //Subscribe to input when ship is in position
            InputController.Instance.Fire1.Subscribe(b => HandleFireInput(b)).AddTo(Disposables);
            InputController.Instance.Horizontal.Subscribe(f => moveInput.x = f).AddTo(Disposables);

        }, 0, true);

        ballEnemies.ForEach(be => 
        {
            be.BulletHitCount = 0;
            be.gameObject.SetActive(true);
            be.transform.position = new Vector3(0f, 0f, 1f);
            be.GetComponent<SimpleSpriteAnimator>().ResetState(0);            
        });

        ExecuteInUpdate = true;

        //Wait that ship has moved into position and eye is opening before small eyes start to animate:
        //yield return new WaitForSeconds(.5f);

        //Show ship after the eye opens

        yield return AnimateBalls();
    }
    
    public override void DoUpdate()
    {
        MoveShip(moveInput);
        ScrollTextImages();
        base.DoUpdate();
    }

    private void ScrollTextImages()
    {
        leftTextImg.material.SetVector("_Offset", new Vector4(0, textImgOffsetY, 0, 0));
        rightTextImg.material.SetVector("_Offset", new Vector4(0, textImgOffsetY, 0, 0));
        textImgOffsetY += textImgScrollSpeed * Time.deltaTime;
    }

    private void HandleFireInput(bool b)
    {
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

    private SpriteRenderer InstantiateBloodSplash(Vector3 pos)
    {
        //Eye blood splash
        SpriteRenderer bloodRenderer = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("BloodSplash", pos, eyeBloodSplashSprites.First());
        bloodSpriteAnimator = bloodRenderer.gameObject.AddComponent<SimpleSpriteAnimator>();
        bloodSpriteAnimator.Loops = 1;
        bloodSpriteAnimator.DestroyAfterLoops = true;
        bloodSpriteAnimator.Sprites = eyeBloodSplashSprites;
        return bloodRenderer;
    }

    private void MoveShip(Vector2 input)
    {
        Vector3 nextPosition = shipRenderer.transform.position + new Vector3(input.x * shipSpeed * Time.deltaTime, 0f, 0f);

        if (nextPosition.x < playAreaRect.xMin || nextPosition.x > playAreaRect.xMax)
            nextPosition.x = shipRenderer.transform.position.x;
        
        shipRenderer.transform.position = nextPosition;
    }

    private void HandleBulletHitBall(GenericActorBase ballEnemy)
    {
        //Give score
        Score.Value += 100;

        if (ballEnemy.BulletHitCount == 1)
        {
            //Play eye opening
            ballEnemy.GetComponent<SimpleSpriteAnimator>().Play(true);
        }
        else if (ballEnemy.BulletHitCount > 1)
        {
            DOTween.Kill(ballEnemy.transform);
            ballEnemy.gameObject.SetActive(false);
            //Instantiate blood sprite anim
            InstantiateBloodSplash(ballEnemy.transform.position);
        }

        /*
        //Stop movement
        DOTween.Kill(ballEnemy.transform);

        //Move to center
        ballEnemy.transform.DOLocalMove(new Vector3(0, 0, 1f), 2f, false);
        */


        if (ballEnemies.All(be => be.BulletHitCount > 1) && !isEnding)
        {
            isEnding = true;

            //Should all hit actions for all balls be removed?

            Debug.Log("ALL BALLS HAVE BEEN HIT, END DEMO!!!!");
            //Unsubsribe from input and asteroid spawning
            Disposables.Dispose();
            moveInput = Vector2.zero;

            //Move ship to up? and fade in transition
            shipRenderer.transform.DOMoveY(playAreaRect.yMax + shipRenderer.size.y, 2f, false).SetEase(Ease.InExpo).OnComplete(() => {

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
        
        /* correct time step is: full movement time (e.g. 2 sec) / amount of balls * 2 ( = e.g. 16) which gives current hard-coded step of 0.125f */
        float angleStep = 22.5f;
        float radius = 0.64f;        
        float fullMoveTime = 1.2f;
        
        for (int i = 0; i < ballEnemies.Count(); i++)
        {
            float ypos = radius * Mathf.Cos(Mathf.PI * angleStep * i / 180f);
            float xpos = radius * Mathf.Sin(Mathf.PI * angleStep * i / 180f);

            Vector2 posMovePoint = new Vector2(xpos, ypos);            
            GameObject currentBallRenderer = ballEnemies[i].gameObject;

            //currentBallRenderer.SetActive(true);
            currentBallRenderer.transform.DOLocalMove(new Vector3(posMovePoint.x, posMovePoint.y, 1f), fullMoveTime, false).SetDelay(fullMoveTime / ballEnemies.Count() * 2f * i).OnComplete(() =>
            {
                //currentBallRenderer.transform.DOLocalMove(new Vector3(posMovePoint.x * -1f, posMovePoint.y * -1f, 1f), fullMoveTime * 2f, false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
                RestartBallDoMove(currentBallRenderer, posMovePoint, fullMoveTime);
            });
        }
        
        yield return null;
        
    }

    private void RestartBallDoMove(GameObject ball, Vector2 posMovePoint, float fullMoveTime)
    {
        posMovePoint *= -1f;
        Vector2 tempMovePoint = posMovePoint;
        tempMovePoint *= 1f + MathFunctions.GetSin(Time.time, 2f, .64f);
        ball.transform.DOLocalMove(new Vector3(tempMovePoint.x, tempMovePoint.y, 1f), fullMoveTime * 2f, false).SetEase(Ease.Linear).OnComplete(() => RestartBallDoMove(ball, posMovePoint, fullMoveTime));
    }
}
