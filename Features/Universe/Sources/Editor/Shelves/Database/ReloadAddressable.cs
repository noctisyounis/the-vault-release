using UnityEditor;
using UnityEngine;

using static UnityEditor.EditorGUIUtility;
using static UnityEngine.GUILayout;

namespace Universe.Toolbar.Editor
{
	public class ReloadAddressableButton
	{
		public static void Draw()
		{
			var tex = IconContent(@"d_Profiler.NetworkOperations").image;
			if (Button(new GUIContent(" Refresh Addressable", tex, "Focus SceneView when entering play mode")))
			{
				UGroupHelper.RefreshAaGroups();
			}
		}
	}
}