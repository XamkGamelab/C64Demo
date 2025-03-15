using System.Collections;
using System.Collections.Generic;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine;
using UnityEngine.UI;
public class UiViewMainMenu : UiView
{
    public Button ButtonStartGame;

    protected override void Awake()
    {
        base.Awake();

        ButtonStartGame.onClick.AddListener(HandleStartButton);
    }

    /**
    * Extend UiView Show by selecting/deselecting the initially selected button.    
    */
    public override void Show(UiView _showViewWhenThisHides = null)
    {
        base.Show(_showViewWhenThisHides);

        /*
        if (currentlySelectedButton != null)
        {
            currentlySelectedButton.Select();
        }
        else if (initSelectedButton != null)
        {
            initSelectedButton.Select();
        }

        //Load screen shot images

        if (Saver.Instance.SaveSettings.Value != null)
            saveSlotButtons.ForEach(button => button.LoadScreenShotImage());
        */
    }

    private void HandleStartButton()
    {
        Debug.Log("START GAME BUTTON");
        ApplicationController.Instance.StartNewGame();
        Hide();
    }
}
