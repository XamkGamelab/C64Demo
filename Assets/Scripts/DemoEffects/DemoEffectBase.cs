using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;

public abstract class DemoEffectBase: IDemoEffect
{
    public ReactiveProperty<int> Score = new ReactiveProperty<int>(0);
    public ReactiveProperty<int> HiScore = new ReactiveProperty<int>(0);
    public ReactiveProperty<bool> Started = new ReactiveProperty<bool>(false);

    public float ParTime = 0f;
    public string TutorialText = "";

    public Dictionary<string, GameObject> GeneratedObjects = new Dictionary<string, GameObject>();
    public bool Initialized { get; private set; } = false;
    public bool ExecuteInUpdate { get; protected set; } = false;

    public System.Action EndDemoCallback;
    
    //Disposables for subscriptions like input
    protected CompositeDisposable Disposables = new CompositeDisposable();
    //Variables for basic input (left/right up/down and fire are used in most effects)
    protected bool FirePressed = false;
    protected float HorizontalInput = 0f;
    protected float VerticalInput = 0f;

    public virtual DemoEffectBase Init(float parTime, string tutorialText)
    {
        //Set par time and tutorial text
        ParTime = parTime;
        TutorialText = tutorialText;

        Initialized = true;
        return this;
    }

    public virtual void AddToGeneratedObjectsDict(string name, GameObject go)
    {
        go.SetActive(false);
        GeneratedObjects.Add(name, go);
    }

    public virtual IEnumerator Run(System.Action endDemoCallback)
    {
        Started.Value = true;
        Score.Value = 0;
        
        Disposables = new CompositeDisposable();
        EndDemoCallback = endDemoCallback;        
        yield return null;
    }

    public virtual void End(bool dispose = true) 
    {
        Started.Value = false;

        //Stop running updates
        ExecuteInUpdate = false;

        //Reset input values;
        HorizontalInput = VerticalInput = 0f;
        FirePressed = false;

        //Dispose disposables if dispose ;)
        if (dispose)
            Disposables?.Dispose();

        GeneratedObjectsSetActive(false);
        EndDemoCallback.Invoke();
    }
    public virtual void DoUpdate() { }
    public void GeneratedObjectsSetActive(bool active)
    {
        GeneratedObjects.ToList().ForEach(kvp => kvp.Value.SetActive(active));
    }

    public void GeneratedObjectsSetActive(bool active, List<string> exlude)
    {
        GeneratedObjects.ToList().Where(kvp => !exlude.Contains(kvp.Key)).ToList().ForEach(kvp => kvp.Value.SetActive(active));
    }
}
