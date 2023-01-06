namespace Universe.Trophy.Runtime
{
	public class TrophyProgressive : TrophyBinary
	{
		#region Exposed

		public IntFact m_value;
		public int m_target;

		#endregion
		
		
		#region Main

		public override bool Evaluate()
		{
			m_unlocked.Value = m_value.Value >= m_target;

			return m_unlocked.Value;
		}

		public override FactBase GetValue() => m_value;

		#endregion
	}
}