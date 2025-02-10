using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIController : SingletonMono<UIController>
{
    public Camera Cam => Camera.main;
    public Canvas UICanvas; //=> GetComponent<Canvas>();
    public UIController Init()
    {
        UICanvas = GetComponent<Canvas>();
        UICanvas.worldCamera = Cam;             
        return this;
    }

    public static Vector2? GetCanvasSize()
    {
        return Instance?.UICanvas?.GetComponent<RectTransform>().sizeDelta;
    }

    public RectTransform CreateRectTransformObject(string name, Vector2 size, Vector3 pos, Vector2 anchorMin, Vector2 anchorMax, Vector2? offsetMin = null, Vector2? offsetMax = null, Transform? parent = null)
    {
        RectTransform rectTransform = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rectTransform.SetParent(parent == null ? UICanvas.transform : parent);
        
        rectTransform.localScale = Vector3.one;
        rectTransform.anchoredPosition3D = pos;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;        
        rectTransform.sizeDelta = size;        

        if (offsetMin.HasValue)
            rectTransform.offsetMin = offsetMin.Value;
        if (offsetMax.HasValue)
            rectTransform.offsetMax = offsetMax.Value;

        return rectTransform;
    }

    public static Image AddResourcesImageComponent(RectTransform rectTransform, string spriteName, Color? color = null)
    {
        Image img = rectTransform.AddComponent<Image>();
        img.sprite = GameObject.Instantiate<Sprite>(Resources.Load<Sprite>(spriteName));
        img.color = color == null? Color.white : color.Value;
        return img;
    }

    public void ParentTransformToUI(Transform rectTransform, Transform parent = null, Vector3? position = null)
    {
        rectTransform.SetParent(parent == null ? UICanvas.transform : parent);
        rectTransform.localScale = Vector3.one;
        rectTransform.localPosition = position == null? Vector3.zero : position.Value;
    }

    public static void SetLeft(RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }
}
