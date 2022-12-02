using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using Universe.Toolbar.Editor;

using static UnityEditor.EditorGUIUtility;

namespace Universe.Overlays
{
	[EditorToolbarElement(ID, typeof(EditorWindow))]
	public class OpenLevel : EditorToolbarButton
	{
		#region Exposed
		
		public const string ID = "Level/Open";
		public string m_iconName = "d_Project";

		#endregion
		

		#region Constructors

		public OpenLevel()
		{
			var tex = IconContent(m_iconName).image;
			
			text = "Open";
			tooltip = "Open an existing level";
			icon = tex as Texture2D;
			clicked += OnClick;
		}

		#endregion

		
		#region Main

		private void OnClick()
		{
			OpenLevelWindow.ShowWindow();
		}

		#endregion

		
	}
}