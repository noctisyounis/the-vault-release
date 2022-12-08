using UnityEngine;

using static UnityEditor.EditorGUIUtility;
using static UnityEngine.GUILayout;

namespace Universe.Toolbar.Editor
{
    public class OpenLevel
	{
		#region Main

		public static void Draw()
		{
			var tex = IconContent(@"d_Project").image;
			if( !Button( new GUIContent( "Open Level", tex, "Select a level to open level" ) ) )
				return;

			OpenLevelWindow.ShowWindow();
		}

		#endregion
	}
}