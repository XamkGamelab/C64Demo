using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "PixelatedPaletteMaterialPointer", menuName = "PixelatedPalette/PixelatedPaletteMaterialPointer")]
public class PixelatedPaletteMaterialPointer : UnityEngine.ScriptableObject
{
    //---Your Materials---
    public Material Material;

    //---Accessing the data from the Pass---
    static PixelatedPaletteMaterialPointer _instance;

    public static PixelatedPaletteMaterialPointer Instance
    {
        get
        {
            if (_instance != null) return _instance;
            // TODO check if application is quitting
            // and avoid loading if that is the case

            _instance = Resources.Load("PixelatedPaletteMaterialPointer") as PixelatedPaletteMaterialPointer;
            return _instance;
        }
    }
}