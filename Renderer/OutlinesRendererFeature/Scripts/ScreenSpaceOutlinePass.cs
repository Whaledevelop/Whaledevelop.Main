using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Whaledevelop.Rendering
{
    public class ScreenSpaceOutlinePass : ScriptableRenderPass
    {
        private readonly Material _outlineMaterial;
        private readonly Material _normalsMaterial;
        private readonly ScreenSpaceOutlineSettings _settings;

        private readonly List<ShaderTagId> _shaderTags;
        private FilteringSettings _filteringSettings;

        private RTHandle _normalsRT;
        private RTHandle _tempRT;

        private Color? _overridenColor;
        private float? _overridenScale;

        public ScreenSpaceOutlinePass(ScreenSpaceOutlineSettings settings)
        {
            _settings = settings;
            renderPassEvent = settings.renderPassEvent;

            _outlineMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/Outlines"));
            if (_outlineMaterial == null)
            {
                Debug.LogError("[OutlinePass] Failed to load 'Hidden/Outlines' shader");
            }

            _normalsMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/ViewSpaceNormals"));
            if (_normalsMaterial == null)
            {
                Debug.LogError("[OutlinePass] Failed to load 'Hidden/ViewSpaceNormals' shader");
            }
            
            _outlineMaterial?.SetColor("_OutlineColor", settings.outlineColor);
            _outlineMaterial?.SetFloat("_OutlineScale", settings.outlineScale);
            _outlineMaterial?.SetFloat("_DepthThreshold", settings.depthThreshold);
            _outlineMaterial?.SetFloat("_RobertsCrossMultiplier", settings.robertsCrossMultiplier);
            _outlineMaterial?.SetFloat("_NormalThreshold", settings.normalThreshold);
            _outlineMaterial?.SetFloat("_SteepAngleThreshold", settings.steepAngleThreshold);
            _outlineMaterial?.SetFloat("_SteepAngleMultiplier", settings.steepAngleMultiplier);

            _shaderTags = new List<ShaderTagId>
            {
                new("UniversalForward"),
                new("UniversalForwardOnly"),
                new("LightweightForward"),
                new("SRPDefaultUnlit")
            };

            _filteringSettings = new FilteringSettings(RenderQueueRange.opaque, settings.outlinesLayerMask);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = _settings.depthBufferBits;
            desc.colorFormat = _settings.colorFormat;

            //Debug.Log("[OutlinePass] Allocating normals and temp RTs");

            RenderingUtils.ReAllocateHandleIfNeeded(ref _normalsRT, desc, _settings.filterMode, TextureWrapMode.Clamp, name: "_OutlineNormalsRT");

            desc.depthBufferBits = 0;
            RenderingUtils.ReAllocateHandleIfNeeded(ref _tempRT, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_OutlineTempRT");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            //Debug.Log("[OutlinePass] Execute started");

            if (_outlineMaterial == null)
            {
                Debug.LogError("[OutlinePass] Missing outline material");
                return;
            }

            if (_normalsMaterial == null)
            {
                Debug.LogError("[OutlinePass] Missing normals material");
                return;
            }

            if (_normalsRT == null || _normalsRT.rt == null)
            {
                Debug.LogError("[OutlinePass] Normals RT is not allocated");
                return;
            }

            if (_tempRT == null || _tempRT.rt == null)
            {
                Debug.LogError("[OutlinePass] Temp RT is not allocated");
                return;
            }

            if (renderingData.cameraData.renderer.cameraColorTargetHandle.rt == null)
            {
                //Debug.LogError("[OutlinePass] Camera color target is null (possibly due to RenderGraph)");
                return;
            }

            var cmd = CommandBufferPool.Get("Screen Space Outline");

            using (new ProfilingScope(cmd, new ProfilingSampler("Outline Normals")))
            {
                var drawSettings = CreateDrawingSettings(_shaderTags, ref renderingData, SortingCriteria.CommonOpaque);
                drawSettings.overrideMaterial = _normalsMaterial;
                drawSettings.perObjectData = _settings.perObjectData;
                drawSettings.enableInstancing = _settings.enableInstancing;
                drawSettings.enableDynamicBatching = _settings.enableDynamicBatching;

                cmd.SetRenderTarget(_normalsRT, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
                cmd.ClearRenderTarget(true, true, _settings.backgroundColor);

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                //Debug.Log("[OutlinePass] Drawing normals");

                var rendererListParams = new RendererListParams(renderingData.cullResults, drawSettings, _filteringSettings);
                var rendererList = context.CreateRendererList(ref rendererListParams);
                cmd.DrawRendererList(rendererList);

                cmd.SetGlobalTexture("_SceneViewSpaceNormals", _normalsRT);
            }

            using (new ProfilingScope(cmd, new ProfilingSampler("Outline Blit")))
            {
                //Debug.Log("[OutlinePass] Blitting outlines");

                Blitter.BlitCameraTexture(cmd, renderingData.cameraData.renderer.cameraColorTargetHandle, _tempRT, _outlineMaterial, 0);
                Blitter.BlitCameraTexture(cmd, _tempRT, renderingData.cameraData.renderer.cameraColorTargetHandle);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);

            //Debug.Log("[OutlinePass] Execute completed");
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }

        public void Release()
        {
            CoreUtils.Destroy(_outlineMaterial);
            CoreUtils.Destroy(_normalsMaterial);
            _normalsRT?.Release();
            _tempRT?.Release();
        }
    }
}
