using System;
using System.Collections;
using Universe.Leaderboard.Runtime;

namespace Universe.Stores.Runtime
{
	public abstract class IStoreProvider : UBehaviour
	{
		#region Lifecycle

		public abstract IEnumerator Initialize(Action<AsyncState> callback = null);
		public abstract void Tick();
		public abstract void ShutDown();
		
		#endregion
		
		
		#region Event

		public delegate void OnTrophyUnlockedHandler(int id);
		
		#endregion


		#region Entitlement
		
		public abstract void DoEntitlementCheck(Action<bool> callback = null);
		
		#endregion
		
		
		#region Leaderboard

		public abstract void PostScore(int toId, int value, Action<int> callback = null);
		public abstract void GetScore(int fromId, Action<Entry> callback = null);
		public abstract void GetRankings(int fromId, int amount, EntryAlignment alignment, Action<Entry[]> callback = null);

		#endregion
		
		
		#region Trophy

		public abstract void SubscribeOnTrophyUnlocked(OnTrophyUnlockedHandler handler);
		public abstract void UnlockTrophy(int id, Action callback = null);
		public abstract void SetTrophyProgress(int id, int value, Action<int> callback = null);
		public abstract void SetStatProgress(string statName, int value, Action<int> callback = null);

		#endregion
		
		
		#region Activity
		
		public abstract void StartActivity(string activityId, Action callback = null);
		public abstract void ResumeActivity(string activityId, string[] progressIds, string[] completedIds, Action callback = null);
		public abstract void CompleteActivity(string activityId, Action callback = null);
		public abstract void CancelActivity(string activityId, Action callback = null);
		public abstract void FailActivity(string activityId, Action callback = null);
		public abstract void UnlockActivities(string[] activityIds, Action callback = null);
		public abstract void LockActivities(string[] activityIds, Action callback = null);

		#endregion
	}
}