using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Universe.Leaderboard.Runtime
{
	public class LeaderboardDisplayer : UBehaviour
	{
		#region Exposed

		[Header("References")] 
		public Transform m_holder;
		public AssetReferenceGameObject m_entryTemplate;
		
		[Header("Parameters")]
		public int m_maxDisplayedAmount;
		public Entry[] m_content;
		
		#endregion


		#region Unity API

		public void Start()
		{
			Refresh();
		}

		#endregion
		
		
		#region Main

		public void SetContent(Entry[] next) => m_content = next;
		
		public void Refresh()
		{
			var contentAmount = m_content.Length;
			var currentEntryAmount = m_entries.Count;

			for (var i = 0; i < m_maxDisplayedAmount; i++)
			{
				if (i >= contentAmount)
				{
					if (!m_entries.GreaterThan(i)) return;

					m_entries[i].gameObject.SetActive(false);
					
					continue;
				}
				
				var currentValue = m_content[i];

				if (i < currentEntryAmount)
				{
					var entry = m_entries[i];

					entry.Value = currentValue;
				}
				else Spawn(m_entryTemplate, Vector3.zero, Quaternion.identity, m_holder, go =>
				{
					var entry = go.GetComponent<EntryDisplayer>();
					entry.Value = currentValue;
					
					m_entries.Add(entry);
				}); 

			}
		}
		
		#endregion
		
		
		#region Private

		[SerializeField]
		private List<EntryDisplayer> m_entries = new();

		#endregion
	}
}
