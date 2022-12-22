using System;

namespace Universe.Leaderboard.Runtime
{
	[Serializable]
	public struct Entry
	{
		#region Exposed

		public bool m_favourite;
		public string m_name;
		public long m_score;
		public int m_rank;

		#endregion
		
		
		#region Utils

		public override string ToString() => $"[{m_rank}] {m_name}: {m_score} (highlight: {m_favourite})";

		public override bool Equals(object obj)
		{
			if (obj is null) return false;
			if (obj is not Entry entry) return false;

			var sameName = m_name.Equals(entry.m_name);
			var sameScore = m_score.Equals(entry.m_score);

			return sameName && sameScore;
		}
		
		public override int GetHashCode()
		{
			return HashCode.Combine(m_name, m_score);
		}
		#endregion
	}
}