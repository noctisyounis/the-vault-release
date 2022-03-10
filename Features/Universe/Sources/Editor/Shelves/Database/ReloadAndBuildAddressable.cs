using UnityEngine;
using UnityEditor.AddressableAssets.Settings;

using static UnityEditor.EditorGUIUtility;
using static UnityEngine.GUILayout;

namespace Universe
{
	public class ReloadAndBuildAddressable
	{
		public static void Draw()
		{
			var tex = IconContent(@"d_Profiler.NetworkOperations").image;
			if (Button(new GUIContent(" Refresh And Build Addressable", tex, "Refresh addressable then rebuild them")))
			{
				UGroupHelper.RefreshAaGroups();
				UGroupHelper.OnRefreshCompleted += RebuildAddressable;
			}
		}

		public static void RebuildAddressable()
		{
			AddressableAssetSettings.BuildPlayerContent();
			UGroupHelper.OnRefreshCompleted -= RebuildAddressable;
		}
	}
}