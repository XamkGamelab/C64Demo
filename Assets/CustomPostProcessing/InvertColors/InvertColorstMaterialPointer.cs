using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "InvertColorsMaterialPointer", menuName = "InvertColors/InvertColorsMaterialPointer")]
public class InvertColorsMaterialPointer : UnityEngine.ScriptableObject
{
    //---Your Materials---
    public Material InvertColorsMaterial;

    //---Accessing the data from the Pass---
    static InvertColorsMaterialPointer _instance;

    public static InvertColorsMaterialPointer Instance
    {
        get
        {
            if (_instance != null) return _instance;
            // TODO check if application is quitting
            // and avoid loading if that is the case

            _instance = Resources.Load("InvertColorsMaterialPointer") as InvertColorsMaterialPointer;
            return _instance;
        }
    }
}