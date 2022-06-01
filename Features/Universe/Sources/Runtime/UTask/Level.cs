using System;
using UnityEngine;

namespace Universe.SceneTask.Runtime
{
	public static class Level
	{
		#region Public

		public static LevelData m_currentLevel;

		public static Environment CurrentEnvironment
		{
			get => _currentEnvironment;
			set => _currentEnvironment = value;
		}

		public static TaskData CurrentAudioTask =>
			m_currentLevel.m_audio;
		public static TaskData CurrentBlockMeshTask =>
			m_currentLevel.m_blockMeshEnvironment;
		public static TaskData CurrentArtTask =>
			m_currentLevel.m_artEnvironment;
		public static TaskData CurrentGameplayTask =>
			m_currentLevel.m_gameplayTasks[_currentTaskIndex];
		public static int CurrentTaskIndex =>
			_currentTaskIndex;

		public static bool CanLoadArt => ( CurrentEnvironment & Environment.ART ) != 0;
		public static bool CanLoadBlockMesh => ( CurrentEnvironment & Environment.BLOCK_MESH ) != 0;
		public static bool IsFullyLoaded => _audioTaskLoaded && _blockMeshTaskLoaded && _artTaskLoaded && _gameplayTaskLoaded;


		#endregion


		#region Events

		public static Action<LevelData> OnLevelLoaded;

		#endregion


		#region Main

		///<summary>
		///Load all the compound tasks of the level.
		///</summary>

		public static void ULoadLevelAbsolute( this UBehaviour source, LevelData level ) =>
			source.ULoadLevelAbsolute( level, 0 );
		

		public static void ULoadLevelAbsolute( this UBehaviour source, LevelData level, int task )
		{
			if( HasCurrentLevel )
				source.UUnloadLevel( m_currentLevel );

			m_currentLevel = level;

			LoadAudioTask(source);
			TryLoadBlockMeshTask(source);
			TryLoadArtTask(source);
			LoadGameplayTask(source, task);
		}

		///<summary>
		///Load the tasks that are different from the current level.
		///</summary>

		public static void ULoadLevelOptimized( this UBehaviour source, LevelData level ) =>
			source.ULoadLevelOptimized( level, 0 );

		public static void ULoadLevelOptimized( this UBehaviour source, LevelData level, int task )
		{
			var audioTask = level.m_audio;
			var blockMeshTask = level.m_blockMeshEnvironment;
			var artTask = level.m_artEnvironment;
			var gameplayTask = level.m_gameplayTasks[task];

			var needAudio = false;
			var needBlockMesh = false;
			var needArt = false;

			if( !HasCurrentLevel )
			{
				needAudio = true;
				needBlockMesh = true;
				needArt = true;
			}
			else
			{
				if( !IsCurrentAudioEquals( audioTask ) )
				{
					source.UUnloadTask( CurrentAudioTask );
					needAudio = true;
				}
				if(!IsCurrentBlockMeshEquals( blockMeshTask ) )
				{
					source.UUnloadTask( CurrentBlockMeshTask );
					needBlockMesh = true;
				}
				if(!IsCurrentArtEquals( artTask ) )
				{
					source.UUnloadTask( CurrentArtTask );
					needArt = true;
				}

				source.UUnloadTask( CurrentGameplayTask );
			}

			m_currentLevel = level;

			if( needAudio )
				LoadAudioTask( source );
			if( needBlockMesh )
				TryLoadBlockMeshTask( source );
			if( needArt )
				TryLoadArtTask( source );

			LoadGameplayTask( source, task );
		}

		public static void UReloadCurrentLevelAbsolute( this UBehaviour source ) =>
			source.UReloadCurrentLevelAbsolute( 0 );

		public static void UReloadCurrentLevelAbsolute( this UBehaviour source, int at )
		{
			if( !HasCurrentLevel )
				return;

			source.ULoadLevelAbsolute( m_currentLevel, at);
		}

		public static void UReloadCurrentLevelOptimized( this UBehaviour source) =>
			source.UReloadCurrentLevelOptimized( 0 );

		public static void UReloadCurrentLevelOptimized( this UBehaviour source, int at )
		{
			if( !HasCurrentLevel )
				return;

			source.ULoadLevelOptimized( m_currentLevel, at );
		}

		public static void UUnloadLevel( this UBehaviour source, LevelData level )
		{
			if( !HasCurrentLevel )
				return;
			if( !level.Equals( m_currentLevel ) )
				throw new Exception( $"{level.name} isn't currently loaded" );

			var audioTask = CurrentAudioTask;
			var blockMeshEnvironmentTask = CurrentBlockMeshTask;
			var artEnvironmentTask = CurrentArtTask;
			var gameplayTasks = level.m_gameplayTasks;
			var gameplayTask = CurrentGameplayTask;

			source.UUnloadTask( audioTask );
			source.UUnloadTask( artEnvironmentTask );
			source.UUnloadTask( blockMeshEnvironmentTask );
			source.UUnloadTask( gameplayTask );

			if( gameplayTasks.GreaterThan( _previousLevelTaskIndex ))
			{
				var previousTask = level.m_gameplayTasks[_previousLevelTaskIndex];

				source.UUnloadTask( previousTask );
				_previousLevelTaskIndex = -1;
			}

			m_currentLevel = null;
		}

		public static void ULoadNextTask( this UBehaviour source )
		{
			if( !HasCurrentLevel )
				return;

			var nextLevelTask = _currentTaskIndex + 1;

			source.ULoadLevelTask( nextLevelTask );
		}

		public static void ULoadLevelTask( this UBehaviour source, int taskIndex )
		{
			if( !HasCurrentLevel )
				return;

			var currentLevelTaskIndex = _currentTaskIndex;
			var tasks = m_currentLevel.m_gameplayTasks;
			if( !tasks.GreaterThan( taskIndex ))
				return;

			var nextGameplayTask = tasks[taskIndex];

			source.ULoadTask( nextGameplayTask );

			_currentTaskIndex = taskIndex;
			_previousLevelTaskIndex = currentLevelTaskIndex;
		}

		public static void UReloadCheckpoint( this UBehaviour source )
		{
			if( !HasCurrentLevel )
				return;

			var tasks = m_currentLevel.m_gameplayTasks;
			if( !tasks.GreaterThan( _currentTaskIndex ) )
				return;

			var currentLevelTask = tasks[_currentTaskIndex];

			source.UUnloadTask( currentLevelTask );
			source.ULoadTask( currentLevelTask );
		}

		public static void UUnloadPreviousTask( this UBehaviour source ) =>
			source.UUnloadLevelTask( _previousLevelTaskIndex );

		public static void UUnloadLevelTask( this UBehaviour source, int taskIndex )
		{
			if( !HasCurrentLevel )
				return;
			var tasks = m_currentLevel.m_gameplayTasks;
			if( !tasks.GreaterThan( taskIndex ) )
				return;

			var gameplayTask = tasks[taskIndex];
			source.UUnloadTask( gameplayTask );

			if( taskIndex == _previousLevelTaskIndex )
				_previousLevelTaskIndex = -1;
		}


		#endregion


		#region Utils

		private static void LoadAudioTask( UBehaviour source )
		{
			_audioTaskLoaded = false;
			source.ULoadTask( m_currentLevel.m_audio );
			Task.OnTaskLoaded += OnAudioTaskLoaded;
		}

		private static void TryLoadBlockMeshTask(UBehaviour source)
		{
			if( CanLoadBlockMesh )
			{
				_blockMeshTaskLoaded = false;
				source.ULoadTask( m_currentLevel.m_blockMeshEnvironment );
				Task.OnTaskLoaded += OnBlockMeshTaskLoaded;
			}

			_blockMeshTaskLoaded = true;
		}

		private static void TryLoadArtTask( UBehaviour source )
		{
			if( CanLoadArt )
			{
				_artTaskLoaded = false;
				source.ULoadTask( m_currentLevel.m_artEnvironment );
				Task.OnTaskLoaded += OnArtTaskLoaded;
			}

			_artTaskLoaded = true;
		}

		private static void LoadGameplayTask( UBehaviour source, int index )
		{
			var tasks = m_currentLevel.m_gameplayTasks;
			if( !tasks.GreaterThan( index ) ) return;

			var current = tasks[index];

			_currentTaskIndex = index;
			_gameplayTaskLoaded = false;
			source.ULoadTask( current );
			Task.OnTaskLoaded += OnGameplayTaskLoaded;
		}

		private static void OnAudioTaskLoaded( TaskData audio )
		{
			var current = CurrentAudioTask;
			if( !audio.Equals( current ) )
				return;
			
			_audioTaskLoaded = true;
			Task.OnTaskLoaded -= OnAudioTaskLoaded;

			if( !IsFullyLoaded )
				return;

			OnLevelLoaded?.Invoke( m_currentLevel );
		}

		private static void OnBlockMeshTaskLoaded( TaskData environment )
		{
			var current = CurrentBlockMeshTask;
			if( !environment.Equals( current ) )
				return;

			_blockMeshTaskLoaded = true;
			Task.OnTaskLoaded -= OnBlockMeshTaskLoaded;

			if( !IsFullyLoaded )
				return;

			OnLevelLoaded?.Invoke( m_currentLevel );
		}

		private static void OnArtTaskLoaded( TaskData environment )
		{
			var current = CurrentArtTask;
			if( !environment.Equals( current ) )
				return;

			_artTaskLoaded = true;
			Task.OnTaskLoaded -= OnBlockMeshTaskLoaded;

			if( !IsFullyLoaded )
				return;

			OnLevelLoaded?.Invoke( m_currentLevel );
		}

		private static void OnGameplayTaskLoaded( TaskData gameplay )
		{
			var current = m_currentLevel.m_gameplayTasks[_currentTaskIndex];

			if( !gameplay.Equals( current ) )
				return;

			_gameplayTaskLoaded = true;
			Task.OnTaskLoaded -= OnGameplayTaskLoaded;

			if( !IsFullyLoaded )
				return;

			OnLevelLoaded?.Invoke( m_currentLevel );
		}

		private static bool IsCurrentAudioEquals( TaskData to )
		{
			if( !HasCurrentLevel )
				return false;

			var current = CurrentAudioTask;
			var currentAssetReference = current.m_assetReference;
			var otherAssetReference = to.m_assetReference;

			return currentAssetReference.Equals( otherAssetReference );
		}

		private static bool IsCurrentBlockMeshEquals( TaskData to )
		{
			if( !HasCurrentLevel )
				return false;

			var current = CurrentBlockMeshTask;
			var currentAssetReference = current.m_assetReference;
			var otherAssetReference = to.m_assetReference;

			return currentAssetReference.Equals( otherAssetReference );
		}

		private static bool IsCurrentArtEquals( TaskData to )
		{
			if( !HasCurrentLevel )
				return false;

			var current = CurrentArtTask;
			var currentAssetReference = current.m_assetReference;
			var otherAssetReference = to.m_assetReference;

			return currentAssetReference.Equals( otherAssetReference );
		}

		#endregion


		#region Private

		private static bool HasCurrentLevel => m_currentLevel;

		private static bool _audioTaskLoaded;
		private static bool _blockMeshTaskLoaded;
		private static bool _artTaskLoaded;
		private static bool _gameplayTaskLoaded;

		private static Environment _currentEnvironment;
		private static int _currentTaskIndex;
		private static int _previousLevelTaskIndex;

		#endregion
	}
}