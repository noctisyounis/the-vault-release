using System.Collections.Generic;
using UnityEngine;
using Universe;
using Universe.Editor;
using Universe.SceneTask.Runtime;

using static UnityEditor.AssetDatabase;
using static UnityEditor.EditorGUILayout;
using static UnityEditor.EditorGUI;
using static UnityEngine.GUILayout;
using static UnityEngine.PlayerPrefs;

namespace Universe.Toolbar.Editor
{
	public class SelectTask
	{
		#region Exposed

		public static float s_width = 150.0f;

		#endregion


		#region Main

		public static void Draw( string levelPlayerPref ) =>
			Draw( "Task", levelPlayerPref );
		public static void Draw( string label, string levelPlayerPref ) =>
			Draw( label, levelPlayerPref, false );
		public static void Draw( string label, string levelPlayerPref, bool saveInSettings = false )
		{
			if( !_subcribedOnTaskLoaded )
			{
				CreateLevelHelper.OnSituationAdded += PopulateTaskNames;
				_subcribedOnTaskLoaded = true;
			}

			var levelPath = GetString( levelPlayerPref );
			UpdateLevel( levelPath, saveInSettings );

			if( _taskNames.Count < 1 )
			{
				PopulateTaskNames();
			}

			BeginChangeCheck();

			Label( label );
			_currentSituationIndex = Popup( _currentSituationIndex, _taskNames.ToArray(), Width( s_width ) );

			if( !EndChangeCheck() ) return;
			if( !saveInSettings ) return;

			var settings = USettingsHelper.GetSettings<LevelSettings>();
			var checkpoint = settings.m_editorCheckpoint;

			checkpoint.m_situation = _currentLevel.GetSituation( _currentSituationIndex );
			settings.SaveAsset();
		}

		#endregion


		#region Utils

		private static void UpdateLevel( string path, bool saveInSettings )
		{
			if( path.Equals( _currentPath ) && _currentLevel ) return;
			_currentPath = path;
			_currentLevel = LoadAssetAtPath<LevelData>( path );
			_currentSituationIndex = 0;
			PopulateTaskNames();

			if( !saveInSettings ) return;
			var settings = USettingsHelper.GetSettings<LevelSettings>();
			var checkpoint = settings.m_editorCheckpoint;

			_currentSituationIndex = _currentLevel.IndexOf( checkpoint.m_situation );
			if( !_taskNames.GreaterThan( _currentSituationIndex ) )
				_currentSituationIndex = 0;

			checkpoint.m_level = _currentLevel;
			checkpoint.m_situation = _currentLevel.GetSituation( _currentSituationIndex );

			settings.SaveAsset();
		}

		private static void PopulateTaskNames()
		{
			var situations = _currentLevel.Situations;

			if (situations is null) return;

			_taskNames = new();
			foreach( var situation in situations )
			{
				var taskName = situation.m_name;
				_taskNames.Add( taskName );
			}
		}

		#endregion


		#region Private

		private static string _currentPath;
		private static LevelData _currentLevel;

		private static List<string> _taskNames;
		private static int _currentSituationIndex;
		private static bool _subcribedOnTaskLoaded;

		#endregion
	}
}