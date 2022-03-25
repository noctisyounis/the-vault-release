using UnityEditor;
using UnityEngine;
using Universe.Editor;

using static UnityEditor.EditorGUIUtility;
using static UnityEngine.GUILayout;

namespace Universe.Toolbar.Editor
{
	public class ReloadAddressableButton
	{
		public static void Draw()
		{
			var tex = IconContent(@"d_Profiler.NetworkOperations").image;
			if (Button(new GUIContent("Refresh Addressable", tex, "Refresh the addressables with the helpers")))
			{
				UGroupHelper.RefreshAaGroups();
			}
		}
	}
}