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

using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace URPDebugViews
{
	partial class DebugViewRenderer
	{
		partial void DrawGizmos();

		partial void DrawUnsupportedShaders();

		partial void PrepareForSceneWindow();

		partial void PrepareBuffer();

#if UNITY_EDITOR

		private static readonly ShaderTagId[] _legacyShaderTagIds =
		{
			new ShaderTagId("Always"),
			new ShaderTagId("ForwardBase"),
			new ShaderTagId("PrepassBase"),
			new ShaderTagId("Vertex"),
			new ShaderTagId("VertexLMRGBM"),
			new ShaderTagId("VertexLM")
		};

		private static Material _errorMaterial;

		private string SampleName { get; set; }

		partial void DrawGizmos()
		{
			if (Handles.ShouldRenderGizmos())
			{
				_context.DrawGizmos(_camera, GizmoSubset.PreImageEffects);
				_context.DrawGizmos(_camera, GizmoSubset.PostImageEffects);
			}
		}

		partial void DrawUnsupportedShaders()
		{
			if (_errorMaterial == null)
			{
				_errorMaterial =
					new Material(Shader.Find("Hidden/InternalErrorShader"));
			}

			var drawingSettings = new DrawingSettings(
				_legacyShaderTagIds[0], new SortingSettings(_camera)
			)
			{
				overrideMaterial = _errorMaterial
			};
			for (int i = 1; i < _legacyShaderTagIds.Length; i++)
			{
				drawingSettings.SetShaderPassName(i, _legacyShaderTagIds[i]);
			}

			var filteringSettings = FilteringSettings.defaultValue;
			_context.DrawRenderers(
				_cullingResults, ref drawingSettings, ref filteringSettings
			);
		}

		partial void PrepareForSceneWindow()
		{
			if (_camera.cameraType == CameraType.SceneView)
			{
				ScriptableRenderContext.EmitWorldGeometryForSceneView(_camera);
			}
		}

		partial void PrepareBuffer()
		{
			Profiler.BeginSample("Editor Only");
			_buffer.name = SampleName = _camera.name;
			Profiler.EndSample();
		}

#else

	const string SampleName = BufferName;

#endif
	}
}