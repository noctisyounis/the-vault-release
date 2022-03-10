using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Universe.SceneTask.Runtime;

using static System.IO.File;
using static System.IO.Path;
using static UnityEditor.AssetDatabase;
using static UnityEngine.GUILayout;

namespace Universe.Toolbar.Editor
{
	public class SwitchLevel
	{
		#region Main

		public static void Draw()
		{
			var currentLevelPath 	= PlayerPrefs.GetString(_playerPrefName);
			var tex 				= EditorGUIUtility.IconContent(@"d_preAudioLoopOff").image;

			if(!IsValidPath(currentLevelPath)) return;

			var labelText = IsBlockMesh ? "Load Environment Art" : "Load Block Mesh";

			if(Button(new GUIContent(labelText, tex, "Switch between Block mesh and Art environment")))
			{
				Level.CurrentEditorEnvironment = IsBlockMesh ? Environment.ART : Environment.BLOCK_MESH;

				var level 			= LoadAssetAtPath<LevelData>(currentLevelPath);
				var blockMeshGuid 	= level.m_blockMeshEnvironment.m_assetReference.AssetGUID;
				var artGuid			= level.m_artEnvironment.m_assetReference.AssetGUID;
				var blockMesh 		= GUIDToAssetPath(blockMeshGuid);
				var art				= GUIDToAssetPath(artGuid);

				var unloadedPath 	= IsBlockMesh ? art : blockMesh;
				var unloaded 		= EditorSceneManager.GetSceneByPath(unloadedPath);
				var loaded 			= IsBlockMesh ? blockMesh : art;

				EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
				EditorSceneManager.CloseScene(unloaded, true);
				EditorSceneManager.OpenScene(loaded, OpenSceneMode.Additive);
			}
		}

		#endregion


		#region Utils

		private static bool IsValidPath(string path)
		{
			if(string.IsNullOrEmpty(path)) return false;
			
			var fullPath = GetFullPath(path);

			return Exists(fullPath);
		}

		private static bool IsBlockMesh => Level.CurrentEditorEnvironment == Environment.BLOCK_MESH;

		#endregion


		#region Private 

		private static string _playerPrefName = "PlaymodeLevelPath";

		#endregion
	}
}