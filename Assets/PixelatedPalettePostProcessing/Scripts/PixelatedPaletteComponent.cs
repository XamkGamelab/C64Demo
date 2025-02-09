using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable ]
public class PixelatedPaletteComponent : VolumeComponent, IPostProcessComponent
{
    //Define a serialized field for the MaterialPropertyBlock
    [SerializeField]
    private MaterialPropertyBlock materialPropertyBlock;

    //Define a property to access the MaterialPropertyBlock
    public MaterialPropertyBlock MaterialPropertyBlock => materialPropertyBlock;
    public Texture2DParameter PaletteLookUpTexture = new(value: null);    
    public ClampedFloatParameter EffectIntensity = new ClampedFloatParameter(value: 0, min: 0, max: 1, overrideState: true);
    public IntParameter ResolutionPixelHeight = new IntParameter(value: 200);
    public bool IsTileCompatible() => true;

    public bool IsActive() => EffectIntensity.value > 0f;
    
}