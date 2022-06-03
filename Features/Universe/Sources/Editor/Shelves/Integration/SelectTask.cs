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
				CreateLevelHelper.OnTaskAdded += PopulateTaskNames;
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
			_currentTaskIndex = Popup( _currentTaskIndex, _taskNames.ToArray(), Width( s_width ) );

			if( !EndChangeCheck() ) return;
			if( !saveInSettings ) return;

			var settings = USettingsHelper.GetSettings<LevelSettings>();
			var checkpoint = settings.m_editorCheckpoint;

			checkpoint.m_task = _currentLevel.GetGameplayTask( _currentTaskIndex );
			settings.SaveAsset();
		}

		#endregion


		#region Utils

		private static void UpdateLevel( string path, bool saveInSettings )
		{
			if( path.Equals( _currentPath ) && _currentLevel ) return;
			_currentPath = path;
			_currentLevel = LoadAssetAtPath<LevelData>( path );
			_currentTaskIndex = 0;
			PopulateTaskNames();

			if( !saveInSettings ) return;
			var settings = USettingsHelper.GetSettings<LevelSettings>();
			var checkpoint = settings.m_editorCheckpoint;

			_currentTaskIndex = _currentLevel.IndexOf( checkpoint.m_task );
			if( !_taskNames.GreaterThan( _currentTaskIndex ) )
				_currentTaskIndex = 0;

			checkpoint.m_level = _currentLevel;
			checkpoint.m_task = _currentLevel.GetGameplayTask( _currentTaskIndex );

			settings.SaveAsset();
		}

		private static void PopulateTaskNames()
		{
			var tasks = _currentLevel.m_gameplayTasks;

			_taskNames = new();
			foreach( var task in tasks )
			{
				var taskName = task.GetTrimmedName();
				_taskNames.Add( taskName );
			}
		}

		#endregion


		#region Private

		private static string _currentPath;
		private static LevelData _currentLevel;

		private static List<string> _taskNames;
		private static int _currentTaskIndex;
		private static bool _subcribedOnTaskLoaded;

		#endregion
	}
}