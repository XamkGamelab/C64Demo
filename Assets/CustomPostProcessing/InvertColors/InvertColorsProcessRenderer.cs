using UnityEngine.Rendering.Universal;

[System.Serializable]
public class InvertColorsProcessRenderer : ScriptableRendererFeature
{
    InvertColorsPostProcessPass pass;

    public override void Create()
    {
        pass = new InvertColorsPostProcessPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
}