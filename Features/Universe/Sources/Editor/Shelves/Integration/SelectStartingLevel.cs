using System.Linq;
using UnityEngine;
using UnityEditor;
using Universe.SceneTask.Runtime;

using static System.IO.Path;
using static UnityEditor.AssetDatabase;
using static UnityEngine.GUILayout;

namespace Universe.Toolbar.Editor
{
	public class SelectStartingLevel
	{
		#region Main

		public static void Draw()
		{
			FindLevelDatas();

			var levelPath = PlayerPrefs.GetString(_playerPrefName);
			_currentLevel = FindAssociatedLevel(levelPath);

			Label("Current", Width(_labelWidth));
			_currentLevel = EditorGUILayout.Popup(_currentLevel, _levelNames, Width(_popupWidth));

			levelPath = _levelPaths[_currentLevel];
			PlayerPrefs.SetString(_playerPrefName, levelPath);
		}

		#endregion


		#region Utils

		private static int FindAssociatedLevel(string path)
		{
			if(string.IsNullOrEmpty(path)) return 0;

			var pathList 	= _levelPaths.ToList();
			var result 		= pathList.IndexOf(path);

			return result < 0 ? 0 : result; 
		}

		private static void FindLevelDatas()
		{
			var levels 	= FindAssets($"t:{typeof(LevelData)}");
			var size 	= levels.Length;
			
			_levelNames = new string[size];
			_levelPaths = new string[size];

			for (int i = 0; i < size; i++)
			{
				var level 	= levels[i];
				var path 	 = GUIDToAssetPath(level);
				var fullPath = GetFullPath(path);
				
				_levelPaths[i] = path;
				_levelNames[i] = GetFileNameWithoutExtension(fullPath);
			}
		}

		#endregion


		#region Private

		private static float _labelWidth = 50f;
		private static float _popupWidth = 175f;

		private static string[] _levelNames;
		private static string[] _levelPaths;

		private static int _currentLevel;
		private static string _levelPath;
		private static string _playerPrefName = "PlaymodeLevelPath";

		#endregion
	}
}