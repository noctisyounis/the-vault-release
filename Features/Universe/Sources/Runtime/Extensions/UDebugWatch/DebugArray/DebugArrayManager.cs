using System;
using System.Collections.Generic;
using UnityEngine;

namespace Universe.DebugWatch.Runtime
{
	public class DebugArrayManager : UBehaviour
	{
		#region Public Members

		[Header("Array Manager Settings")]
		public GameObjectFact m_instance;
		public DebugArrayEntry m_entryTemplate;
		public Transform[] m_columns;
		public int m_columnCapacity;
		public float m_tickRate;

		#endregion


		#region Unity API

		public override void Awake() => m_instance.Value = gameObject;
		public override void OnDestroy() => m_instance.Value = null;

		#endregion


		#region Public API

		public void AddEntry(string name, Func<string> valueComputer)
		{
			var existing = _entries.Find((entry) => entry.name.Equals(name));

			if(existing) return;

			var nextColumn = GetNextColumn();

			if(nextColumn < 0)
			{
				Debug.LogError($"No more available space to display {name}.", this);
				return;
			}

			var column = m_columns[nextColumn];
			var newEntry = Instantiate<DebugArrayEntry>(m_entryTemplate, column);
			var nextTick = Mathf.Ceil(UTime.Time);

			UpdateColumn( column );

			newEntry.name = name;
			newEntry.m_computing = valueComputer;
			newEntry.m_tickRate = m_tickRate;

			_entries.Add(newEntry);

			foreach (var entry in _entries)
			{
				entry.NextTick = nextTick;
			}
		}

		public void RemoveEntry(string name)
		{
			var existing = _entries.Find((entry) => entry.name.Equals(name));

			if(!existing) return;

			_entries.Remove(existing);
			DestroyImmediate(existing.gameObject);

			foreach( var column in m_columns )
			{
				UpdateColumn( column );
			}
		}

		#endregion


		#region Utils

		private int GetNextColumn()
		{
			var count = m_columns.Length;
			for (var i = 0; i < count; i++)
			{
				var childCount = m_columns[i].childCount;

				if(childCount < m_columnCapacity) return i;
			}

			return -1;
		}

		private void UpdateColumn( Transform target )
		{
			var rectColumn = (RectTransform) target;
			var rectEntry = m_entryTemplate.GetComponent<RectTransform>();
			var currentSize = rectColumn.sizeDelta;
			var childCount = rectColumn.childCount;

			currentSize.y = childCount * rectEntry.sizeDelta.y;
			rectColumn.sizeDelta = currentSize;
		}

		#endregion


		#region Private Members

		private List<DebugArrayEntry> _entries = new();

		#endregion
	}
}