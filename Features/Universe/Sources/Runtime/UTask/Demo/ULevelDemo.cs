using UnityEngine;
using static UnityEngine.GUILayout;

namespace Universe.SceneTask.Runtime
{
	public class ULevelDemo : UBehaviour
	{
		#region Exposed

		public LevelData m_level1;
		public LevelData m_level2;
		public CheckpointData m_runtimeCheckpoint;

		#endregion


		#region Main

		private void OnGUI()
		{
			BeginVertical();
			Label( $"Focused Task : {Task.GetFocusSceneName()}" );
			Label( $"Focused Priority : {Task.GetFocusPriority()}" );
			Label( $"TaskIndex : {Level.CurrentSituationIndex}" );
			DrawModeToggle();
			DrawLevelControl( m_level1 );
			DrawLevelControl( m_level2 );

			BeginHorizontal();
			DrawSaveCheckpointButton();
			DrawLoadCheckpointButton();
			EndHorizontal();
			EndVertical();
		}

		public void Start()
		{
			//LoadLevelAbsolute(m_level1);
		}

		#endregion


		#region Utils

		private void DrawModeToggle()
		{
			var next = (_currentMode == LoadLevelMode.LoadAll) ? LoadLevelMode.LoadMissingTasks : LoadLevelMode.LoadAll;

			BeginHorizontal();
			Label( $"Mode : {_currentMode}" );
			if( Button( $"Toggle to {next}" ) )
				_currentMode = next;
			EndHorizontal();
		}

		private void DrawLevelControl( LevelData level )
		{
			var situations = level.Situations;
			var count = situations.Count;

			BeginHorizontal();
			Label( $"{level.name}" );

			if( Level.s_currentLevel && Level.s_currentLevel.Equals( level ) )
			{
				for( var i = 0; i < count; i++ )
				{
					var situation = situations[i];
					DrawTaskButton( $"Task_{i + 1}", situation );
				}
			}
			else
			{
				for( var i = 0; i < count; i++ )
				{
					var situation = situations[i];
					if (!situation.m_isCheckpoint) continue;
					DrawLoadLevelButton( $"Task_{i + 1}", level, situation );
				}
			}
			
			EndHorizontal();
		}

		private void DrawTaskButton( string name, SituationData situation )
		{
			var gameplay = situation.m_gameplay;
			var loaded = Task.IsLoaded(gameplay);
			var nextStatus = loaded ? "Unload" : "Load";

			if( !Button( $"{nextStatus} {name}" ) )
				return;

			if( loaded )
				UnloadSituation( situation );
			else
				LoadSituation( situation );
		}

		private void DrawLoadLevelButton( string name, LevelData level, SituationData situation )
		{
			if( !Button( $"Load at {name}" ) )
				return;

			ChangeLevel( level, situation, _currentMode );
		}

		private void DrawSaveCheckpointButton()
		{
			if( !Button( "Save State" ) )
				return;

			m_runtimeCheckpoint.m_level = Level.s_currentLevel;
			m_runtimeCheckpoint.m_situation = Level.CurrentSituation;
		}

		private void DrawLoadCheckpointButton()
		{
			if( !Button( "Load State" ) )
				return;

			LoadCheckpoint( m_runtimeCheckpoint, _currentMode );
		}

		#endregion


		#region Private 

		private LoadLevelMode _currentMode = LoadLevelMode.LoadAll;

		#endregion
	}
}