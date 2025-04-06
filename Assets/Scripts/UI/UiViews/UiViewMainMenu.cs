using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UniRx.Triggers;
public class UiViewMainMenu : UiView
{
    public List<UiMainMenuButton> mainMenuButtons => GetComponentsInChildren<UiMainMenuButton>().ToList();

    protected override void Awake()
    {
        base.Awake();

        mainMenuButtons.ForEach(button => button.OnPointerClickEvent += HandleMenuButton);
        
        //credits is corner case which works with select/deselect logic
        UiMainMenuButton creditsButton = mainMenuButtons.Where(button => button.ButtonType == UiMainMenuButton.MainMenuButtonType.Credits).First();
        creditsButton.OnSelectEvent += _ => ApplicationController.Instance.ShowCredits(true);
        creditsButton.OnDeselectEvent += _ => ApplicationController.Instance.ShowCredits(false);
    }


    /// <summary>
    /// Extend UiView Show by selecting/deselecting the initially selected button.    
    /// </summary>
    /// <param name="_showViewWhenThisHides"></param>    
    public override void Show(UiView _showViewWhenThisHides = null)
    {
        base.Show(_showViewWhenThisHides);        
    }

    private void HandleMenuButton(UiMainMenuButton button)
    {
        Hide();

        switch (button.ButtonType)
        {
            case UiMainMenuButton.MainMenuButtonType.Start:
                ApplicationController.Instance.StartNewGame();
                break;
            case UiMainMenuButton.MainMenuButtonType.Quit:
                ApplicationController.Instance.QuitApp();
                break;
            default:
                Debug.LogWarning("Button functionality not implemented yet!");
                break;
        }
    }
}
