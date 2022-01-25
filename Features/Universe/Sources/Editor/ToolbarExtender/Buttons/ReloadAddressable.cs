using UnityEditor;
using UnityEngine;

using static UnityEditor.EditorGUIUtility;
using static UnityEngine.GUILayout;
using static Universe.ToolbarExtention.ButtonStyles;
using static Universe.ToolbarExtention.ToolbarExtender;

namespace Universe.ToolbarExtention
{
	
	static class ButtonStyles
	{
		public static readonly GUIStyle commandButtonStyle;

		static ButtonStyles()
		{
			commandButtonStyle = new GUIStyle("Command")
			{
				fontSize = 9,
				alignment = TextAnchor.MiddleCenter,
				imagePosition = ImagePosition.ImageLeft,
				fixedWidth = 110
			};
		}
	}
	
	[InitializeOnLoad]
	public class ReloadAddressableButton
	{
		static ReloadAddressableButton()
		{
			LeftToolbarGUI.Add(OnToolbarGUI);
		}

		static void OnToolbarGUI()
		{
			FlexibleSpace();

			var tex = IconContent(@"d_Profiler.NetworkOperations").image;
			if (Button(new GUIContent("Refresh Addressable", tex, "Focus SceneView when entering play mode"), commandButtonStyle))
			{
				UGroupHelper.RefreshAaGroups();
			}
		}
	}
}