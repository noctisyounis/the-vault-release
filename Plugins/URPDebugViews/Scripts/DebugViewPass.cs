//
// URP Debug Views for Unity
// (c) 2019 PH Graphics
// Source code may be used and modified for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 
// *** A NOTE ABOUT PIRACY ***
// 
// If you got this asset from a pirate site, please consider buying it from the Unity asset store. This asset is only legally available from the Unity Asset Store.
// 
// I'm a single indie dev supporting my family by spending hundreds and thousands of hours on this and other assets. It's very offensive, rude and just plain evil to steal when I (and many others) put so much hard work into the software.
// 
// Thank you.
//
// *** END NOTE ABOUT PIRACY ***
//

using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

namespace URPDebugViews
{
    public class DebugViewPass : ScriptableRenderPass
    {
        private readonly List<ShaderTagId> _shaderTagIdList = new List<ShaderTagId>();

        private const string ProfilerTag = "Debug View Pass";

        private RenderStateBlock _renderStateBlock;
        private FilteringSettings _filteringSettings;

        public DebugViewPass()
        {
            _filteringSettings = new FilteringSettings(RenderQueueRange.all);

            _shaderTagIdList.Add(new ShaderTagId("UniversalForward"));
            _shaderTagIdList.Add(new ShaderTagId("LightweightForward"));
            _shaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit")); 
            
            _renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);

            // might be exposed later on, at the moment it's used only by wireframe
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var viewData = DebugViewsManager.Instance.CurrentViewData;

            if (!viewData)
                return;

            CommandBuffer cmd = CommandBufferPool.Get(ProfilerTag);
            cmd.BeginSample(ProfilerTag);
            
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;
            DrawingSettings drawingSettings =
                CreateDrawingSettings(_shaderTagIdList, ref renderingData, sortingCriteria);
            drawingSettings.overrideMaterialPassIndex = 0;
            drawingSettings.overrideMaterial = viewData ? viewData.Material : null;

            // debug view drawing
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings,
                ref _renderStateBlock);

            cmd.EndSample(ProfilerTag);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
