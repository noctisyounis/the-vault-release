using UnityEngine;

namespace Universe.SceneTask.Runtime
{
	public class LevelData : ScriptableObject
	{
		#region Exposed

		public TaskData m_blockMeshEnvironment;
		public TaskData m_artEnvironment;
		public TaskData m_gameplay;

		#endregion
	}
}