using UnityEngine;

using static UnityEditor.EditorGUIUtility;
using static UnityEngine.GUILayout;
using static  UnityEditor.AddressableAssets.Settings.AddressableAssetSettings;

namespace Universe.Toolbar.Editor
{
	public class BuildAddressableButton
	{
		#region Main
		
		public static void Draw()
		{
			var tex = IconContent(_iconName).image;
			if (Button(new GUIContent(_buttonLabel, tex, _buttonTooltip)))
			{
				BuildPlayerContent();
			}
		}
		
		#endregion
		
		
		#region Private

		private static string _iconName = @"d_Profiler.NetworkOperations";
		private static string _buttonLabel = "Build";
		private static string _buttonTooltip = "Build the addressables";

		#endregion
	}
}