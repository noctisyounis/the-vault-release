using System;
using UnityEngine;
using TMPro;

namespace Universe
{
	public class DebugArrayEntry : UBehaviour
	{
		#region Public Members

		[Header("Entry Settings")]
		public TMP_Text m_name;
		public TMP_Text m_value;
		public float m_tickRate;
		
		public Func<string> m_computing;

		public float NextTick
		{
			get => _nextTick;
			set => _nextTick = value;
		}
		
		#endregion


		#region Universe API

		public override void OnLateUpdate(float deltaTime)
		{
			if(m_computing == null) return;

			var time = UTime.Time;
			if (NextTick > time) return;
			
			var nextValue 	= m_computing.Invoke();

			NextTick		= m_tickRate + time;
			m_name.text 	= name;
			m_value.text 	= nextValue;
		}

		#endregion

		
		#region Private

		private float _nextTick;

		#endregion
	}
}