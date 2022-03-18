using UnityEngine;
using Universe.DebugWatch.Runtime;

namespace Universe.DebugWatchTools.Runtime
{
	public static class ToolManager
	{
		#region Player
		#endregion


		#region Net
		#endregion


		#region Gameplay
		#endregion


		#region Profile

		[DebugMenu("Profile.../Framerate Counter")] public static void ToggleFPS() => ShowFPS.ToggleDisplay();

		#endregion


		#region Memory

		[DebugMenu("Memory.../Memory Profiler")] public static void ToggleMemoryProfiler() => MemoryProfiler.ToggleDisplay();
		[DebugMenu("Memory.../Render Profiler")] public static void ToggleRenderProfiler() => RendererProfiler.ToggleDisplay();

		#endregion


		#region Camera

		[DebugMenu("Vision.../Bounding Box")] public static void ToggleMeshBoundingBox() => MeshBoundingBox.ToggleDisplay();
		[DebugMenu("Vision.../Collider")] public static void ToggleCollider() => CollidersOutline.ToggleDisplay();

		#endregion


		#region Clock
		#endregion


		#region Audio
		#endregion


		#region Scripts
		#endregion


		#region Levels
		#endregion


		#region Tasks
		#endregion
	}
}