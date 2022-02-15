using UnityEditor;
using UnityEngine;

namespace Symlink.Editor
{
    public class SymllinkGUIStyle
    {
        #region Public
        
        private static GUIStyle _symlinkMarkerStyle = null;
        
        public static GUIStyle SymlinkMarkerStyle
        {
            get
            {
                if (_symlinkMarkerStyle != null) return _symlinkMarkerStyle;

                _symlinkMarkerStyle = new GUIStyle(EditorStyles.label)
                {
                    normal = {textColor = new Color(.2f, .8f, .2f, .8f)}, alignment = TextAnchor.MiddleRight
                };

                return _symlinkMarkerStyle;
            }
        }
        
        #endregion
    }
}