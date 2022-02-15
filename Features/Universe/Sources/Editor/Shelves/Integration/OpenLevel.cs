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
	public class OpenLevel
	{
		#region Main

		public static void Draw()
		{
			var currentLevelPath 	= PlayerPrefs.GetString(_playerPrefName);
			var tex 				= EditorGUIUtility.IconContent(@"d_Project").image;

			if(!IsValidPath(currentLevelPath)) return;

			if(Button(new GUIContent("Open", tex, "Open selected level")))
			{
				Level.CurrentEditorEnvironment = Environment.BLOCK_MESH;

				var level 			= LoadAssetAtPath<LevelData>(currentLevelPath);
				var blockMeshGuid 	= level.m_blockMeshEnvironment.m_assetReference.AssetGUID;
				var gameplayGuid	= level.m_gameplay.m_assetReference.AssetGUID;
				var blockMesh 		= GUIDToAssetPath(blockMeshGuid);
				var gameplay		= GUIDToAssetPath(gameplayGuid);

				EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
				EditorSceneManager.OpenScene(gameplay, OpenSceneMode.Single);
				EditorSceneManager.OpenScene(blockMesh, OpenSceneMode.Additive);
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


		#endregion


		#region Private 

		private static string _playerPrefName = "PlaymodeLevelPath";

		#endregion

	}
}