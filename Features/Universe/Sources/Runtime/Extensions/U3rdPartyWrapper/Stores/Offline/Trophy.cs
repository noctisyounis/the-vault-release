using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Universe.Stores.Runtime;
using Universe.Trophy.Runtime;

namespace Universe.Stores.Offline.Runtime
{
	public class Trophy : UBehaviour
	{
		#region Exposed

		public List<TrophyBinary> m_trophies;
		public List<FactBase> m_relatedStats;

		#endregion
		
		
		#region Event

		public static event IStoreProvider.OnTrophyUnlockedHandler OnTrophyUnlocked;
		
		#endregion
		
		
		#region Unity API

		public override void Awake()
		{
			foreach (var stat in m_relatedStats)
			{
				stat.OnValueChanged += OnStatValueChanged;
			}
		}

		public override void OnDestroy()
		{
			foreach (var stat in m_relatedStats)
			{
				stat.OnValueChanged -= OnStatValueChanged;
			}
		}

		#endregion
		
		
		#region Main

		[Button("Populate Stats")]
		private void PopulateStats()
		{
			var length = m_trophies.Count;
			
			m_relatedStats = new();
			m_statTrophyBuffers = new();
			for (var i = 0; i < length; i++)
			{
				var trophy = m_trophies[i];
				var stat = trophy.GetValue();
				var statName = stat.name;
				if (m_statTrophyBuffers.ContainsKey(statName))
				{
					m_statTrophyBuffers[statName].Add(i);
					continue;
				}
				if (m_relatedStats.Contains(stat)) continue;

				var newBuffer = new List<int>();
				
				newBuffer.Add(i);
				m_relatedStats.Add(stat);
				m_statTrophyBuffers.Add(statName, newBuffer);
			}
		}

		public void UnlockTrophy(int id, Action callback = null)
		{
			if(id < 0) return;
			
			if (m_trophies.Count == 0)
			{
				Debug.LogError("[Trophy] No trophies registered");
				return;
			}

			if (!m_trophies.GreaterThan(id)) return;

			var trophy = m_trophies[id];
			var wasUnlocked = trophy.m_unlocked.Value;
			
			trophy.Unlock();
			if (!wasUnlocked) OnTrophyUnlocked?.Invoke(id);
			callback?.Invoke();
		}

		public void SetTrophyProgress(int id, int value, Action<int> callback = null)
		{
			if(id < 0) return;
			
			if (m_trophies.Count == 0)
			{
				Debug.LogError("[Trophy] No trophies registered");
				return;
			}
			
			if (!m_trophies.GreaterThan(id)) return;
			
			var trophy = m_trophies[id];
			var wasUnlocked = trophy.m_unlocked.Value;
			var unlocked = trophy.Evaluate();
			
			callback?.Invoke(value);
			if (unlocked && !wasUnlocked) OnTrophyUnlocked?.Invoke(id);
		}

		public void SetStatProgress(string statName, int value, Action<int> callback = null)
		{
			if (m_relatedStats.Count == 0)
			{
				Debug.LogError("[Trophy] No Stats registered");
				return;
			}

			var stat = m_relatedStats.Find((stat) => stat.name.Equals(statName));
			if (stat == null) return;
			
			if (m_statTrophyBuffers.ContainsKey(statName)) EvaluateThroughBuffer(stat.name);
			else EvaluateThroughAll(stat.name);

			callback?.Invoke(value);
		}

		#endregion
		
		
		#region Utils

		private void OnStatValueChanged(FactBase next)
		{
			var statName = next.name;
			var buffer = m_statTrophyBuffers[statName];
			var value = -1;

			if (buffer.Count == 0) return;
			
			if (next is BoolFact boolFact) value = boolFact.Value ? 1 : 0;
			else if (next is IntFact intFact) value = intFact.Value;

			if (value < 0) return;
			
			StoreManager.RequestSetTrophyProgress(buffer[0], value, nextValue => VerboseError($"Progress set to {nextValue}"));
			EvaluateThroughBuffer(statName);
		}

		private void EvaluateThroughBuffer(string of)
		{
			var trophyIds = m_statTrophyBuffers[of];
			var length = trophyIds.Count;

			for (var i = 0; i < length; i++)
			{
				var id = trophyIds[i];
				var trophy = m_trophies[id];
				var wasUnlocked = trophy.m_unlocked.Value;
				var unlocked = trophy.Evaluate();

				if (unlocked && !wasUnlocked) OnTrophyUnlocked?.Invoke(id);
			}
		}

		private void EvaluateThroughAll(string with)
		{
			var buffer = new List<int>();
			var length = m_trophies.Count;

			for (var id = 0; id < length; id++)
			{
				var trophy = m_trophies[id];
				var stat = trophy.GetValue();
				var statName = stat.name;
				if (!statName.Equals(with)) continue;

				var wasUnlocked = trophy.m_unlocked.Value;
				var unlocked = trophy.Evaluate();
				
				buffer.Add(id);
				
				if (unlocked && !wasUnlocked) OnTrophyUnlocked?.Invoke(id);
			}

			if (m_statTrophyBuffers.ContainsKey(with)) m_statTrophyBuffers[with] = buffer;
			else m_statTrophyBuffers.Add(with, buffer);
		}

		private string ExtractStatName(string from)
		{
			var splitName = from.Split('_');

			if (splitName.Length < 2) return "";
			return splitName[1];
		}
		
		#endregion
		
		
		#region Private

		[SerializeField, ReadOnly]
		private Dictionary<string, List<int>> m_statTrophyBuffers = new();

		#endregion
	}
}
