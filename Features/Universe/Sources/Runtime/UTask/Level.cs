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

		public static Environment CurrentEditorEnvironment
		{
			get => _currentEditorEnvironment;
			set => _currentEditorEnvironment = value;
		}

		#endregion


		#region Main

		///<summary>
		///Load all the compound tasks of the level.
		///</summary>
		public static void ULoadLevelAbsolute(this UBehaviour source, LevelData level)
		{
			var environmentTask = IsUsingArtEnvironment ? level.m_artEnvironment : level.m_blockMeshEnvironment;
			var gameplayTask = level.m_gameplay;

			if(HasCurrentLevel)	source.UUnloadLevel(m_currentLevel);

			source.ULoadTask(environmentTask);
			source.ULoadTask(gameplayTask);

			m_currentLevel = level;
		}

		///<summary>
		///Load the tasks that are different from the current level.
		///</summary>
		public static void ULoadLevelOptimized(this UBehaviour source, LevelData level)
		{
			var environmentTask = IsUsingArtEnvironment ? level.m_artEnvironment : level.m_blockMeshEnvironment;
			var gameplayTask = level.m_gameplay;

			if(IsLoadingCurrentEnvironment(environmentTask)) 
			{
				source.UUnloadTask(m_currentLevel.m_gameplay);
			}
			else
			{
				source.UUnloadLevel(m_currentLevel);
				source.ULoadTask(environmentTask);
			}
			
			source.ULoadTask(gameplayTask);

			m_currentLevel = level;
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
			var gameplayTask = level.m_gameplay;

			source.UUnloadTask(artEnvironmentTask);
			source.UUnloadTask(blockMeshEnvironmentTask);
			source.UUnloadTask(gameplayTask);

			m_currentLevel = null;
		}

		#endregion


		#region Utils
		private static bool IsLoadingCurrentEnvironment(TaskData environment) 
		{
			if(!HasCurrentLevel) return false;

			var current = IsUsingArtEnvironment ? m_currentLevel.m_artEnvironment : m_currentLevel.m_blockMeshEnvironment;

			return current.m_assetReference.Equals(environment.m_assetReference);
		}

		#endregion


		#region Private

		private static bool IsUsingArtEnvironment 	=> CurrentEnvironment == Environment.ART;
		private static bool HasCurrentLevel			=> m_currentLevel;

		private static Environment _currentEnvironment;
		private static Environment _currentEditorEnvironment;

		#endregion
	}
}