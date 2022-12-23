namespace Universe.Leaderboard.Runtime
{
	public class EntryListFact : ListFactGeneric<Entry>
	{
		#region Main
		
		public override void Sort()
		{
			Value.Sort((entry1, entry2) =>
			{
				if (entry1.m_score == entry2.m_score) return 0;

				return (entry1.m_score > entry2.m_score) ? 1 : -1;
			});
		}
		
		#endregion
	}
}
