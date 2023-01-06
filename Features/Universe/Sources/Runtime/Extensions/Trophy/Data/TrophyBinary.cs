using UnityEngine;

namespace Universe.Trophy.Runtime
{
	public class TrophyBinary : UniverseScriptableObject
	{
		#region Exposed

		public StringFact m_name;
		public StringFact m_description;
		public Sprite m_icon;
		public BoolFact m_unlocked;

		#endregion
		
		
		#region Main

		public virtual bool Evaluate() => m_unlocked;
		public virtual void Unlock() => m_unlocked.Value = true;
		public virtual void Lock() => m_unlocked.Value = false;
		public virtual FactBase GetValue() => m_unlocked;

		#endregion
	}
}