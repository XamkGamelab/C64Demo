using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class PixelatedPalettePostProcessPass : ScriptableRenderPass
{
    private PixelatedPaletteRenderFeature.CustomPassSettings settings;

    //NOTE: If changed to RTHandles, should they be initialized?
    //https://docs.unity3d.com/Packages/com.unity.render-pipelines.core@12.0/manual/rthandle-system-using.html
    //Was this the cause of NRE with handles -> RTHandles.Initialize(Screen.width, Screen.height)

    // Used to render from camera to post processings
    // back and forth, until we render the final image to
    // the camera
    RenderTargetIdentifier source;
    RenderTargetIdentifier destinationA;
    RenderTargetIdentifier destinationB;
    RenderTargetIdentifier latestDest;

    private Material material => PixelatedPaletteMaterialPointer.Instance.Material;
    private PixelatedPaletteComponent pixelatedPaletteComponent => VolumeManager.instance.stack.GetComponent<PixelatedPaletteComponent>();

    private int pixelScreenHeight, pixelScreenWidth;

    readonly int temporaryRTIdA = Shader.PropertyToID("_TempRT");
    readonly int temporaryRTIdB = Shader.PropertyToID("_TempRTB");

    public PixelatedPalettePostProcessPass(PixelatedPaletteRenderFeature.CustomPassSettings settings)
    {
        this.settings = settings;
        // Set the render pass event
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        material.SetFloat(Shader.PropertyToID("_EffectIntensity"), pixelatedPaletteComponent.EffectIntensity.value);
        material.SetTexture(Shader.PropertyToID("_LUT"), pixelatedPaletteComponent.PaletteLookUpTexture.value);
        
        RTHandles.Initialize(Screen.width, Screen.height);

        // Grab the camera target descriptor. We will use this when creating a temporary render texture.
        RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
        descriptor.depthBufferBits = 0;

        var renderer = renderingData.cameraData.renderer;
        source = renderer.cameraColorTarget;

        pixelScreenHeight = pixelatedPaletteComponent.ResolutionPixelHeight.value;
        pixelScreenWidth = (int)(pixelScreenHeight * renderingData.cameraData.camera.aspect + 0.5f);

        material.SetVector("_BlockCount", new Vector2(pixelScreenWidth, pixelScreenHeight));
        material.SetVector("_BlockSize", new Vector2(1.0f / pixelScreenWidth, 1.0f / pixelScreenHeight));
        material.SetVector("_HalfBlockSize", new Vector2(0.5f / pixelScreenWidth, 0.5f / pixelScreenHeight));

        descriptor.height = Screen.height; //pixelScreenHeight;
        descriptor.width = Screen.width; //pixelScreenWidth;

        // Create a temporary render texture using the descriptor from above.
        cmd.GetTemporaryRT(temporaryRTIdA, descriptor, FilterMode.Point);
        destinationA = new RenderTargetIdentifier(temporaryRTIdA);
        cmd.GetTemporaryRT(temporaryRTIdB, descriptor, FilterMode.Point);
        destinationB = new RenderTargetIdentifier(temporaryRTIdB);
    }

    // The actual execution of the pass. This is where custom rendering occurs.
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (!pixelatedPaletteComponent.IsActive())
            return;

        CommandBuffer cmd = CommandBufferPool.Get("Doom Post Processing");
        cmd.Clear();

        // Starts with the camera source
        latestDest = source;

        BlitTo(cmd, material);        
        Blit(cmd, latestDest, source);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);

    }

    //Cleans the temporary RTs when we don't need them anymore
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        cmd.ReleaseTemporaryRT(temporaryRTIdA);
        cmd.ReleaseTemporaryRT(temporaryRTIdB);
    }

    private void BlitTo(CommandBuffer cmd, Material mat, int pass = 0)
    {
        var first = latestDest;
        var last = first == destinationA ? destinationB : destinationA;
        Blit(cmd, first, last, mat, pass);

        latestDest = last;
    }
}