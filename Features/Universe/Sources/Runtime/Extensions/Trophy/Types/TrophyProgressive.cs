namespace Universe.Trophy.Runtime
{
	public class TrophyProgressive : Trophy
	{
		#region Exposed

		public IntFact m_value;
		public int m_target;

		#endregion
		
		
		#region Main

		public override bool Evaluate()
		{
			m_unlocked = m_value.Value >= m_target;

			return m_unlocked;
		}

		#endregion
	}
}