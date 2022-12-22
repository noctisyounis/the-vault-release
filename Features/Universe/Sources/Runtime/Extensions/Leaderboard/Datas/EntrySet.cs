namespace Universe.Leaderboard.Runtime
{
	public class EntrySet : SetGeneric<Entry>
	{
		public override void SortSet()
		{
			SortSet((entry1, entry2) =>
			{
				if (entry1.m_score == entry2.m_score) return 0;

				return (entry1.m_score > entry2.m_score) ? 1 : -1;
			});
		}
		
		public override void SortSetDescending()
		{
			SortSet((entry1, entry2) =>
			{
				if (entry1.m_score == entry2.m_score) return 0;

				return (entry1.m_score < entry2.m_score) ? 1 : -1;
			});
		}
	}
}
