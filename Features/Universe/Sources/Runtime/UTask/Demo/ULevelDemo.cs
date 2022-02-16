using UnityEngine;
using static UnityEngine.GUILayout;

namespace Universe.SceneTask.Runtime
{
	public class ULevelDemo : UBehaviour
	{
		#region Exposed

		public LevelData m_level1;
		public LevelData m_level2;

		#endregion


		#region Main

		private void OnGUI() 
		{
			BeginHorizontal(Width(200.0f));
			Label("Load Level 1 : ");
			if(Button("Absolute"))	LoadLevelAbsolute(m_level1);	
			if(Button("Optimized")) LoadLevelOptimized(m_level1);
			EndHorizontal();

			BeginHorizontal(Width(200.0f));
			Label("Load Level 2 : ");
			if(Button("Absolute"))	LoadLevelAbsolute(m_level2);	
			if(Button("Optimized")) LoadLevelOptimized(m_level2);
			EndHorizontal();

			if(Button($"Toggle to {(IsUsingArtEnvironment ? "block mesh" : "art")}"))
			{
				Level.CurrentEnvironment = IsUsingArtEnvironment ? Environment.BLOCK_MESH : Environment.ART;
			}

			if(Button("Reload Level")) ReloadCurrentLevelAbsolute();
			if(Button("Reload Gameplay")) ReloadCurrentLevelOptimized();
		}

		public void Start()
		{
			LoadLevelAbsolute(m_level1);
		}

		#endregion


		#region Private 

		private static bool IsUsingArtEnvironment => Level.CurrentEnvironment == Environment.ART;

		#endregion
	}
}