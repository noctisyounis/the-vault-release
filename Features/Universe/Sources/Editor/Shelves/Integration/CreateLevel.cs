using UnityEngine;

using static UnityEditor.EditorGUIUtility;
using static UnityEngine.GUILayout;


namespace Universe.Toolbar.Editor
{
    public static class CreateLevelButton
	{
		public static void Draw()
		{
			FlexibleSpace();

			var tex = IconContent(@"d_Toolbar Plus").image;
			if( !Button( new GUIContent( "Level", tex, "Add a new level to the project" ) ) )
				return;
			
			CreateLevelWindow.ShowLevelWindow();
		}
	}
}