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

		public void Start()
		{
			m_offline.Initialize();

			_provider = U3rdPartyWrapper.GetStoreProvider();
			if (!_provider) throw new Exception($"[StoreManager] No store provider found");
			if (!_provider.Initialize()) return;

			if (m_skipEntitlement)
			{
				OnEntitlementSucceed?.Invoke();
				return;
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

		public static void RequestPostScore(int toId, long value, Action<int> callback) =>
			Instance.PostScore(toId, value, callback);

		public static void RequestScore(int fromId, Action<Entry> callback) =>
			Instance.GetScore(fromId, callback);
		
		public static void RequestOfflineScore(int fromId, Action<Entry> callback) =>
			Instance.GetOfflineScore(fromId, callback);
		
		public static void RequestRankings(int fromId, int amount, EntryAlignment alignment, Action<Entry[]> callback) =>
			Instance.GetRankings(fromId, amount, alignment, callback);

		public static void RequestOfflineRankings(int fromId, int amount, EntryAlignment alignment, Action<Entry[]> callback) =>
			Instance.GetOfflineRankings(fromId, amount, alignment, callback);

		private void PostScore(int toId, long value, Action<int> callback)
		{
			if (m_offline) m_offline.PostScore(toId, value, callback);
			if (CanUseStore) _provider.PostScore(toId, value, callback);
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

		private static StoreManager _instance;
		private bool _initResult;
		private IStoreProvider _provider;
		private AsyncProgress _entitlement;

		#endregion
	}
}
