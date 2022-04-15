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
		
		#endregion


		#region Main

		///<summary>
		///Load all the compound tasks of the level.
		///</summary>
		public static void ULoadLevelAbsolute(this UBehaviour source, LevelData level)
		{
			var environmentTask = IsUsingArtEnvironment ? level.m_artEnvironment : level.m_blockMeshEnvironment;
			var gameplayTask = level.m_gameplayTasks[0];

			if(HasCurrentLevel)	source.UUnloadLevel(m_currentLevel);

			source.ULoadTask(environmentTask);
			source.ULoadTask(gameplayTask);

			m_currentLevel = level;
			_currentLevelTask = 0;
		}

		///<summary>
		///Load the tasks that are different from the current level.
		///</summary>
		public static void ULoadLevelOptimized(this UBehaviour source, LevelData level)
		{
			var environmentTask = IsUsingArtEnvironment ? level.m_artEnvironment : level.m_blockMeshEnvironment;
			var gameplayTask = level.m_gameplayTasks[_currentLevelTask];

			if(IsLoadingCurrentEnvironment(environmentTask)) 
			{
				source.UUnloadTask(m_currentLevel.m_gameplayTasks[_currentLevelTask]);
			}
			else
			{
				source.UUnloadLevel(m_currentLevel);
				source.ULoadTask(environmentTask);
			}
			
			source.ULoadTask(gameplayTask);

			m_currentLevel = level;
			_currentLevelTask = 0;
		}

		public static void UReloadCurrentLevelAbsolute(this UBehaviour source)
		{
			if(!HasCurrentLevel) return;

			source.ULoadLevelAbsolute(m_currentLevel);
		}

		public static void UReloadCurrentLevelOptimized(this UBehaviour source)
		{
			if(!HasCurrentLevel) return;
			
			source.ULoadLevelOptimized(m_currentLevel);
		}

		public static void UUnloadLevel(this UBehaviour source, LevelData level)
		{
			if(!HasCurrentLevel) return;

			var artEnvironmentTask = level.m_artEnvironment;
			var blockMeshEnvironmentTask = level.m_blockMeshEnvironment;
			var gameplayTask = level.m_gameplayTasks[_currentLevelTask];

			source.UUnloadTask(artEnvironmentTask);
			source.UUnloadTask(blockMeshEnvironmentTask);
			source.UUnloadTask(gameplayTask);
			
			if(IsValidLevelTask(_previousLevelTask))
			{
				var previousTask = level.m_gameplayTasks[_previousLevelTask];

				source.UUnloadTask(previousTask);
			}
			
			m_currentLevel = null;
		}

		public static void ULoadNextTask(this UBehaviour source)
        {
            if (!HasCurrentLevel) return;

            var nextLevelTask = _currentLevelTask + 1;
			
			source.ULoadLevelTask(nextLevelTask);
        }

		public static void ULoadLevelTask(this UBehaviour source, int taskIndex)
		{
			if (!HasCurrentLevel) return;

            var currentLevelTask = _currentLevelTask;

            if (!IsValidLevelTask(taskIndex)) return;

            var nextGameplayTask 	= m_currentLevel.m_gameplayTasks[taskIndex];

			source.ULoadTask(nextGameplayTask);

			_currentLevelTask = taskIndex;
			_previousLevelTask = currentLevelTask;
		}

		public static void UReloadCheckpoint(this UBehaviour source)
		{
			if(!HasCurrentLevel) return;

			var currentLevelTask = m_currentLevel.m_gameplayTasks[_currentLevelTask];

			source.UUnloadTask(currentLevelTask);
			source.ULoadTask(currentLevelTask);
		}

		public static void UUnloadPreviousTask(this UBehaviour source)
		{
			if(!HasCurrentLevel) return;
			if(!IsValidLevelTask(_previousLevelTask)) return;

			var previousGameplayTask = m_currentLevel.m_gameplayTasks[_previousLevelTask];

			source.UUnloadTask(previousGameplayTask);
			_previousLevelTask = -1;
		}


        #endregion


        #region Utils
        private static bool IsLoadingCurrentEnvironment(TaskData environment) 
		{
			if(!HasCurrentLevel) return false;

			var current = IsUsingArtEnvironment ? m_currentLevel.m_artEnvironment : m_currentLevel.m_blockMeshEnvironment;

			return current.m_assetReference.Equals(environment.m_assetReference);
		}

        private static bool IsValidLevelTask(int nextLevelTask) => ((nextLevelTask >= 0) && (nextLevelTask < m_currentLevel.m_gameplayTasks.Count));

		#endregion


		#region Private

		private static bool IsUsingArtEnvironment 	=> CurrentEnvironment == Environment.ART;
		private static bool HasCurrentLevel			=> m_currentLevel;

		private static Environment _currentEnvironment;
		private static int _currentLevelTask;
		private static int _previousLevelTask;

		#endregion
	}
}