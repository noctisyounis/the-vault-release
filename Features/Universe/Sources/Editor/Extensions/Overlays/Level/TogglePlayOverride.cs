using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

using static UnityEditor.EditorGUIUtility;
using static UnityEngine.PlayerPrefs;
using static Universe.Editor.UPrefs;

namespace Universe.Overlays
{
	public class TogglePlayOverride : EditorToolbarToggle
	{
		#region Public
		
		public string m_onIcon	= "d_winbtn_mac_max_h";
		public string m_offIcon = "d_winbtn_mac_close_h";
		public string m_label	= "Override Playmode";
		
		#endregion
		
		
		#region Constructor

		public TogglePlayOverride()
		{
			Initialize();
			
			this.RegisterValueChangedCallback(OnToggle);
		}
		
		#endregion
		
		
	    #region Main

	    private void Initialize()
	    {
		    InitializeValues();
		    InitializeIcons();
	    }

	    private void InitializeValues()
	    {
		    text		= m_label;
		    _playerPref = EDITOR_OVERRIDE_PLAYMODE;
		    
		    UpdateValue();
	    }
	    
	    private void InitializeIcons()
	    {
		    var onIconTexture   = IconContent(m_onIcon).image as Texture2D;
		    var offIconTexture	= IconContent(m_offIcon).image as Texture2D;

		    onIcon	= onIconTexture;
		    offIcon = offIconTexture;
	    }
	    
	    private void OnToggle(ChangeEvent<bool> context)
	    {
		    var next	= context.newValue;
		    var nextInt		= next ? 1 : 0;
		    
		    SetInt(_playerPref, nextInt);
	    }
	    
	    #endregion


	    #region Utils

	    private void UpdateValue()
	    {
		    var overrideBit = GetInt(_playerPref);
		    var next = (overrideBit == 1);
			
		    value = next;
	    }

	    #endregion
	    
	    
	    #region Private

	    private VisualElement	_content;
	    private string			_playerPref;

	    #endregion
	}
}