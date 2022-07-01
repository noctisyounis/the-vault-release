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

using UnityEngine;
using UnityEngine.Rendering;

namespace URPDebugViews
{
	public class DebugViewRenderPipeline : RenderPipeline
	{
		private readonly DebugViewRenderer _renderer = new DebugViewRenderer();

		public DebugViewRenderPipeline()
		{
			// For compatibility reasons we also match old LightweightPipeline tag.
			Shader.globalRenderPipeline = "UniversalPipeline,LightweightPipeline";
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			
			Shader.globalRenderPipeline = "";
		}

		protected override void Render(
			ScriptableRenderContext context, Camera[] cameras
		)
		{
			foreach (var camera in cameras)
			{
				_renderer.Render(context, camera);
			}
		}
	}
}