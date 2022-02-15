using UnityEngine;

namespace Universe.Toolbar.Editor
{
    public static class ToolbarStyle
    {
        public static readonly GUIStyle commandButtonStyle;

        static ToolbarStyle()
        {
            commandButtonStyle = new GUIStyle("Command")
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Bold
            };
        }
    }    
}