using Universe.DebugWatch.Runtime;
using Universe.SceneTask.Runtime;

namespace Universe.DebugWatchTools.Runtime
{
	public static class ToolManager
	{
		#region Player
		#endregion


		#region Vision

		[DebugMenu( "Vision.../Bounding Box", "Display the bounding boxes", 200 )] public static void ToggleMeshBoundingBox() => MeshBoundingBox.ToggleDisplay();
		[DebugMenu( "Vision.../Collider", "Display the colliders", 200 )] public static void ToggleCollider() => CollidersOutline.ToggleDisplay();

		[DebugMenu( "Vision.../Overdraw", "Display the overdraw", 200 )] public static void ToggleOverdraw() => DebugViews.ChangeView( 0 );
		[DebugMenu( "Vision.../Shadow Cascades", "", 200 )] public static void ToggleShadowCascades() => DebugViews.ChangeView( 1 );
		[DebugMenu( "Vision.../UV0 Checker", "Display UV0", 200 )] public static void ToggleUV0Checker() => DebugViews.ChangeView( 2 );
		[DebugMenu( "Vision.../UV1 Checker", "Display UV01", 200 )] public static void ToggleUV1Checker() => DebugViews.ChangeView( 3 );
		[DebugMenu( "Vision.../Vertex Color A", "Display only Alpha shades", 200 )] public static void ToggleVertexColorA() => DebugViews.ChangeView( 4 );
		[DebugMenu( "Vision.../Vertex Color B", "Display only Blue shades", 200 )] public static void ToggleVertexColorB() => DebugViews.ChangeView( 5 );
		[DebugMenu( "Vision.../Vertex Color G", "Display only Green shades", 200 )] public static void ToggleVertexColorG() => DebugViews.ChangeView( 6 );
		[DebugMenu( "Vision.../Vertex Color R", "Display only Red shades", 200 )] public static void ToggleVertexColorR() => DebugViews.ChangeView( 7 );
		[DebugMenu( "Vision.../Vertex Color RGB", "Display Albedo", 200 )] public static void ToggleVertexColorRGB() => DebugViews.ChangeView( 8 );
		[DebugMenu( "Vision.../Wireframe", "Display meshes wireframe", 200 )] public static void ToggleWireframe() => DebugViews.ChangeView( 9 );
		[DebugMenu( "Vision.../World Normals", "Display meshes normals", 200 )] public static void ToggleWorldNormals() => DebugViews.ChangeView( 10 );
		[DebugMenu( "Vision.../World Tangents", "Display meshes tangents", 200 )] public static void ToggleWorldTangents() => DebugViews.ChangeView( 11 );

		[DebugMenuSelector("Vision.../Filter",
			new string[] { "Overdraw", "UV0", "World Normals", "World Tangents" },
			new object[] { 0, 2, 10, 11 },
			"Other visions in one", 200)]
		public static int ToggleFilter(int next)
		{
			DebugViews.ChangeView(next);
			return next;
		}

		#endregion


		#region Net
		#endregion


		#region Gameplay
		#endregion


		#region Profile

		[DebugMenu("Profile.../Framerate Counter", "Toggle FPS counter", 500)] public static void ToggleFPS() => ShowFPS.ToggleDisplay();

		#endregion


		#region Memory

		[DebugMenu("Memory.../Memory Profiler", "Toggle memory usage values", 600)] public static void ToggleMemoryProfiler() => MemoryProfiler.ToggleDisplay();
		[DebugMenu("Memory.../Render Profiler", "Toggle rendering values", 600)] public static void ToggleRenderProfiler() => RendererProfiler.ToggleDisplay();

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

		[DebugMenu( "Tasks.../Re-play Current", "Restart current situation", 1200 )] public static void ReloadGameplay() => 
			LevelManagement.ReloadGameplayTaskRequest();
		[DebugMenu( "Tasks.../Save Progress", "Save current state", 1200 )] public static void SaveDebug() =>
			LevelManagement.SaveDebugCheckpointRequest();
		[DebugMenu( "Tasks.../Load Progress", "Load saved state", 1200 )] public static void LoadDebug() =>
			LevelManagement.LoadDebugCheckpointRequest();

		[DebugMenu("Level.../Environment.../Toggle Block Mesh", "Load / Unload block mesh tasks", 1250)]
		public static bool ToggleBlockMesh()
		{
			LevelManagement.ToggleEnvironmentRequest( Environment.BLOCK_MESH );
			
			return (Situation.CurrentEnvironment & Environment.BLOCK_MESH) != 0;
		}

		[DebugMenu("Level.../Environment.../Toggle Art", "Load / Unload art tasks", 1250)]
		public static bool ToggleArt()
		{
			LevelManagement.ToggleEnvironmentRequest( Environment.ART );
			
			return (Situation.CurrentEnvironment & Environment.ART) != 0;
		}

		#endregion
	}
}