using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UiMainMenuButton : Selectable, IPointerClickHandler, ISubmitHandler, ISelectHandler
{
    public event Action<UiMainMenuButton> OnPointerClickEvent;

    public enum MainMenuButtonType { Start, Continue, Options, Credits, Quit }
    public MainMenuButtonType ButtonType;
    public bool IsSelected = false;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        OnPointerClickEvent?.Invoke(this);
    }

    public void OnSubmit(BaseEventData eventData)
    {
        //throw new System.NotImplementedException();
        OnPointerClickEvent?.Invoke(this);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);

        Debug.Log("SELECTED: " + ButtonType);
    }

    protected override void Start()
    {
        base.Start();
        
        if (IsSelected)
            Select();
    }
}
