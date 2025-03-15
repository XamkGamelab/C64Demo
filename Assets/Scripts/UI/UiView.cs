/**
 * Base class for all visible UI views
 */

using UnityEngine;
using UniRx;

public class UiView : MonoBehaviour
{
    public enum State { Visible, Hidden }
    public State DefaultState;

    public GameObject HideChildObject;
    public bool AllowToggle = true;
    public bool CloseWithCancel = false;
    public bool CloseWhenAnotherViewOpens = false;
    public ReactiveProperty<bool> IsOpen = new ReactiveProperty<bool>(false);

    private CompositeDisposable disposables = new CompositeDisposable();
    private UiView showViewWhenThisHides = null;

    protected virtual void Awake()
    {
        //Initially show or hide UIView depending on it's DefaultState
        if (DefaultState == State.Visible)
            Show();
        else
            Hide();

        if (CloseWithCancel)
            InputController.Instance.EscDown.Subscribe(c => { if (c) CancelInputClose(); }).AddTo(disposables);
    }

    private void OnDestroy()
    {
        disposables.Dispose();
    }

    /**
     * Reset UI views along with other IResetables.
     * This is only for some UI views that may be opened while playing, like the dialogue view! 
     * Otherwise state logic controls views visibility during the game.
     */
    public virtual void ResetState() 
    {
        StopAllCoroutines();
    }

    /**
     * Show UI view
     */
    public virtual void Show(UiView _showViewWhenThisHides = null)
    {
        showViewWhenThisHides = _showViewWhenThisHides;

        IsOpen.Value = true;
        if (HideChildObject == null)
            gameObject.SetActive(true);
        else
            HideChildObject.SetActive(true);
    }

    /**
     * Hide UI view
     */
    public virtual void Hide(bool _stopCoroutines = false)
    {
        if (!gameObject.activeSelf || HideChildObject != null && !HideChildObject.activeSelf)
            return;

        if (_stopCoroutines)
            StopAllCoroutines();

        IsOpen.Value = false;
        if (HideChildObject == null)
            gameObject.SetActive(false);
        else
            HideChildObject.SetActive(false);

        //TODO: Obfuscating and hazardous
        if (showViewWhenThisHides != null)
        {
            ApplicationController.Instance.UI.ShowUiView(showViewWhenThisHides, false);
            showViewWhenThisHides = null;
        }
    }

    /**
     * Toggle UI view active state
     */
    public virtual void Toggle()
    {
        if (!AllowToggle)
        {
            Debug.LogError("This is not set up to as a togglable menu");
            return;
        }

        if (IsOpen.Value)
        {
            Hide();
        }
        else
            Show();
    }

    protected virtual void CancelInputClose()
    {
        if (gameObject.activeInHierarchy)
            Hide(true);        
    }
}
