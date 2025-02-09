using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable ]
public class InvertColorsComponent : VolumeComponent, IPostProcessComponent
{
    // Define a serialized field for the MaterialPropertyBlock
    [SerializeField]
    private MaterialPropertyBlock materialPropertyBlock;

    // Define a property to access the MaterialPropertyBlock
    public MaterialPropertyBlock MaterialPropertyBlock => materialPropertyBlock;

    public NoInterpColorParameter MultiplyColor        = new NoInterpColorParameter(Color.white);
    public ClampedFloatParameter InvertColorsIntensity = new ClampedFloatParameter(value: 0, min: 0, max: 1, overrideState: true);
    public bool IsTileCompatible() => true;

    public bool IsActive() => InvertColorsIntensity.value > 0f || MultiplyColor.value != Color.white;
    
}