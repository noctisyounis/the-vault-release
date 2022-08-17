using UnityEditor;
using UnityEngine;

using static UnityEditor.EditorGUIUtility;
using static UnityEngine.GUILayout;

namespace Universe.Toolbar.Editor
{
	public class OpenAddressableButton
	{
		#region Main
		
		public static void Draw()
		{
			var tex = IconContent(_iconName).image;
			if (Button(new GUIContent(_buttonLabel, tex, _buttonTooltip)))
			{
				EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Groups");
			}
		}
		
		#endregion
		
		
		#region Private

		private static string _buttonLabel = "Open";
		private static string _buttonTooltip = "Open the addressable window";
		private static string _iconName = @"d_Profiler.NetworkOperations";

		#endregion
	}
}