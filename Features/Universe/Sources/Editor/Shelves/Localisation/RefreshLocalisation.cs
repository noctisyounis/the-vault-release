using UnityEngine;
using Universe.Editor;

using static UnityEditor.EditorGUIUtility;
using static UnityEngine.GUILayout;

namespace Universe.Toolbar.Editor
{
	public static class RefreshLocalisation
	{
		public static void Draw()
		{
			var tex = IconContent(@"d_Profiler.NetworkOperations").image;
			if (Button(new GUIContent("Refresh localisation", tex, "Refresh the localisation")))
			{
				RefreshLanguageEditorWindow.RefreshAllLanguages();
			}
		}
	}
}