using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Universe.Leaderboard.Runtime;
using Universe.Stores.Offline.Runtime;
using static Universe.Stores.Runtime.AsyncState;

namespace Universe.Stores.Runtime
{
	public class StoreManager : UBehaviour
	{
		#region Exposed

		[Header("Parameters")]
		public bool m_skipEntitlement;
		public OfflineProvider m_offline;
		public IntFact m_debug;

		#endregion
		
		
		#region Event

		public UnityEvent OnEntitlementSucceed;
		public UnityEvent OnEntitlementFailed;
		
		#endregion
		
		
		#region Unity API

		public override void Awake()
		{
			base.Awake();
			Instance = this;
		}

		public IEnumerator Start()
		{
			_provider = U3rdPartyWrapper.GetStoreProvider();
			if (!_provider) Debug.LogError($"[StoreManager] No store provider found, going offline mode");

			Initialize();
			yield return 0;
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			
			if (m_offline) m_offline.ShutDown();
			if (CanUseStore) _provider.ShutDown();
		}

		#endregion
		
		
		#region Universe API

		public override void OnUpdate(float deltatime)
		{
			if (m_offline) m_offline.Tick();
			if (CanUseStore) _provider.Tick();
		}

		#endregion
		
		
		#region Lifecycle

		private void Initialize()
		{
			try
			{
				StartCoroutine(_provider.Initialize(OnProviderInitializationEnded));
				StartCoroutine(m_offline.Initialize());
			}
			catch
			{
				Debug.LogError("[[StoreManager] Store provider initialization failed, going offline mode]");
				StartCoroutine(m_offline.Initialize(OnProviderInitializationEnded));
			}
		}

		private void OnProviderInitializationEnded(AsyncState state)
		{
			VerboseWarning("[StoreManager] Checking entitlement");
			
#if UNITY_EDITOR
			
			OnEntitlementSucceed?.Invoke();
			_entitlement = SUCCEED;
			
			return;
			
#endif
			
			if (m_skipEntitlement)
			{
				OnEntitlementSucceed?.Invoke();
				_entitlement = SUCCEED;
				
				return;
			}

			if (state.Equals(FAILED)) return;

			_entitlement = STARTED;
			if (!_provider)
			{
				_entitlement = SUCCEED;
				OnEntitlementSucceed?.Invoke();
				return;
			}
			
			_provider.DoEntitlementCheck((hasEntitlement) =>
			{
				Debug.LogError($"[StoreManager] Entitlement found: {hasEntitlement}");

				if (!hasEntitlement)
				{
					OnEntitlementFailed?.Invoke();
					_entitlement = FAILED;
					return;
				}

				_entitlement = SUCCEED;
				OnEntitlementSucceed?.Invoke();
			});
		}
		
		#endregion
		
		
		#region Leaderboard

		public static void RequestPostScore(int toId, int value, Action<int> callback = null) =>
			Instance.PostScore(toId, value, callback);

		public static void RequestScore(int fromId, Action<Entry> callback = null) =>
			Instance.GetScore(fromId, callback);
		
		public static void RequestOfflineScore(int fromId, Action<Entry> callback = null) =>
			Instance.GetOfflineScore(fromId, callback);
		
		public static void RequestRankings(int fromId, int amount, EntryAlignment alignment, Action<Entry[]> callback = null) =>
			Instance.GetRankings(fromId, amount, alignment, callback);

		public static void RequestOfflineRankings(int fromId, int amount, EntryAlignment alignment, Action<Entry[]> callback = null) =>
			Instance.GetOfflineRankings(fromId, amount, alignment, callback);

		private void PostScore(int toId, int value, Action<int> callback = null)
		{
			if (CanUseStore)
			{
				_provider.PostScore(toId, value, callback);
				if(m_offline) m_offline.PostScore(toId, value);
			}
			else if (m_offline) m_offline.PostScore(toId, value, callback);
		}

		private void GetScore(int fromId, Action<Entry> callback = null)
		{
			if (CanUseStore) _provider.GetScore(fromId, callback);
		}

		private void GetOfflineScore(int fromId, Action<Entry> callback = null)
		{
			if (m_offline) m_offline.GetScore(fromId, callback);
		}

		private void GetRankings(int fromId, int amount, EntryAlignment alignment, Action<Entry[]> callback = null)
		{
			if (CanUseStore) _provider.GetRankings(fromId, amount, alignment, callback);
		}
		
		private void GetOfflineRankings(int fromId, int amount, EntryAlignment alignment, Action<Entry[]> callback = null)
		{
			if (m_offline) m_offline.GetRankings(fromId, amount, alignment, callback);
		}
		
		#endregion
		
		
		#region Trophy

		public static void RequestTrophyUnlock(int id, Action callback = null) =>
			Instance.TrophyUnlock(id, callback);

		public static void RequestSetTrophyProgress(int id, int value, Action<int> callback = null) =>
			Instance.SetTrophyProgress(id, value, callback);

		public static void RequestSetStatProgress(string statName, int value, Action<int> callback = null) =>
			Instance.SetStatProgress(statName, value, callback);

		private void SubscribeOnTrophyUnlocked(IStoreProvider.OnTrophyUnlockedHandler handler)
		{
			if (CanUseStore) _provider.SubscribeOnTrophyUnlocked(handler);
			else if(m_offline) m_offline.SubscribeOnTrophyUnlocked(handler);
		}
		
		private void TrophyUnlock(int id, Action callback = null)
		{
			if (CanUseStore)
			{
				_provider.UnlockTrophy(id, callback);
				if(m_offline) m_offline.UnlockTrophy(id);
			}
			else if(m_offline) m_offline.UnlockTrophy(id, callback);
		}

		private void SetTrophyProgress(int id, int value, Action<int> callback = null)
		{
			if (CanUseStore)
			{
				_provider.SetTrophyProgress(id, value, callback);
				if(m_offline) m_offline.SetTrophyProgress(id, value);
			}
			else if(m_offline) m_offline.SetTrophyProgress(id, value, callback);
		}

		private void SetStatProgress(string statName, int value, Action<int> callback = null)
		{
			if (CanUseStore)
			{
				_provider.SetStatProgress(statName, value, callback);
				if(m_offline) m_offline.SetStatProgress(statName, value);
			}
			else if(m_offline) m_offline.SetStatProgress(statName, value, callback);
		}
		
		#endregion
		
		
		#region Activity

		public static void RequestStartActivity(string activityID, Action callback = null) =>
			Instance.StartActivity(activityID, callback);
		
		public static void RequestResumeActivity(string activityID, string[] progressIds, string[] completedIds, Action callback = null) => 
			Instance.ResumeActivity(activityID, progressIds, completedIds, callback)
;
		public static void RequestCompleteActivity(string activityID, Action callback = null) =>
			Instance.CompleteActivity(activityID, callback);
		
		public static void RequestFailActivity(string activityID, Action callback = null) =>
			Instance.FailActivity(activityID, callback);
		
		public static void RequestCancelActivity(string activityID, Action callback = null) =>
			Instance.CancelActivity(activityID, callback);
		
		public static void RequestUnlockActivities(string[] activityIds, Action callback = null) =>
			Instance.UnlockActivities(activityIds, callback);
		
		public static void RequestLockActivities(string[] activityIds, Action callback = null) =>
			Instance.LockActivities(activityIds, callback);

		public void OnActivityStart(string activityID)
		{
			StartActivity(activityID);
			//StartCoroutine(GameIntentDebug(activityID));
		}
		
		private IEnumerator GameIntentDebug(string id)
		{
			yield return new WaitForSeconds(2.0f);
			CompleteActivity($"{id}-task-01");
			yield return new WaitForSeconds(5.0f);
			CompleteActivity($"{id}-task-02");
			yield return new WaitForSeconds(5.0f);
		}

		private void StartActivity(string activityID, Action callback = null)
		{
			if (CanUseStore)
			{
				_provider.StartActivity(activityID, callback);
				if(m_offline) m_offline.StartActivity(activityID);
			}
			else if(m_offline) m_offline.StartActivity(activityID, callback);
		}
		
		private void ResumeActivity(string activityID, string[] progressIds, string[] completedIds, Action callback = null)
		{
			if (CanUseStore)
			{
				_provider.ResumeActivity(activityID, progressIds, completedIds, callback);
				if(m_offline) m_offline.ResumeActivity(activityID, progressIds, completedIds);
			}
			else if(m_offline) m_offline.ResumeActivity(activityID, progressIds, completedIds, callback);
		}

		private void CompleteActivity(string activityID, Action callback = null)
		{
			if (CanUseStore)
			{
				_provider.CompleteActivity(activityID, callback);
				if(m_offline) m_offline.CompleteActivity(activityID);
			}
			else if(m_offline) m_offline.CompleteActivity(activityID, callback);
		}
		
		private void FailActivity(string activityID, Action callback = null)
		{
			if (CanUseStore)
			{
				_provider.FailActivity(activityID, callback);
				if(m_offline) m_offline.FailActivity(activityID);
			}
			else if(m_offline) m_offline.FailActivity(activityID, callback);
		}
		
		private void CancelActivity(string activityID, Action callback = null)
		{
			if (CanUseStore)
			{
				_provider.CancelActivity(activityID, callback);
				if(m_offline) m_offline.CancelActivity(activityID);
			}
			else if(m_offline) m_offline.CancelActivity(activityID, callback);
		}
		
		public void UnlockActivities(string[] activityIds, Action callback = null)
		{
			if (CanUseStore)
			{
				_provider.UnlockActivities(activityIds, callback);
				if(m_offline) m_offline.UnlockActivities(activityIds);
			}
			else if(m_offline) m_offline.UnlockActivities(activityIds, callback);
		}
		
		public void LockActivities(string[] activityIds, Action callback = null)
		{
			if (CanUseStore)
			{
				_provider.LockActivities(activityIds, callback);
				if(m_offline) m_offline.LockActivities(activityIds);
			}
			else if(m_offline) m_offline.LockActivities(activityIds, callback);
		}
		
		#endregion
		
		
		#region Private

		private static StoreManager Instance
		{
			get => _instance;
			set
			{
				if(_instance) Debug.LogWarning("[StoreManager] Another instance already loaded");
				_instance = value;
			}
		}

		private bool CanUseStore => (_provider && HasEntitlementSucceed);
		private bool HasEntitlementStarted => (_entitlement.Equals(STARTED));
		private bool HasEntitlementFailed => (_entitlement.Equals(FAILED));
		private bool HasEntitlementSucceed => (_entitlement.Equals(SUCCEED));
		private bool IsEntitlementWaiting => (_entitlement.Equals(WAITING));

		private static StoreManager _instance;
		private bool _initResult;
		private IStoreProvider _provider;
		private AsyncState _entitlement;

		#endregion
	}
}
