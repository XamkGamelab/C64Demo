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
    private List<Renderer> ballRenderers = new List<Renderer>();

    private List<Sprite> eyeSprites => TextureAndGaphicsFunctions.LoadSpriteSheet("EyeSheet");

    //Gameplay
    private Vector2 moveInput = Vector2.zero;
    private Rect playAreaRect;
    private float shipSpeed = 2f;

    public override DemoEffectBase Init()
    {
        RectTransform rect = ApplicationController.Instance.UI.CreateRectTransformObject("Image_lizard_eye", new Vector2(320, 200), Vector2.zero, Vector2.one * .5f, Vector2.one * .5f); 
        rect.SetAsFirstSibling();
        img = rect.AddComponent<Image>();
        img.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("Images/LizardEye"));        
        AddToGeneratedObjectsDict(rect.gameObject.name, rect.gameObject);

        int amount = 8;
        

        //Eye ball sprites
        for (int i = 0; i < amount; i++)
        {
            SpriteRenderer ballRenderer = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("EyeBall_" + i, new Vector3(0f, 0f, 1f), eyeSprites[1]);
            ballRenderer.sortingOrder = 100 + i;
            ballRenderers.Add(ballRenderer);
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

        Camera.main.backgroundColor = ApplicationController.Instance.C64PaletteArr[0];

        moveInput = Vector2.zero;

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

    //TODO: Make a proper function of this crap
    private IEnumerator AnimateBalls()
    {
        //r * cos/sin of rad angle if movement step:
        float ypos = 0.64f * Mathf.Cos(Mathf.PI * 67.5f / 180f);
        float xpos = 0.64f * Mathf.Sin(Mathf.PI * 67.5f / 180f);

        Debug.Log("Point on circle: " + xpos + " y: " + ypos);

        /* correct time step is: full movement time (e.g. 2 sec) / amount of balls * 2 ( = e.g. 16) which gives current hard-coded step of 0.125f */

        //UP
        ballRenderers[0].gameObject.SetActive(true);
        ballRenderers[0].transform.DOLocalMove(new Vector3(0, 0.64f, 1f), 1f, false).OnComplete(() =>
        {
            ballRenderers[0].transform.DOLocalMove(new Vector3(0, -0.64f, 1f), 1f, false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        });

        //UP 22.5 RIGHT
        yield return new WaitForSeconds(0.125f); //was 1f
        ballRenderers[1].gameObject.SetActive(true);
        ballRenderers[1].transform.DOLocalMove(new Vector3(0.24491f, 0.591f, 1f), 1f, false).OnComplete(() =>
        {
            ballRenderers[1].transform.DOLocalMove(new Vector3(-0.24491f, -0.591f, 1f), 1f, false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        });

        //UP 45 RIGHT
        yield return new WaitForSeconds(0.125f); //was 4.5        
        ballRenderers[2].gameObject.SetActive(true);
        ballRenderers[2].transform.DOLocalMove(new Vector3(0.452f, 0.452f, 1f), 1f, false).OnComplete(() =>
        {
            ballRenderers[2].transform.DOLocalMove(new Vector3(-0.452f, -0.452f, 1f), 1f, false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        });

        //UP 67.5 RIGHT
        yield return new WaitForSeconds(0.125f); //was 4.5        
        ballRenderers[3].gameObject.SetActive(true);
        ballRenderers[3].transform.DOLocalMove(new Vector3(0.591f, 0.24491f, 1f), 1f, false).OnComplete(() =>
        {
            ballRenderers[3].transform.DOLocalMove(new Vector3(-0.591f, -0.24491f, 1f), 1f, false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        });

        //RIGHT
        yield return new WaitForSeconds(0.125f); //< was 2.5        
        ballRenderers[4].gameObject.SetActive(true);
        ballRenderers[4].transform.DOLocalMove(new Vector3(0.64f, 0, 1f), 1f, false).OnComplete(() =>
        {
            ballRenderers[4].transform.DOLocalMove(new Vector3(-0.64f, 0, 1f), 1f, false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        });

        //DOWN 22.5 RIGHT
        yield return new WaitForSeconds(0.125f); //was 1f
        ballRenderers[5].gameObject.SetActive(true);
        ballRenderers[5].transform.DOLocalMove(new Vector3(0.591f, -0.24491f, 1f), 1f, false).OnComplete(() =>
        {
            ballRenderers[5].transform.DOLocalMove(new Vector3(-0.591f, 0.24491f, 1f), 1f, false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        });

        //DOWN 45 RIGHT
        yield return new WaitForSeconds(0.125f); //was 3.25        
        ballRenderers[6].gameObject.SetActive(true);
        ballRenderers[6].transform.DOLocalMove(new Vector3(0.452f, -0.452f, 1f), 1f, false).OnComplete(() =>
        {
            ballRenderers[6].transform.DOLocalMove(new Vector3(-0.452f, 0.452f, 1f), 1f, false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        });

        //DOWN 22.5 LEFT
        yield return new WaitForSeconds(0.125f); //was 1f
        ballRenderers[7].gameObject.SetActive(true);
        ballRenderers[7].transform.DOLocalMove(new Vector3(0.24491f, -0.591f, 1f), 1f, false).OnComplete(() =>
        {
            ballRenderers[7].transform.DOLocalMove(new Vector3(-0.24491f, 0.591f, 1f), 1f, false).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        });
    }
}
