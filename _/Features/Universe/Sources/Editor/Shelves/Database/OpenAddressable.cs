using UnityEditor;
using UnityEngine;

using static UnityEditor.EditorGUIUtility;
using static UnityEngine.GUILayout;

namespace Universe.Toolbar.Editor
{
	public class OpenAddressableButton
	{
		public static void Draw()
		{
			var tex = IconContent(@"d_Profiler.NetworkOperations").image;
			if (Button(new GUIContent("Open Addressable", tex, "Open the addressable window")))
			{
				EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Groups");
			}
		}
	}
}