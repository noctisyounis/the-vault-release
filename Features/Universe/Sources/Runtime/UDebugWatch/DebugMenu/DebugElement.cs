using UnityEngine;

namespace Universe.DebugWatch.Runtime
{
	public abstract class DebugElement : MonoBehaviour
	{
		public DebugPanel m_owner;
		public string m_path;

		public abstract void OnSelected();
		public abstract void OnDeselected();
	}
}