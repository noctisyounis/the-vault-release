using System.Collections.Generic;
using UnityEngine;

namespace Universe.SceneTask.Runtime
{
	public class LevelData : ScriptableObject
	{
		#region Exposed

		public TaskData m_blockMeshEnvironment;
		public TaskData m_artEnvironment;
		public List<TaskData> m_gameplayTasks = new();

		#endregion
	}
}