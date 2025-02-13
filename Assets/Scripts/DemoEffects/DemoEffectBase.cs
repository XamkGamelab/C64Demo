using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UniRx;

public abstract class DemoEffectBase: IDemoEffect
{
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

    public virtual DemoEffectBase Init()
    {
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
        Disposables = new CompositeDisposable();
        EndDemoCallback = endDemoCallback;        
        yield return null;
    }

    public virtual void End(bool dispose = true) 
    {
        //Dispose disposables if dispose ;)
        if (dispose)
            Disposables?.Dispose();

        //Reset input values;
        HorizontalInput = VerticalInput = 0f;
        FirePressed = false;

        GeneratedObjectsSetActive(false);
        EndDemoCallback.Invoke();
    }
    public virtual void DoUpdate() { }
    public void GeneratedObjectsSetActive(bool active)
    {
        GeneratedObjects.ToList().ForEach(kvp => kvp.Value.SetActive(active));
    }
}
