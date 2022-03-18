using UnityEngine;
using UnityEditor.AddressableAssets.Settings;

using static UnityEditor.EditorGUIUtility;
using static UnityEngine.GUILayout;

namespace Universe.Editor
{
	public class ReloadAndBuildAddressable
	{
		public static void Draw()
		{
			var tex = IconContent(@"d_Profiler.NetworkOperations").image;
			if (Button(new GUIContent(" Refresh And Build Addressable", tex, "Refresh addressable then rebuild them")))
			{
				UGroupHelper.OnRefreshCompleted += RebuildAddressable;
				UGroupHelper.RefreshAaGroups();
			}
		}

		public static void RebuildAddressable()
		{
			AddressableAssetSettings.BuildPlayerContent();
			UGroupHelper.OnRefreshCompleted -= RebuildAddressable;
		}
	}
}