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
			Label( $"{Level.CurrentTaskIndex}" );
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
			var tasks = level.m_gameplayTasks;
			var count = tasks.Count;

			BeginHorizontal();
			Label( $"{level.name}" );

			if( Level.s_currentLevel && Level.s_currentLevel.Equals( level ) )
			{
				for( var i = 0; i < count; i++ )
				{
					var task = tasks[i];
					DrawTaskButton( $"Task_{i + 1}", task );
				}
			}
			else
			{
				for( var i = 0; i < count; i++ )
				{
					var task = tasks[i];
					DrawLoadLevelButton( $"Task_{i + 1}", level, task );
				}
			}
			
			EndHorizontal();
		}

		private void DrawTaskButton( string name, TaskData task )
		{
			var loaded = Task.GetLoadedScene(task).Scene.IsValid();
			var nextStatus = loaded ? "Unload" : "Load";

			if( !Button( $"{nextStatus} {name}" ) )
				return;

			if( loaded )
				UnloadGameplayTask( task );
			else
				LoadGameplayTask( task );
		}

		private void DrawLoadLevelButton( string name, LevelData level, TaskData task )
		{
			if( !Button( $"Load at {name}" ) )
				return;

			ChangeLevel( level, task, _currentMode );
		}

		private void DrawSaveCheckpointButton()
		{
			if( !Button( "Save State" ) )
				return;

			m_runtimeCheckpoint.m_level = Level.s_currentLevel;
			m_runtimeCheckpoint.m_task = Level.CurrentGameplayTask;
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