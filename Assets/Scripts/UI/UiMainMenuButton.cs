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
    public event Action<UiMainMenuButton> OnSelectEvent;
    public event Action<UiMainMenuButton> OnDeselectEvent;

    public enum MainMenuButtonType { Start, Continue, Options, Credits, Quit }
    public MainMenuButtonType ButtonType;
    public bool IsSelected = false;

    private TMP_Text buttonText => GetComponentInChildren<TMP_Text>();
    private string initText;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        OnPointerClickEvent?.Invoke(this);
    }

    public void OnSubmit(BaseEventData eventData)
    {
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

        OnSelectEvent?.Invoke(this);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        buttonText.text = initText;

        IsSelected = false;

        OnDeselectEvent?.Invoke(this);
    }

    protected override void Start()
    {
        base.Start();

        initText = buttonText.text;

        if (IsSelected)
            Select();
    }
}
