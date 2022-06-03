using System;
using System.Collections.Generic;

namespace Universe.SceneTask.Runtime
{
	public class LevelData : UniverseScriptableObject
	{
		#region Exposed

		public TaskData m_audio;
		public TaskData m_blockMeshEnvironment;
		public TaskData m_artEnvironment;
		public List<TaskData> m_gameplayTasks = new();

		#endregion


		#region Public API

		public int IndexOf( TaskData gameplayTask )
			=> m_gameplayTasks.IndexOf( gameplayTask );

		public TaskData GetGameplayTask( int index )
		{
			if( !m_gameplayTasks.GreaterThan( index ) )
				return null;

			return m_gameplayTasks[index];
		}

        #endregion
    }
}