using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIController : MonoBehaviour /*SingletonMono<UIController>*/
{
    public Camera Cam => Camera.main;
    public Canvas UILoResCanvas;
    public Canvas UIHiResCanvas;
    public List<UiLayerRoot> GuiLayerRoots = new List<UiLayerRoot>();

    private UiViewsList UiViews;
    private List<UiView> UiViewInstances = new List<UiView>();

    public UIController Init()
    {
        UILoResCanvas.worldCamera = Cam;

        //Instantiate all the UI views from UiViewsList component
        UiViews = GetComponent<UiViewsList>();
        UiViewInstances = InstantiateViews(UiViews);

        return this;
    }

    public Vector2? GetCanvasSize()
    {
        return UILoResCanvas.GetComponent<RectTransform>().sizeDelta;
    }

    //Hi-res UI view management
    private List<UiView> InstantiateViews(UiViewsList uiViews)
    {
        List<UiView> viewInstances = new List<UiView>();

        uiViews.Views.ForEach(x =>
        {
            UiView view = Instantiate<UiView>(x.View);
            viewInstances.Add(view);
            ParentUiObjectToLayerRoot(x.UiLayer, view.gameObject);
        });

        return viewInstances;
    }

    public UiView ShowUiView(UiView view, bool _hideOtherViews = true, UiView _showViewWhenThisHides = null)
    {
        if (view != null)
        {
            if (view.AllowToggle)
                view.Toggle();
            else
                view.Show(_showViewWhenThisHides);

            //Hide all other views
            if (_hideOtherViews)
                UiViewInstances.Where(viewInstance => viewInstance != view).ToList().ForEach(itemToClose => itemToClose.Hide());
            //Or only views that are marked to hide when other views open
            else
                UiViewInstances.Where(viewInstance => viewInstance != view && viewInstance.CloseWhenAnotherViewOpens).ToList().ForEach(itemToClose => itemToClose.Hide());

        }

        return view;
    }

    public UiView ShowUiView<T>(UiView _showViewWhenThisHides = null, bool _hideOtherViews = true)
    {
        UiView view = UiViewInstances.First(viewInstance => viewInstance.GetType() == typeof(T));
        if (view == null)
            Debug.LogError("UiView with \"" + typeof(T) + "\" not found.");

        return ShowUiView(view, _hideOtherViews, _showViewWhenThisHides);
    }

    public T GetUiView<T>()
    {
        return UiViewInstances.Select(v => v).OfType<T>().FirstOrDefault();
    }

    private GameObject GetUiLayerRootObject(UiElement.ElementLayers layer)
    {
        return GuiLayerRoots.Single(x => x.UiLayer == layer).LayerRoot;
    }

    private void ParentUiObjectToLayerRoot(UiElement.ElementLayers uiLayer, GameObject guiObject)
    {
        guiObject.transform.SetParent(GetUiLayerRootObject(uiLayer).transform);
        guiObject.transform.localScale = Vector3.one;
        guiObject.transform.localPosition = Vector3.zero;
        guiObject.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        guiObject.GetComponent<RectTransform>().offsetMin = Vector2.zero;
    }

    public void HideAllUiViews()
    {
        UiViewInstances.ForEach(itemToClose => itemToClose.Hide(true));
    }
    //MOVE ALL BELOW TO UIFunctions scripts and they should all be static
    public RectTransform CreateRectTransformObject(string name, Vector2 size, Vector3 pos, Vector2 anchorMin, Vector2 anchorMax, Vector2? offsetMin = null, Vector2? offsetMax = null, Transform? parent = null)
    {
        RectTransform rectTransform = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
        rectTransform.SetParent(parent == null ? UILoResCanvas.transform : parent);
        
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
        rectTransform.SetParent(parent == null ? UILoResCanvas.transform : parent);
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
