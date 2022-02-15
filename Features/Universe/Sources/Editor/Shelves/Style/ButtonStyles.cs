using UnityEngine;

namespace Universe.Toolbar.Editor
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
}