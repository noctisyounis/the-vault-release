using System;
using Universe.Leaderboard.Runtime;

namespace Universe.Stores.Runtime
{
	public abstract class IStoreProvider : UBehaviour
	{
		#region Lifecycle

		public abstract bool Initialize();
		public abstract void Tick();
		public abstract void ShutDown();
		
		#endregion


		#region Entitlement
		
		public abstract void DoEntitlementCheck(Action<bool> callback);
		
		#endregion
		
		
		#region Leaderboard

		public abstract void PostScore(int toId, long value, Action<int> callback);
		public abstract void GetScore(int fromId, Action<Entry> callback);
		public abstract void GetRankings(int fromId, int amount, EntryAlignment alignment, Action<Entry[]> callback);

		#endregion
		
		
		#region Achievement

		public abstract void SubscribeOnTrophyUnlocked(Action<int> target);
		public abstract void UnlockTrophy(int id, Action callback);
		public abstract void SetTrophyProgress(int id, long value, Action<long> callback);
		public abstract void SetStatProgress(string stat, long value, Action<long> callback);

		#endregion
	}
}
