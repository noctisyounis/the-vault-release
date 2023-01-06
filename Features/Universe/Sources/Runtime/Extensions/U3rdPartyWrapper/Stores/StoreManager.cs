using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Universe.Leaderboard.Runtime;
using Universe.Stores.Offline.Runtime;
using static Universe.Stores.Runtime.AsyncProgress;

namespace Universe.Stores.Runtime
{
	public class StoreManager : UBehaviour
	{
		#region Exposed

		[Header("Parameters")]
		public bool m_skipEntitlement;
		public OfflineProvider m_offline;
		public BoolFact m_unlockTest;
		public IntFact m_progressTest;

		#endregion
		
		
		#region Event

		public UnityEvent OnEntitlementSucceed;
		public UnityEvent OnEntitlementFailed;
		
		#endregion
		
		
		#region Unity API

		private void OnGUI()
		{
			if (GUILayout.Button("Unlock Trophy"))
			{
				Debug.Log("Try to unlock");
				m_unlockTest.Value = true;
				//RequestTrophyUnlock(0, () => Debug.Log($"Unlocked"));
			}

			if (GUILayout.Button("Progress Trophy"))
			{
				//_debugProgress++;
				//RequestSetTrophyProgress(1, _debugProgress, (next) => Debug.Log($"Progress set to {next}"));
			}
			
			if (GUILayout.Button("Progress Stat"))
			{
				m_progressTest.Value += 1;

				//_debugProgress++;
				//RequestSetStatProgress("TrophyProgress", _debugProgress, (next) => Debug.Log($"Progress set to {next}"));
			}
		}

		public override void Awake()
		{
			base.Awake();
			Instance = this;
		}

		public IEnumerator Start()
		{
			m_offline.Initialize();

			_provider = U3rdPartyWrapper.GetStoreProvider();
			if (!_provider) throw new Exception($"[StoreManager] No store provider found");
			if (!_provider.Initialize()) yield break;

			if (m_skipEntitlement)
			{
				OnEntitlementSucceed?.Invoke();
				_entitlement = SUCCEED;
				yield return new WaitForSeconds(3.0f);
				Debug.LogWarning($"Requesting update progress: {_entitlement}");
				m_progressTest.Value = 11;
				yield break;
			}
			
			_entitlement = STARTED;
			StartCoroutine(WaitEntitlement());
			_provider.DoEntitlementCheck((hasEntitlement) =>
			{
				Debug.LogError($"[StoreManager] Entitlement found: {hasEntitlement}");

				if (!hasEntitlement)
				{
					_entitlement = FAILED;
					return;
				}

				_entitlement = SUCCEED;
			});
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

		private void PostScore(int toId, int value, Action<int> callback)
		{
			if (CanUseStore)
			{
				_provider.PostScore(toId, value, callback);
				if(m_offline) m_offline.PostScore(toId, value);
			}
			else if (m_offline) m_offline.PostScore(toId, value, callback);
		}

		private void GetScore(int fromId, Action<Entry> callback)
		{
			if (CanUseStore) _provider.GetScore(fromId, callback);
		}

		private void GetOfflineScore(int fromId, Action<Entry> callback)
		{
			if (m_offline) m_offline.GetScore(fromId, callback);
		}

		private void GetRankings(int fromId, int amount, EntryAlignment alignment, Action<Entry[]> callback)
		{
			if (CanUseStore) _provider.GetRankings(fromId, amount, alignment, callback);
		}
		
		private void GetOfflineRankings(int fromId, int amount, EntryAlignment alignment, Action<Entry[]> callback)
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
		
		private void TrophyUnlock(int id, Action callback)
		{
			if (CanUseStore)
			{
				_provider.UnlockTrophy(id, callback);
				if(m_offline) m_offline.UnlockTrophy(id);
			}
			else if(m_offline) m_offline.UnlockTrophy(id, callback);
		}

		private void SetTrophyProgress(int id, int value, Action<int> callback)
		{
			if (CanUseStore)
			{
				Debug.LogWarning("online");
				_provider.SetTrophyProgress(id, value, callback);
				if(m_offline) m_offline.SetTrophyProgress(id, value);
			}
			else if(m_offline) m_offline.SetTrophyProgress(id, value, callback);
		}

		private void SetStatProgress(string statName, int value, Action<int> callback)
		{
			if (CanUseStore)
			{
				Debug.LogWarning("Using provider");
				_provider.SetStatProgress(statName, value, callback);
				if(m_offline) m_offline.SetStatProgress(statName, value);
			}
			else if(m_offline) m_offline.SetStatProgress(statName, value, callback);
		}
		
		#endregion
		
		
		#region Main

		private IEnumerator WaitEntitlement()
		{
			while (HasEntitlementStarted) yield return new WaitForEndOfFrame();
			
			if (HasEntitlementFailed) OnEntitlementFailed?.Invoke();
			if (HasEntitlementSucceed) OnEntitlementSucceed?.Invoke();
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
		private AsyncProgress _entitlement;

		private int _debugProgress;

		#endregion
	}
}
