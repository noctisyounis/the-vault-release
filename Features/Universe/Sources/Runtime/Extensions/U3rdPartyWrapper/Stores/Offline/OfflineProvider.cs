using System;
using UnityEngine;
using Universe.Leaderboard.Runtime;
using Universe.Stores.Runtime;

namespace Universe.Stores.Offline.Runtime
{
	public class OfflineProvider : IStoreProvider
	{
		#region Exposed

		[Header("References")] 
		public Leaderboard m_leaderboard;
		public StringFact m_lastPlayer;
		
		#endregion
		
		
		#region Lifecycle

		public override bool Initialize()
		{
			_lastEntry = new Entry() { m_name = "" };
			return true;
		}

		public override void Tick()
		{}

		public override void ShutDown()
		{}
		
		#endregion
		
		
		#region Entitlement Check

		public override void DoEntitlementCheck(Action<bool> callback)
			=> callback(true);
		
		#endregion
		
		
		#region Leaderboard

		public override void PostScore(int toId, long value, Action<int> callback)
		{
			_lastEntry = new()
			{
				m_favourite = false,
				m_name = m_lastPlayer.Value,
				m_score = value,
				m_rank = 0
			};

			m_leaderboard.PostScore(toId, _lastEntry, rank =>
			{
				_lastEntry.m_rank = rank;
				callback(rank);
			});
		}

		public override void GetScore(int fromId, Action<Entry> callback)
		{
			var result = (fromId == _lastBoard) ? _lastEntry : default;

			callback(result);
		}

		public override void GetRankings(int fromId, int amount, EntryAlignment alignment, Action<Entry[]> callback)
		{
			m_leaderboard.GetRankings(fromId, amount, alignment, (entries) =>
			{
				if(fromId == _lastBoard) HighlightLastEntry(entries);

				callback(entries);
			});
			
		}

		#endregion
		
		
		#region Utils

		private void HighlightLastEntry(Entry[] entries)
		{
			var amount = entries.Length;

			for (var i = 0; i < amount; i++)
			{
				var entry = entries[i];
				var isLast = _lastEntry.Equals(entry);

				if (!isLast) continue;

				entry.m_favourite = true;
				entries[i] = entry;
				break; 
			}
		}
		
		#endregion
		
		
		#region Private

		private int _lastBoard;
		private Entry _lastEntry;

		#endregion
	}
}
