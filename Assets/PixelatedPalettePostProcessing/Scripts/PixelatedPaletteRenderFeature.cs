using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class PixelatedPaletteRenderFeature : ScriptableRendererFeature
{
    PixelatedPalettePostProcessPass pass;

    [System.Serializable]
    public class CustomPassSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;        
    }

    [SerializeField] private CustomPassSettings settings;

    public override void Create()
    {
        pass = new PixelatedPalettePostProcessPass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
}