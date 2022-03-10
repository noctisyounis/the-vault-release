using UnityEngine;
using UnityEditor.AddressableAssets.Settings;

using static UnityEditor.EditorGUIUtility;
using static UnityEngine.GUILayout;

namespace Universe.Toolbar.Editor
{
	public class BuildAddressableButton
	{
		public static void Draw()
		{
			var tex = IconContent(@"d_Profiler.NetworkOperations").image;
			if (Button(new GUIContent("Build Addressables", tex, "Build the addressables")))
			{
				AddressableAssetSettings.BuildPlayerContent();
			}
		}
	}
}