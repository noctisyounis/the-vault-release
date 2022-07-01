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
using UnityEngine;
using UnityEngine.Rendering;

namespace URPDebugViews
{
	public partial class DebugViewRenderer
	{
		private const string BufferName = "Debug View";

		private readonly List<ShaderTagId> _shaderTagIdList = new List<ShaderTagId>();

		private readonly CommandBuffer _buffer = new CommandBuffer
		{
			name = BufferName
		};

		private ScriptableRenderContext _context;
		private Camera _camera;
		private CullingResults _cullingResults;
		private readonly Color ClearColor = new Color(0.1f, 0.1f, 0.1f);
		
		// copied from URP MainLightShadowCasterPass.cs
		private static readonly int CascadeShadowSplitSpheres0 = Shader.PropertyToID("_CascadeShadowSplitSpheres0");
		private static readonly int CascadeShadowSplitSpheres1 = Shader.PropertyToID("_CascadeShadowSplitSpheres1");
		private static readonly int CascadeShadowSplitSpheres2 = Shader.PropertyToID("_CascadeShadowSplitSpheres2");
		private static readonly int CascadeShadowSplitSpheres3 = Shader.PropertyToID("_CascadeShadowSplitSpheres3");
		private static readonly int CascadeShadowSplitSphereRadii = Shader.PropertyToID("_CascadeShadowSplitSphereRadii");
		
		private static readonly int DebugViewShadowDistances = Shader.PropertyToID("_DebugViewShadowDistances");

		public DebugViewRenderer()
		{
			_shaderTagIdList.Add(new ShaderTagId("UniversalForward"));
			_shaderTagIdList.Add(new ShaderTagId("LightweightForward"));
			_shaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
		}

		public void Render(ScriptableRenderContext context, Camera camera)
		{
			_context = context;
			_camera = camera;

			PrepareBuffer();
			PrepareForSceneWindow();
			if (!Cull())
			{
				return;
			}

			Setup();
			UpdateGlobalProperties();
			DrawVisibleGeometry();
			DrawUnsupportedShaders();
			DrawGizmos();
			Submit();
		}

		private bool Cull()
		{
			if (_camera.TryGetCullingParameters(out ScriptableCullingParameters p))
			{
				_cullingResults = _context.Cull(ref p);
				return true;
			}

			return false;
		}

		private void Setup()
		{
			_context.SetupCameraProperties(_camera);
			_buffer.ClearRenderTarget(true,true, ClearColor);
			_buffer.BeginSample(SampleName);
			ExecuteBuffer();
		}
		
		private void UpdateGlobalProperties()
		{
			Vector4 shadowDistances;
			DebugViewsManager.Instance.GetShadowsDistances(out shadowDistances);
			_buffer.SetGlobalVector(DebugViewShadowDistances, shadowDistances);
		}

		private void Submit()
		{
			_buffer.EndSample(SampleName);
			ExecuteBuffer();
			_context.Submit();
		}

		private void ExecuteBuffer()
		{
			_context.ExecuteCommandBuffer(_buffer);
			_buffer.Clear();
		}

		private void DrawVisibleGeometry()
		{
			var sortingSettings = new SortingSettings(_camera)
			{
				criteria = SortingCriteria.CommonOpaque
			};
			var drawingSettings = new DrawingSettings();
			drawingSettings.sortingSettings = sortingSettings;

			for (int i = 0; i < _shaderTagIdList.Count; i++)
				drawingSettings.SetShaderPassName(i, _shaderTagIdList[i]);

			var viewData = DebugViewsManager.Instance.CurrentViewData;
			drawingSettings.overrideMaterial = viewData ? viewData.Material : null;
			var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);

			_context.DrawRenderers(_cullingResults, ref drawingSettings, ref filteringSettings);

			sortingSettings.criteria = SortingCriteria.CommonTransparent;
			drawingSettings.sortingSettings = sortingSettings;
			filteringSettings.renderQueueRange = RenderQueueRange.transparent;

			_context.DrawRenderers(
				_cullingResults, ref drawingSettings, ref filteringSettings
			);
		}
	}
}