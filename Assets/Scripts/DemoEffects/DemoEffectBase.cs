using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class DemoEffectBase
{
    public Dictionary<string, GameObject> GeneratedObjects = new Dictionary<string, GameObject>();
    public bool Initialized { get; private set; } = false;
    public bool ExecuteInUpdate { get; protected set; } = false;
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

    public virtual IEnumerator Run()
    {
        yield break;
    }

    public virtual void DoUpdate()
    {

    }
}
