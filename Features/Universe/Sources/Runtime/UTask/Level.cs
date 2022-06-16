using System;
using UnityEngine;

using static Universe.SceneTask.Runtime.LoadLevelMode;

namespace Universe.SceneTask.Runtime
{
    public static class Level
	{
		#region Exposed

		public static LevelData s_currentLevel;

		public static Environment CurrentEnvironment
		{
			get => _currentEnvironment;
			set => _currentEnvironment = value;
		}

		public static TaskData CurrentAudioTask =>
			s_currentLevel.m_audio;
		public static TaskData CurrentBlockMeshTask =>
			s_currentLevel.m_blockMeshEnvironment;
		public static TaskData CurrentArtTask =>
			s_currentLevel.m_artEnvironment;
		public static TaskData CurrentGameplayTask =>
			GetGameplayTask( CurrentTaskIndex );
		public static TaskData NextGameplayTask =>
			GetGameplayTask( CurrentTaskIndex + 1 );
		public static TaskData PreviousGameplayTask =>
			GetGameplayTask( CurrentTaskIndex - 1 );
		public static int CurrentTaskIndex =>
			_currentTaskIndex;

		public static bool CanLoadArt => ( CurrentEnvironment & Environment.ART ) != 0;
		public static bool CanLoadBlockMesh => ( CurrentEnvironment & Environment.BLOCK_MESH ) != 0;
		public static bool IsFullyLoaded => _audioTaskLoaded && _blockMeshTaskLoaded && _artTaskLoaded && AreAllGameplayLoaded;

		#endregion


		#region Events

		public static Action<LevelData> OnLevelLoaded;

		#endregion


		#region Public API

		public static TaskData GetGameplayTask( int index )
			=> s_currentLevel.GetGameplayTask( index );

		#endregion


		#region Main

		public static void ULoadLevel( this UBehaviour source, LevelData level, TaskData task = null)
		{
			s_currentLevel = level;
			if( !task )
				task = level.GetGameplayTask( 0 );

			source.TryLoadAudioTask();
			source.TryLoadBlockMeshTask();
			source.TryLoadArtTask();
			source.ULoadGameplayTask( task );
		}

		public static void UChangeLevel( this UBehaviour source, LevelData toLevel, TaskData toTask = null, LoadLevelMode mode = LoadAll )
		{
			if( s_currentLevel )
			{
				if( mode == LoadAll )
				{
					source.UUnloadLevel( s_currentLevel );
				}
				else
				{
					var audio = toLevel.m_audio;
					var blockMesh = toLevel.m_blockMeshEnvironment;
					var art = toLevel.m_artEnvironment;

					if( !IsCurrentAudioEquals( audio ) )
						source.UnloadAudioTask( s_currentLevel );
					if( !IsCurrentBlockMeshEquals( blockMesh ) )
						source.UnloadBlockMeshTask( s_currentLevel );
					if( !IsCurrentArtEquals( art ) )
						source.UnloadArtTask( s_currentLevel );

					source.UnloadGameplayTasks( s_currentLevel );
				}
			}

			source.ULoadLevel( toLevel, toTask );
		}

		public static void UReloadLevel( this UBehaviour source, LoadLevelMode mode = LoadLevelMode.LoadAll )
		{
			var level = s_currentLevel;
			var task = GetGameplayTask(0);

			source.UChangeLevel( level, task, mode );
		}

		public static void UUnloadLevel( this UBehaviour source, LevelData level )
		{
			source.UnloadAudioTask( level );
			source.UnloadBlockMeshTask( level );
			source.UnloadArtTask( level );
			source.UnloadGameplayTasks( level );

			_currentTaskIndex = 0;
		}

		public static void ULoadGameplayTask( this UBehaviour source, TaskData task )
		{
			var tasks = s_currentLevel.m_gameplayTasks;
			var index = s_currentLevel.IndexOf(task);
			if( index < 0 )
				return;

			var loaded = Task.GetLoadedScene(task).Scene.IsValid();
			if( loaded )
				return;

			_currentTaskIndex = index;
			_gameplayTaskRequestedAmount++;
			source.ULoadTask( task );

			if( IsSubscribed )
				return;
			Task.OnTaskLoaded += OnGameplayTaskLoaded;
		}

		public static void UUnloadGameplayTask( this UBehaviour source, TaskData task )
		{
			var loaded = Task.GetLoadedScene(task).Scene.IsValid();
			if( !loaded )
				return;

			source.UUnloadTask( task );

			_gameplayTaskLoadedAmount--;
			_gameplayTaskRequestedAmount--;
		}

		#endregion


		#region Utils

		private static void TryLoadAudioTask( this UBehaviour source )
		{
			var audio = s_currentLevel.m_audio;

			_audioTaskLoaded = Task.GetLoadedScene(audio).Scene.IsValid();
			if( _audioTaskLoaded )
				return;

			source.ULoadTask( audio );
			Task.OnTaskLoaded += OnAudioTaskLoaded;
		}

		private static void UnloadAudioTask( this UBehaviour source, LevelData of )
		{
			var audio = of.m_audio;

			source.UUnloadTask( audio );
		}

		private static void TryLoadBlockMeshTask( this UBehaviour source )
		{
			
			if( CanLoadBlockMesh )
			{
				var blockMesh = s_currentLevel.m_blockMeshEnvironment;

				_blockMeshTaskLoaded = Task.GetLoadedScene(blockMesh).Scene.IsValid();
				if( _blockMeshTaskLoaded )
					return;

				source.ULoadTask( blockMesh );
				Task.OnTaskLoaded += OnBlockMeshTaskLoaded;
				return;
			}

			_blockMeshTaskLoaded = true;
		}

		private static void UnloadBlockMeshTask( this UBehaviour source, LevelData of )
		{
			var blockMesh = of.m_blockMeshEnvironment;

			source.UUnloadTask( blockMesh );
		}

		private static void TryLoadArtTask( this UBehaviour source )
		{
			if( CanLoadArt )
			{
				var art = s_currentLevel.m_artEnvironment;
				_artTaskLoaded = Task.GetLoadedScene( art ).Scene.IsValid();

				if( _artTaskLoaded )
					return;

				source.ULoadTask( s_currentLevel.m_artEnvironment );
				Task.OnTaskLoaded += OnArtTaskLoaded;
				return;
			}

			_artTaskLoaded = true;
		}

		private static void UnloadArtTask( this UBehaviour source, LevelData of )
		{
			var art = of.m_artEnvironment;

			source.UUnloadTask( art );
		}

		private static void UnloadGameplayTasks( this UBehaviour source, LevelData of )
		{
			var tasks = of.m_gameplayTasks;

			foreach( var task in tasks )
				source.UUnloadTask( task );

			_gameplayTaskRequestedAmount = 0;
			_gameplayTaskLoadedAmount = 0;
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

			OnLevelLoaded?.Invoke( s_currentLevel );
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

			OnLevelLoaded?.Invoke( s_currentLevel );
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

			OnLevelLoaded?.Invoke( s_currentLevel );
		}

		private static void OnGameplayTaskLoaded( TaskData gameplay )
		{
			_gameplayTaskLoadedAmount++;

			if( !AreAllGameplayLoaded )
				return;

			Task.OnTaskLoaded -= OnGameplayTaskLoaded;

			if( !IsFullyLoaded )
				return;

			OnLevelLoaded?.Invoke( s_currentLevel );
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

		private static bool AreAllGameplayLoaded => 
			_gameplayTaskLoadedAmount == _gameplayTaskRequestedAmount;

		private static bool IsSubscribed
		{
			get
			{
				var action = Task.OnTaskLoaded;
				if( action is null )
					return false;

				var invocations = action.GetInvocationList();
				if( invocations is null )
					return false;

				return invocations.Length > 0;
			}
		}
		#endregion


		#region Private

		private static bool HasCurrentLevel => s_currentLevel;

		private static bool _audioTaskLoaded;
		private static bool _blockMeshTaskLoaded;
		private static bool _artTaskLoaded;
		private static int _gameplayTaskLoadedAmount;
		private static int _gameplayTaskRequestedAmount;

		private static Environment _currentEnvironment;
		private static int _currentTaskIndex;
		private static int _previousLevelTaskIndex;

		#endregion
	}
}