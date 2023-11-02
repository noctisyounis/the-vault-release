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
        
        private static GUIStyle _gitUpToDateStyle = null;
        
        public static GUIStyle GitUpToDateStyle
        {
            get
            {
                if (_gitUpToDateStyle != null) return _gitUpToDateStyle;

                _gitUpToDateStyle = new GUIStyle(EditorStyles.label)
                {
                    normal = {textColor = new Color(.2f, .8f, .2f, .4f)}, alignment = TextAnchor.MiddleRight
                };

                return _gitUpToDateStyle;
            }
        }
        
        private static GUIStyle _gitOutdatedStyle = null;
        
        public static GUIStyle GitOutdatedStyle
        {
            get
            {
                if (_gitOutdatedStyle != null) return _gitOutdatedStyle;

                _gitOutdatedStyle = new GUIStyle(EditorStyles.label)
                {
                    normal = {textColor = new Color(.8f, .0f, .2f, .6f)}, alignment = TextAnchor.MiddleRight
                };

                return _gitOutdatedStyle;
            }
        }
        
        private static GUIStyle _gitToCommitStyle = null;
        
        public static GUIStyle GitToCommitStyle
        {
            get
            {
                if (_gitToCommitStyle != null) return _gitToCommitStyle;

                _gitToCommitStyle = new GUIStyle(EditorStyles.label)
                {
                    normal = {textColor = new Color(1.0f, 0.498f, 0.314f, .6f)}, alignment = TextAnchor.MiddleRight
                };

                return _gitToCommitStyle;
            }
        }
        
        private static GUIStyle _gitAddedStyle = null;
        
        public static GUIStyle GitAddedStyle
        {
            get
            {
                if (_gitAddedStyle != null) return _gitAddedStyle;

                _gitAddedStyle = new GUIStyle(EditorStyles.label)
                {
                    normal = {textColor = new Color(0.0f, 0.3f, 0.9f, .9f)}, alignment = TextAnchor.MiddleRight
                };

                return _gitAddedStyle;
            }
        }
        
        #endregion
    }
}