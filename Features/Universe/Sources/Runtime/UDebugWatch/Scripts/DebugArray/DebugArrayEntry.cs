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

		public Func<string> m_computing;

		#endregion


		#region Universe API

		public override void OnLateUpdate(float deltaTime)
		{
			if(m_computing == null) return;

			var nextValue 	= m_computing.Invoke();

			m_name.text 	= name;
			m_value.text 	= nextValue;
		}

		#endregion
	}
}