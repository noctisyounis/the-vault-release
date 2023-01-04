using UnityEngine;

namespace Universe.Trophy.Runtime
{
	public class Trophy : UniverseScriptableObject
	{
		#region Exposed

		public StringFact m_name;
		public StringFact m_description;
		public Sprite m_icon;
		public bool m_unlocked;

		#endregion
		
		
		#region Main

		public virtual bool Evaluate() => m_unlocked;
		public virtual void Unlock() => m_unlocked = true;
		public virtual void Lock() => m_unlocked = false;

		#endregion
	}
}