using System;
using System.Collections;
using System.Collections.Generic;
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

		public Trophy m_trophy;
		
		#endregion
		
		
		#region Lifecycle

		public override IEnumerator Initialize(Action<AsyncState> callback = null)
		{
			try
			{
				_lastEntry = new Entry() { m_name = "" };
				callback?.Invoke(AsyncState.SUCCEED);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				callback?.Invoke(AsyncState.FAILED);
			}

			yield return 0;
		}

		public override void Tick()
		{}

		public override void ShutDown()
		{}
		
		#endregion
		
		
		#region Entitlement Check

		public override void DoEntitlementCheck(Action<bool> callback = null)
			=> callback?.Invoke(true);
		
		#endregion
		
		
		#region Leaderboard

		public override void PostScore(int toId, int value, Action<int> callback = null)
		{
			_lastEntry = new()
			{
				m_favourite = false,
				m_name = m_lastPlayer.Value,
				m_score = value,
				m_rank = 0
			};

			if (!m_leaderboard) return;

			m_leaderboard.PostScore(toId, _lastEntry, rank =>
			{
				_lastEntry.m_rank = rank;
				callback?.Invoke(rank);
			});
		}

		public override void GetScore(int fromId, Action<Entry> callback = null)
		{
			var result = (fromId == _lastBoard) ? _lastEntry : default;

			callback?.Invoke(result);
		}

		public override void GetRankings(int fromId, int amount, EntryAlignment alignment, Action<Entry[]> callback = null)
		{
			if (!m_leaderboard) return;
			
			m_leaderboard.GetRankings(fromId, amount, alignment, (entries) =>
			{
				if(fromId == _lastBoard) HighlightLastEntry(entries);

				callback?.Invoke(entries);
			});
			
		}

		#endregion
		
		
		#region Trophy

		public override void SubscribeOnTrophyUnlocked(OnTrophyUnlockedHandler handler)
		{
			if(m_trophy) Trophy.OnTrophyUnlocked += handler;
		}

		public override void UnlockTrophy(int id, Action callback = null)
		{
			if(m_trophy) m_trophy.UnlockTrophy(id, callback);
		}

		public override void SetTrophyProgress(int id, int value, Action<int> callback = null)
		{
			if(m_trophy) m_trophy.SetTrophyProgress(id, value, callback);
		}

		public override void SetStatProgress(string statName, int value, Action<int> callback = null)
		{
			if(m_trophy) m_trophy.SetStatProgress(statName, value, callback);
		}

		#endregion
		
		
		#region Activity
		
		public override void StartActivity(string activityId, Action callback = null) => callback?.Invoke();
		public override void ResumeActivity(string activityId, string[] progressIds, string[] completedIds, Action callback = null) => callback?.Invoke();
		public override void CompleteActivity(string activityId, Action callback = null) => callback?.Invoke();
		public override void FailActivity(string activityId, Action callback = null) => callback?.Invoke();
		public override void CancelActivity(string activityId, Action callback = null) => callback?.Invoke();
		public override void UnlockActivities(string[] activityIds, Action callback = null) => callback?.Invoke();
		public override void LockActivities(string[] activityIds, Action callback = null) => callback?.Invoke();
		
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
