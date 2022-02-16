using UnityEditor;

using static UnityEngine.GUILayout;
using static Universe.Toolbar.Editor.ToolbarExtender;

namespace Universe.Toolbar.Editor
{

    [InitializeOnLoad]
    public class RightShelveSelector
    {
        #region Constructor
        
        static RightShelveSelector()
        {
            RightToolbarGUI.Add(OnToolbarGUI);
        }
        
        #endregion
        
        
        #region Unity API

        static void OnToolbarGUI()
        {
            FlexibleSpace();
            ShelveSelector.Draw(SIDE.Right);
        }
        #endregion
    }
}