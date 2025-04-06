using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UiMainMenuButton : Selectable, IPointerClickHandler, ISubmitHandler, ISelectHandler
{
    public event Action<UiMainMenuButton> OnPointerClickEvent;

    public enum MainMenuButtonType { Start, Continue, Options, Credits, Quit }
    public MainMenuButtonType ButtonType;
    public bool IsSelected = false;

    private TMP_Text buttonText => GetComponentInChildren<TMP_Text>();
    private string initText;
    
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
        buttonText.text = "[" + initText + "]";

        //Debug.Log("Button clikc");
        if (!IsSelected)
            AudioController.Instance.PlaySoundEffect("ClickSwitch");

        IsSelected = true;
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        buttonText.text = initText;

        IsSelected = false;
    }

    protected override void Start()
    {
        base.Start();

        initText = buttonText.text;

        if (IsSelected)
            Select();
    }
}
