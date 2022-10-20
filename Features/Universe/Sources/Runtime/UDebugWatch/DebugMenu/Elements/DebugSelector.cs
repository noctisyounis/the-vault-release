using System;
using TMPro;
using UnityEngine;

using static Universe.DebugWatch.Runtime.DebugMenuRoot;

namespace Universe.DebugWatch.Runtime
{
	public class DebugSelector : DebugElement, IClickable, ICancelable, ISettable
	{
		#region Public Members
		
		[Header("References")]
		public TMP_Text m_label;
		public TMP_Text m_currentName;
		public GameObject m_arrowHover;
		
		[Header("Parameters")] 
		public OptionData[] m_options;
		public Color m_disabled = Color.gray;
		public Color m_selected = Color.white;

		public Color m_valid = Color.white;
		public Color m_error = Color.white;
		
		public OptionData[] Options
		{
			get => m_options ??= new OptionData[0];
			set
			{
				m_options = value;
				UpdateDisplay();
			}
		}
		

		#endregion


		#region Unity API

		private void Awake()
		{
			OnDeselected();
			FetchStatus();
			UpdateDisplay();
		}

		private void OnDisable() => OnDeselected();

		#endregion


		#region Main

		public void OnClick() => s_instance.Execute<object>(m_path, _currentOption);

		public void OnCancel() => m_owner.ReturnToParent();

		public void SetOption(int next)
		{
			if (next > 0)		Next();
			else if (next < 0)	Previous();
		}
		
		public void Next()
		{
			var length = Options.Length;

			_currentOption++;
			_currentOption = (_currentOption >= length) ? 0 : _currentOption;

			UpdateDisplay();
		}

		public void Previous()
		{
			var length = Options.Length;

			_currentOption--;
			_currentOption = (_currentOption < 0) ? (length - 1) : _currentOption;

			UpdateDisplay();
		}

		private void UpdateDisplay()
		{
			if (!m_currentName) return;
			
			if (Options is null)
			{
				DisplayError();
				return;
			}
			
			var length = Options.Length;
			if (length == 0)
			{
				DisplayError();
				return;
			}
			
			var name = Options[_currentOption].m_name;

			SetOptionColor(m_valid);
			m_currentName.text = name;
		}

		private void DisplayError()
		{
			m_currentName.text = "No Options";
			SetOptionColor(m_error);
		}


		#endregion


		#region Utils

		public override void OnSelected()
		{
			FetchStatus();
			DisplayArrow();
			DisplayTooltip();
			SetLabelColor(m_selected);
		}

		public override void OnDeselected()
		{
			HideArrow();
			SetLabelColor(m_disabled);
		}
		
		private void FetchStatus()
		{
			var result = s_instance.GetLastResult(m_path);

			if (result is not int index) return;

			_currentOption = 0;
			var length = Options.Length;
			for (var i = 0; i < length; i++)
			{
				var value = Options[i].m_value;
				if(!value.Equals(index)) continue;

				_currentOption = i;
				break;
			}
		}
		
		private void SetLabelColor(Color next)
		{
			if (m_label == null) return;

			m_label.color = next;
		}
		
		private void SetOptionColor(Color next)
		{
			if (m_currentName == null) return;

			m_currentName.color = next;
		}
		
		private void DisplayArrow() => m_arrowHover.gameObject.SetActive(true);
		private void HideArrow() => m_arrowHover.gameObject.SetActive(false);
		private void DisplayTooltip() => s_instance.DisplayTooltip( m_path );

		#endregion
		
		
		#region Private

		private int _currentOption;

		#endregion
	}
}