using UnityEngine;
using UnityEditor;
using Universe.SceneTask.Runtime;

using static System.IO.File;
using static System.IO.Path;
using static UnityEditor.AssetDatabase;
using static UnityEngine.GUILayout;

namespace Universe.Toolbar.Editor
{
	public class AddSituationToLevel
	{
		#region Main

		public static void Draw()
		{
			var tex = EditorGUIUtility.IconContent(@"d_Toolbar Plus").image;
			if( !Button( new GUIContent( "Situation", tex, "Add a situation to the selected level" ) ) )
				return;
			
			var currentLevelPath = PlayerPrefs.GetString(_playerPrefName);
			if(!IsValidPath(currentLevelPath))
			{
				Debug.LogError($"{currentLevelPath} isn't a valid level path");
				return;
			}
			var level = LoadAssetAtPath<LevelData>(currentLevelPath);
			CreateSituationWindow.ShowSituationWindow(level);
		}

		#endregion


		#region Utils

		private static bool IsValidPath(string path)
		{
			if( string.IsNullOrEmpty( path ) )
				return false;
			
			var fullPath = GetFullPath(path);

			return Exists(fullPath);
		}

		#endregion


		#region Private Members

		private static string _playerPrefName = "EditorLevelPath";

		#endregion
	}
}