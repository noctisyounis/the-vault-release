using UnityEngine;
using UnityEditor.Build.Pipeline.Utilities;

using static UnityEditor.EditorGUIUtility;
using static UnityEngine.GUILayout;
using static  UnityEditor.AddressableAssets.Settings.AddressableAssetSettings;

namespace Universe.Toolbar.Editor
{
	public class ClearAddressable
	{
		#region Main
		
		public static void Draw()
		{
			
			var tex = IconContent(_iconName).image;
			if (Button(new GUIContent(_buttonLabel, tex, _buttonTooltip)))
			{
				BuildCache.PurgeCache(true);
			}
		}
		
		#endregion
		
		
		#region Private

		private static string _buttonLabel = "Clear Build";
		private static string _buttonTooltip = "Clear the current build";
		private static string _iconName = @"d_Profiler.NetworkOperations";

		#endregion
	}
}