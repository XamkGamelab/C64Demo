using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using UnityEngine.Rendering;
using System;

public class DemoeffectTextScroller : DemoEffectBase
{
    private Image img;

    private float startTime;
    
    //Bottom text scroller
    private RectTransform txtRect;
    private RectTransform txtRectClone;
    private RectTransform headingRect;
    
    private TMP_Text headingTxt;
    private TMP_Text txt;
    private TMP_Text txtClone;

    //Sprites
    private SpriteRenderer shipRenderer;

    public float scrollSpeed = 2000f;
    private float textWidth;
    private Vector3 startPosition;

    //Top scrolling gradients
    private Texture2D gradientTexture;    
    private Sprite gradientSprite;
    
    private List<Image> gradientImages = new List<Image>();
    private int gradientBarHeight = 8;
    private int gradientBottomHeight = 32;
    private Vector2 initOffset = new Vector2(0, 100f);

    //Starfield
    private List<Transform> stars = new List<Transform>();
    private float starMoveSpeed = 60f;
    private float starsDistance = 1f;
    private bool loopScroller = true;

    //Gameplay
    private Vector2 moveInput = Vector2.zero;
    private float shipSpeed = 2f;
    
    public override DemoEffectBase Init()
    {
        //Top gradients
        InstantiateGradientImages(8, (4, 32));

        //Bottom gradient bg
        RectTransform bottomGradientRect = ApplicationController.Instance.UI.CreateRectTransformObject("Image_gradient_bottom", new Vector2(UIController.GetCanvasSize().Value.x, gradientBottomHeight), new Vector3(0, 8, 0), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
        bottomGradientRect.pivot = new Vector2(0.5f, 0f);
        bottomGradientRect.SetAsFirstSibling();        
        Image bottomImg = bottomGradientRect.AddComponent<Image>();
        Texture2D gradientBottomTexture = CreateGradientTexture(4, gradientBottomHeight, TextureAndGaphicsFunctions.MetalGoldColorGradient);
        Sprite.Create(gradientBottomTexture, new Rect(0, 0, 4, gradientBottomHeight), new Vector2(0.5f, 0.5f), 100.0f);
        bottomImg.sprite = Sprite.Create(gradientBottomTexture, new Rect(0, 0, 4, gradientBottomHeight), new Vector2(0.5f, 0.5f), 100.0f);
        AddToGeneratedObjectsDict(bottomGradientRect.gameObject.name, bottomGradientRect.gameObject);

        //Bottom scrolling text and it's clone
        txtRect = ApplicationController.Instance.UI.CreateRectTransformObject("Text_scroller", new Vector2(320f, 8), new Vector3(0, 24f, 0), Vector2.zero, Vector2.zero);
        txtRect.pivot = Vector2.zero;
        txt = TextFunctions.AddTextMeshProTextComponent(txtRect, "8-BIT_WONDER", 12, ApplicationController.Instance.C64PaletteArr[13]);
        txt.text = " ( SEE YOU ON THE OTHER SIDE )";        
        textWidth = txtRect.sizeDelta.x;
        //txtRect.anchoredPosition3D += new Vector3(0, 16f, 0); 
        startPosition = txtRect.anchoredPosition3D;
        //Clone
        txtClone = UnityEngine.Object.Instantiate(txt) as TextMeshProUGUI;
        txtRectClone = txtClone.GetComponent<RectTransform>();
        txtRectClone.SetParent(txtRect);
        txtRectClone.anchoredPosition3D = new Vector3(txtRect.anchoredPosition3D.x + txtRect.rect.width, 0, 0);
        txtRectClone.localScale = Vector3.one;        
        AddToGeneratedObjectsDict(txtRect.gameObject.name, txtRect.gameObject);

        //Top heading text
        headingRect = ApplicationController.Instance.UI.CreateRectTransformObject("Text_heading", new Vector2(60, 60), new Vector3(0, 0, 0), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
        headingRect.pivot = new Vector2(0.5f, 1f);
        headingTxt = TextFunctions.AddTextMeshProTextComponent(headingRect, "04B_19", 40, ApplicationController.Instance.C64PaletteArr[1]);
        headingTxt.alignment = TextAlignmentOptions.Center;
        headingTxt.text = "64";
        headingTxt.enableVertexGradient = true;
        headingTxt.colorGradient = new VertexGradient(
            ApplicationController.Instance.C64PaletteArr[1],
            ApplicationController.Instance.C64PaletteArr[1],
            ApplicationController.Instance.C64PaletteArr[0],
            ApplicationController.Instance.C64PaletteArr[0]);
        AddToGeneratedObjectsDict(headingRect.gameObject.name, headingRect.gameObject);

        Rect camRect = CameraFunctions.GetCameraRect(Camera.main, Camera.main.transform.position);
        //Ship sprite        
        shipRenderer = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("SpaceShip", new Vector3(camRect.xMin + .16f, camRect.center.y, 1f), GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("SpaceShipHorizontal")));
        shipRenderer.sortingOrder = 100000;
        AddToGeneratedObjectsDict(shipRenderer.gameObject.name, shipRenderer.gameObject);

        //Create star field
        InstantiateStarFieldSprites(30);

        return base.Init();
    }

    public override IEnumerator Run(System.Action callbackEnd)
    {
        yield return base.Run(callbackEnd);

        loopScroller = true;

        moveInput = Vector2.zero;

        ExecuteInUpdate = true;

        //Subscribe to input
        InputController.Instance.Fire1.Subscribe(b => HandleFireInput(b)).AddTo(Disposables);
        InputController.Instance.Horizontal.Subscribe(f => moveInput.x = f).AddTo(Disposables);
        InputController.Instance.Vertical.Subscribe(f => moveInput.y = f).AddTo(Disposables);

        Camera.main.backgroundColor = ApplicationController.Instance.C64PaletteArr[0];
        
        //Enable all generated objects        
        GeneratedObjectsSetActive(true);

        AudioController.Instance.PlayTrack("Track2", 1f, 4f);
        yield return AnimateSpriteScroll();        
    }

    public override void DoUpdate()
    {
        MoveShip(moveInput);
        base.DoUpdate();
    }

    public override void End(bool dispose = true)
    {
        moveInput = Vector2.zero;
        base.End(dispose);
    }

    private void MoveShip(Vector2 input)
    {
        //DOESN'T WORK CORRECTLY, FIX RECT!!

        Vector3 nextPosition = shipRenderer.transform.position + new Vector3(input.x * shipSpeed * Time.deltaTime, input.y * shipSpeed * Time.deltaTime, 0f);
        
        Rect rect = CameraFunctions.GetCameraRect(Camera.main, Camera.main.transform.position);
        rect.center += new Vector2(0, rect.height * 0.2f);
        rect.height *= 0.6f;

        if (nextPosition.y < rect.yMin || nextPosition.y > rect.yMax)
            nextPosition.y = shipRenderer.transform.position.y;

        if (nextPosition.x < rect.xMin || nextPosition.x > rect.xMax)
            nextPosition.x = shipRenderer.transform.position.x;

        shipRenderer.transform.position = nextPosition;

        Debug.DrawLine(new Vector3(rect.xMin, rect.yMax, 1f), new Vector3(rect.xMax, rect.yMax, 1f), Color.green);
        Debug.DrawLine(new Vector3(rect.xMin, rect.yMin, 1f), new Vector3(rect.xMax, rect.yMin, 1f), Color.red);
        
    }

    private void HandleFireInput(bool b)
    {
        if (!FirePressed && b)
        {
            InstantiateLaserShot(shipRenderer.transform.position);
            /*
            ApplicationController.Instance.FadeImageInOut(1f, ApplicationController.Instance.C64PaletteArr[0], () =>
            {
                //End the demo by exiting last coroutine and calling base.End();
                loopScroller = false;
                base.End();
            }, null);
            */
        }
        FirePressed = b;
    }

    private void InstantiateLaserShot(Vector3 pos)
    {
        SpriteRenderer laserRenderer = TextureAndGaphicsFunctions.InstantiateSpriteRendererGO("Laser", pos, GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("LaserGreen")));
        GenericBullet bullet = laserRenderer.AddComponent<GenericBullet>().Init(new Vector2(4f, 0), true);
    }



    private void InstantiateStarFieldSprites(int amount)
    {
        GameObject starsGO = new GameObject("stars");
        for (int i = 0; i < amount; i++)
        {
            Rect camRect = CameraFunctions.GetCameraRect(Camera.main, Camera.main.transform.position);
            GameObject go = new GameObject("star_" + i);
            go.transform.SetParent(starsGO.transform);
            go.transform.position = new Vector3(UnityEngine.Random.Range(camRect.xMin, camRect.xMax), UnityEngine.Random.Range(camRect.yMin, camRect.yMax), starsDistance);
            SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
            Sprite s = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>("white_32x32"));            
            spriteRenderer.sprite = s;
            spriteRenderer.color = ApplicationController.Instance.C64PaletteArr[UnityEngine.Random.Range(2, 5)];
            spriteRenderer.drawMode = SpriteDrawMode.Tiled;
            spriteRenderer.size = new Vector2(0.02f, 0.01f);
            spriteRenderer.sortingOrder = -1000 - i;
            stars.Add(go.transform);
            AddToGeneratedObjectsDict(go.name, go.gameObject);
        }
    }

    private Texture2D CreateGradientTexture(int w, int h, int[] gradientArray32)
    {
        Texture2D gradTexture = new Texture2D(w, h);
        gradTexture.filterMode = FilterMode.Point;

        for (int y = 0; y < gradTexture.height; y++)
        {
            for (int x = 0; x < gradTexture.width; x++)
            {
                gradTexture.SetPixel(x, y, ApplicationController.Instance.C64PaletteArr[gradientArray32[gradientArray32.Length - 1 - y]]);
            }
        }
        gradTexture.Apply();
        return gradTexture;
    }

    private void InstantiateGradientImages(int amount, (int x, int y) textureSize)
    {
        RectTransform imageGradient = ApplicationController.Instance.UI.CreateRectTransformObject("Image_gradient_0", new Vector2(320, gradientBarHeight), Vector3.zero, new Vector2(0, 0.5f), new Vector2(1, 0.5f), Vector2.zero, new Vector2(0, gradientBarHeight));
        imageGradient.SetAsFirstSibling();
        imageGradient.localPosition = initOffset;
        img = imageGradient.AddComponent<Image>();

        gradientTexture = CreateGradientTexture(textureSize.x, textureSize.y, TextureAndGaphicsFunctions.MetalDarkColorGradient);
        gradientSprite = Sprite.Create(gradientTexture, new Rect(0, 0, textureSize.x, gradientBarHeight), new Vector2(0.5f, 0.5f), 100.0f);
        img.sprite = gradientSprite;

        gradientImages.Add(img);

        for (int i = 1; i < amount - 1; i++)
        {
            RectTransform ig = UnityEngine.Object.Instantiate(imageGradient.gameObject).GetComponent<RectTransform>();
            ig.gameObject.name = "Image_gradient_" + i;
            ApplicationController.Instance.UI.ParentTransformToUI(ig.transform, null, new Vector3(0, -gradientBarHeight * i + initOffset.y, 0));
            UIController.SetLeft(ig, 0);
            UIController.SetRight(ig, 0);
            gradientImages.Add(ig.GetComponent<Image>());
        }

        gradientImages.ForEach(gi => AddToGeneratedObjectsDict(gi.gameObject.name, gi.gameObject));
    }

    private IEnumerator AnimateSpriteScroll()
    {
        int offset = 0;
        int[] offsets = new int[gradientImages.Count];

        while (loopScroller)
        {
            //Scroll top gradients
            offset++;
            for (int i = 0; i < gradientImages.Count; i++)
            {
                gradientSprite = Sprite.Create(
                    gradientTexture,
                    new Rect(0, offsets[i], 4, gradientBarHeight),
                    new Vector2(0.5f, 0.5f), 100f);

                gradientImages[i].sprite = gradientSprite;

                offsets[i] = offset + i;
                if (offsets[i] > gradientTexture.height - gradientBarHeight)
                    offsets[i] = i;
            }
            if (offset > gradientTexture.height - gradientBarHeight)
                offset = 0;

            //Scroll bottom text
            if (txtRect.anchoredPosition3D.x < -textWidth)
                txtRect.anchoredPosition3D = startPosition;
            txtRect.anchoredPosition3D += Vector3.left * scrollSpeed * Time.deltaTime;

            //Lerp heading color gradient
            float s = MathF.Sin(Time.time * 2f);
            float s2 = MathF.Sin(Time.time * 4f);
            Color color1 = Color.Lerp(ApplicationController.Instance.C64PaletteArr[0], ApplicationController.Instance.C64PaletteArr[1], (s + 1) * .5f);
            Color color2 = Color.Lerp(ApplicationController.Instance.C64PaletteArr[0], ApplicationController.Instance.C64PaletteArr[1], 1 - (s2 + 1) * .5f);            
            headingTxt.colorGradient = new VertexGradient(color1,color1, color2,color2);

            //Move stars
            Rect camRect = CameraFunctions.GetCameraRect(Camera.main, Camera.main.transform.position);
            stars.ForEach(star => 
            {
                star.Translate(Vector2.left * (starMoveSpeed + UnityEngine.Random.Range(0f, starMoveSpeed)) * Time.deltaTime * star.GetComponent<SpriteRenderer>().color.r);
                if (star.position.x < camRect.xMin)
                    star.position = new Vector3(camRect.width + UnityEngine.Random.Range(0f, 3f), star.position.y, starsDistance);
            });

            yield return new WaitForSeconds(0.07f);
        }
    }
}