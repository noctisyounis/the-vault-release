using System.Linq;
using Universe.Editor;
using Universe.SceneTask.Runtime;

using static System.IO.Path;
using static UnityEditor.AssetDatabase;
using static UnityEditor.EditorGUILayout;
using static UnityEditor.EditorGUI;
using static UnityEngine.GUILayout;
using static UnityEngine.PlayerPrefs;

namespace Universe.Toolbar.Editor
{
	public class SelectLevel
	{
		#region Exposed

		public static float s_width = 100.0f;

		#endregion


		#region Main

		public static bool Draw( string playerPref ) =>
			Draw( "Level", playerPref );

		public static bool Draw( string label, string playerPref ) =>
			Draw( label, playerPref, false );
		public static bool Draw( string label, string playerPref, bool saveInSettings = false )
		{
			FindLevelDatas();

			if( _levelPaths.Length == 0 )
			{
				Label( "No existing level found." );
				return false;
			}

			var levelPath = GetString(playerPref);
			_currentLevel = FindAssociatedLevel( levelPath );

			if( string.IsNullOrEmpty( levelPath ) || _currentLevel < 0 )
			{
				_currentLevel = 0;
				levelPath = _levelPaths[_currentLevel];
				SetString( playerPref, levelPath );
			}

			BeginChangeCheck();

			Label( label );
			_currentLevel = Popup( _currentLevel, _levelNames, Width( s_width ) );
			levelPath = _levelPaths[_currentLevel];

			if( !EndChangeCheck() )
				return true;

			SetString( playerPref, levelPath );

			if( !saveInSettings )
				return true;

			var settings    = USettingsHelper.GetSettings<LevelSettings>();
			var level       = LoadAssetAtPath<LevelData>(levelPath);

			settings.m_startingLevel = level;
			settings.Save();

			return true;
		}

		#endregion


		#region Utils

		private static int FindAssociatedLevel( string path )
		{
			if( string.IsNullOrEmpty( path ) )
				return 0;

			var pathList    = _levelPaths.ToList();
			var result      = pathList.IndexOf(path);

			return result;
		}

		private static void FindLevelDatas()
		{
			var levels  = FindAssets($"t:{typeof(LevelData)}");
			var size    = levels.Length;

			_levelNames = new string[size];
			_levelPaths = new string[size];

			for( int i = 0; i < size; i++ )
			{
				var level       = levels[i];
				var path        = GUIDToAssetPath(level);
				var fullPath    = GetFullPath(path);

				_levelPaths[i] = path;
				_levelNames[i] = GetFileNameWithoutExtension( fullPath );
			}
		}

		#endregion


		#region Private

		private static string[] _levelNames;
		private static string[] _levelPaths;

		private static int _currentLevel;

		#endregion
	}
}