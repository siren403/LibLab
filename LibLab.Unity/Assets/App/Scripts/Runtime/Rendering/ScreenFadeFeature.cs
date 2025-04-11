using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class ScreenFadeFeature : ScriptableRendererFeature
{
    public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
    public Material passMaterial = null!;
    public int passIndex = 0;
    public bool fetchColorBuffer = true;

    private ScreenFadePass? _screenFadePass;

    public float Progress
    {
        set { _screenFadePass?.SetProgress(value); }
    }

    /// <inheritdoc/>
    public override void Create()
    {
        _screenFadePass = new ScreenFadePass(name);
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Preview
            || renderingData.cameraData.cameraType == CameraType.Reflection
            || UniversalRenderer.IsOffscreenDepthTexture(ref renderingData.cameraData))
            return;

        if (passMaterial == null)
        {
            return;
        }

        if (passIndex < 0 || passIndex >= passMaterial.passCount)
        {
            Debug.LogWarningFormat(
                "The full screen feature \"{0}\" will not execute - the pass index is out of bounds for the material.",
                name);
            return;
        }

        if (_screenFadePass != null)
        {
            _screenFadePass.renderPassEvent = renderPassEvent;
            // _screenFadePass.ConfigureInput(ScriptableRenderPassInput.Color);
            _screenFadePass.SetupMembers(passMaterial, passIndex);
            _screenFadePass.requiresIntermediateTexture = fetchColorBuffer;
            renderer.EnqueuePass(_screenFadePass);
        }
    }


    class ScreenFadePass : ScriptableRenderPass
    {
        private static readonly MaterialPropertyBlock _sharedPropertyBlock = new();
        private static readonly int _blitTexture = Shader.PropertyToID("_BlitTexture");
        private static readonly int _blitScaleBias = Shader.PropertyToID("_BlitScaleBias");
        private static readonly int _fadeProgress = Shader.PropertyToID("_Progress");

        private Material? _material;
        private int _passIndex;
        private float _progress;

        public ScreenFadePass(string passName)
        {
            profilingSampler = new ProfilingSampler(passName);
        }

        public void SetupMembers(Material material, int passIndex)
        {
            _material = material;
            _passIndex = passIndex;
        }

        public void SetProgress(float progress)
        {
            _progress = progress;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            UniversalResourceData resourcesData = frameData.Get<UniversalResourceData>();

            Debug.Assert(resourcesData.cameraColor.IsValid());

            var targetDesc = renderGraph.GetTextureDesc(resourcesData.cameraColor);
            targetDesc.name = "_CameraColorScreenFadePass";
            targetDesc.clearBuffer = false;

            TextureHandle source = resourcesData.activeColorTexture;
            TextureHandle destination = renderGraph.CreateTexture(targetDesc);
            using (var builder = renderGraph
                       .AddRasterRenderPass<CopyPassData>("Copy Color Screen Fade", out var passData, profilingSampler))
            {
                passData.inputTexture = source;
                // Read
                builder.UseTexture(passData.inputTexture);
                // Write
                builder.SetRenderAttachment(destination, 0);
                builder.SetRenderFunc(static (CopyPassData data, RasterGraphContext context) =>
                {
                    Blitter.BlitTexture(context.cmd, data.inputTexture, new Vector4(1, 1, 0, 0), 0.0f, false);
                });

                source = destination;
            }

            destination = resourcesData.activeColorTexture;

            using (var builder =
                   renderGraph.AddRasterRenderPass<MainPassData>(passName, out var passData, profilingSampler))
            {
                passData.material = _material!;
                passData.passIndex = _passIndex;
                passData.inputTexture = source;
                passData.progress = _progress;

                if (passData.inputTexture.IsValid())
                {
                    builder.UseTexture(passData.inputTexture);
                }

                bool needsColor = (input & ScriptableRenderPassInput.Color) != ScriptableRenderPassInput.None;

                if (needsColor)
                {
                    Debug.Assert(resourcesData.cameraOpaqueTexture.IsValid());
                    // require pass.ConfigureInput(ScriptableRenderPassInput.Color);
                    // reference FullScreenPassRendererFeature.cs
                    builder.UseTexture(resourcesData.cameraOpaqueTexture);
                }

                builder.SetRenderAttachment(destination, 0);

                builder.SetRenderFunc(static (MainPassData data, RasterGraphContext context) =>
                {
                    // ExecuteMainPass(rgContext.cmd, data.inputTexture, data.material, data.passIndex);

                    _sharedPropertyBlock.Clear();
                    _sharedPropertyBlock.SetTexture(_blitTexture, data.inputTexture);

                    // We need to set the "_BlitScaleBias" uniform for user materials with shaders relying on core Blit.hlsl to work
                    _sharedPropertyBlock.SetVector(_blitScaleBias, new Vector4(1, 1, 0, 0));

                    _sharedPropertyBlock.SetFloat(_fadeProgress, data.progress);

                    context.cmd.DrawProcedural(Matrix4x4.identity, data.material, data.passIndex,
                        MeshTopology.Triangles, 3, 1,
                        _sharedPropertyBlock);
                });
            }
        }
    }

    private class CopyPassData
    {
        internal TextureHandle inputTexture;
    }

    private class MainPassData
    {
        internal Material material;
        internal int passIndex;
        internal TextureHandle inputTexture;
        internal float progress;
    }
}
