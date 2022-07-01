using Universe.DebugWatch.Runtime;
using Universe.SceneTask.Runtime;

namespace Universe.DebugWatchTools.Runtime
{
	public static class ToolManager
	{
		#region Player
		#endregion


		#region Vision

		[DebugMenu( "Vision.../Bounding Box", "Display the bounding boxes" )] public static void ToggleMeshBoundingBox() => MeshBoundingBox.ToggleDisplay();
		[DebugMenu( "Vision.../Collider", "Display the colliders" )] public static void ToggleCollider() => CollidersOutline.ToggleDisplay();

		[DebugMenu( "Vision.../Overdraw", "Display the overdraw" )] public static void ToggleOverdraw() => DebugViews.ChangeView( 0 );
		[DebugMenu( "Vision.../Shadow Cascades" )] public static void ToggleShadowCascades() => DebugViews.ChangeView( 1 );
		[DebugMenu( "Vision.../UV0 Checker", "Display UV0" )] public static void ToggleUV0Checker() => DebugViews.ChangeView( 2 );
		[DebugMenu( "Vision.../UV1 Checker", "Display UV01" )] public static void ToggleUV1Checker() => DebugViews.ChangeView( 3 );
		[DebugMenu( "Vision.../Vertex Color A", "Display only Alpha shades" )] public static void ToggleVertexColorA() => DebugViews.ChangeView( 4 );
		[DebugMenu( "Vision.../Vertex Color B", "Display only Blue shades" )] public static void ToggleVertexColorB() => DebugViews.ChangeView( 5 );
		[DebugMenu( "Vision.../Vertex Color G", "Display only Green shades" )] public static void ToggleVertexColorG() => DebugViews.ChangeView( 6 );
		[DebugMenu( "Vision.../Vertex Color R", "Display only Red shades" )] public static void ToggleVertexColorR() => DebugViews.ChangeView( 7 );
		[DebugMenu( "Vision.../Vertex Color RGB", "Display Albedo" )] public static void ToggleVertexColorRGB() => DebugViews.ChangeView( 8 );
		[DebugMenu( "Vision.../Wireframe", "Display meshes wireframe" )] public static void ToggleWireframe() => DebugViews.ChangeView( 9 );
		[DebugMenu( "Vision.../World Normals", "Display meshes normals" )] public static void ToggleWorldNormals() => DebugViews.ChangeView( 10 );
		[DebugMenu( "Vision.../World Tangents", "Display meshes tangents" )] public static void ToggleWorldTangents() => DebugViews.ChangeView( 11 );

		#endregion


		#region Net
		#endregion


		#region Gameplay
		#endregion


		#region Profile

		[DebugMenu("Profile.../Framerate Counter", "Toggle FPS counter")] public static void ToggleFPS() => ShowFPS.ToggleDisplay();

		#endregion


		#region Memory

		[DebugMenu("Memory.../Memory Profiler", "Toggle memory usage values")] public static void ToggleMemoryProfiler() => MemoryProfiler.ToggleDisplay();
		[DebugMenu("Memory.../Render Profiler", "Toggle rendering values")] public static void ToggleRenderProfiler() => RendererProfiler.ToggleDisplay();

		#endregion


		#region Camera
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

		[DebugMenu( "Tasks.../Re-play Current", "Restart current situation" )] public static void ReloadGameplay() => 
			LevelManagement.ReloadGameplayTaskRequest();
		[DebugMenu( "Tasks.../Save Progress", "Save current state" )] public static void SaveDebug() =>
			LevelManagement.SaveDebugCheckpointRequest();
		[DebugMenu( "Tasks.../Load Progress", "Load saved state" )] public static void LoadDebug() =>
			LevelManagement.LoadDebugCheckpointRequest();
		[DebugMenu( "Level.../Environment.../Toggle Block Mesh", "Load / Unload block mesh tasks" )] public static void ToggleBlockMesh() =>
			LevelManagement.ToggleEnvironmentRequest( Environment.BLOCK_MESH );
		[DebugMenu( "Level.../Environment.../Toggle Art", "Load / Unload art tasks" )] public static void ToggleArt() =>
			LevelManagement.ToggleEnvironmentRequest( Environment.ART );

		#endregion
	}
}