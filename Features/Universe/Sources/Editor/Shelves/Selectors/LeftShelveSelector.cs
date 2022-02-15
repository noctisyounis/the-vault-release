using UnityEditor;

using static UnityEngine.GUILayout;
using static Universe.Toolbar.Editor.ToolbarExtender;

namespace Universe.Toolbar.Editor
{

	[InitializeOnLoad]
	public class LeftShelveSelector
	{
		#region Constructor
		
		static LeftShelveSelector()
		{
			LeftToolbarGUI.Add(OnToolbarGUI);
		}
		
		#endregion
		
		
		#region Unity API

		static void OnToolbarGUI()
		{
			FlexibleSpace();
			ShelveSelector.Draw(SIDE.Left);
		}
		
		#endregion
	}
}