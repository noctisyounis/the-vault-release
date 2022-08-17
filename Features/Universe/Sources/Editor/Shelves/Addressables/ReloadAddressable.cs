using UnityEditor;
using UnityEngine;
using Universe.Editor;

using static UnityEditor.EditorGUIUtility;
using static UnityEngine.GUILayout;

namespace Universe.Toolbar.Editor
{
	public class ReloadAddressableButton
	{
		#region Main
		
		public static void Draw()
		{
			var tex = IconContent(_iconName).image;
			if (Button(new GUIContent(_buttonLabel, tex, _buttonTooltip)))
			{
				UGroupHelper.RefreshAaGroups();
			}
		}
		
		#endregion
		
		
		#region Private

		private static string _buttonLabel = "Scan";
		private static string _buttonTooltip = "Scan the addressables with the helpers";
		private static string _iconName = @"d_Profiler.NetworkOperations";

		#endregion
	}
}