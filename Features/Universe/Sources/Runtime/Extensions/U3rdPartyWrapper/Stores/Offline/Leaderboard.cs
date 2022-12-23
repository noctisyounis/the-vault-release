using System;
using System.Collections.Generic;
using UnityEngine;
using Universe.Leaderboard.Runtime;

namespace Universe.Stores.Offline.Runtime
{
	public class Leaderboard : UBehaviour
	{
		#region Exposed

		[Header("Parameters")]
		public List<EntryListFact> m_leaderboards;
		public int m_maxEntry;

		#endregion
		
		
		#region Main

		public void PostScore(int id, Entry entry, Action<int> callback)
		{
			if (!m_leaderboards.GreaterThan(id))
			{
				VerboseError($"[Leaderboard] No board found for id: {id}");
				callback(-1);
				return;
			}

			Verbose($"[Leaderboard] Posted {entry.m_score}");
			var board = m_leaderboards[id];
			board.Add(entry);
			board.Sort();
			
			if (m_maxEntry > 0) Clean(board);

			Refresh(board);
		}

		public void GetRankings(int id, int amount, EntryAlignment alignment, Action<Entry[]> callback)
		{
			if (!m_leaderboards.GreaterThan(id))
			{
				VerboseError($"[Leaderboard] No board found for id: {id}");
				callback(default);
				return;
			}

			var board = m_leaderboards[id];
			var entries = board.ToArray();
			var entryAmount = entries.Length;
			var resultAmount = (entryAmount < amount) ? entryAmount : amount;
			var result = new Entry[resultAmount];
			
			for (var i = 0; i < resultAmount; i++)
			{
				result[i] = entries[i];
			}

			callback(result);
		}
		
		#endregion
		
		
		#region Utils

		private void Clean(EntryListFact board)
		{
			var count = board.Count;
			
			while (count > m_maxEntry)
			{
				count--;
				board.RemoveAt(count);	
			}
		}

		private void Refresh(EntryListFact board)
		{
			var count = board.Count;
			var rank = 0;

			while (rank < count)
			{
				var index = rank;
				var entry = board[index];

				rank++;
				entry.m_rank = rank;

				board[index] = entry;
			}
		}
		
		#endregion
	}
}
