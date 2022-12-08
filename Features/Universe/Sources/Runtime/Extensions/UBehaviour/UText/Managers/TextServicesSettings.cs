using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using static System.IO.Directory;
#if UNITY_EDITOR
using static UnityEditor.AssetDatabase;
#endif
using static UnityEngine.Debug;

namespace Universe
{
    public class TextServicesSettings : UniverseScriptableObject
    {
        #region Public Members

        [SerializeField]
        public FontSettingsTable m_defaultFontSettings;
        public bool m_isDebug;
        [Tooltip("Please put to false when no TextManager in task.")]
        public bool m_showErrors = true;

        public static int m_selectedIndex = 0;

        #endregion


        #region Constants

        public const string SETTINGS_ASSETS_DIRECTORY = "Assets/Settings/Universe"; 
        public const string SETTINGS_ASSETS_NAME = "TextServicesSettings.asset";

        #endregion

        internal static TextServicesSettings GetOrCreateSettings()
        {
#if UNITY_EDITOR
            var textServices = GetAllTextServicesSettingsAssets<TextServicesSettings>();

            if( textServices.Count == 0 )
            {
                var path = $"{SETTINGS_ASSETS_DIRECTORY}\\{SETTINGS_ASSETS_NAME}";
                var settings = LoadAssetAtPath<TextServicesSettings>( path );

                if( settings != null ) return settings;

                return CreateSettingsAt( path );
            }
            else
            {
                return textServices[m_selectedIndex];
            }
#else
            return null;
#endif
        }

        private static TextServicesSettings CreateSettingsAt( string relativePath )
        {
#if UNITY_EDITOR
            var settings = CreateInstance<TextServicesSettings>();

            if( !Exists( relativePath ) ) CreateDirectory( relativePath );

            CreateAsset( settings, $"{relativePath}" );
            SaveAssets();
            LogWarning( $"Created new settings at <color=cyan>'{relativePath}\\{SETTINGS_ASSETS_NAME}'</color>", settings );

            return settings;
#else
            return null;
#endif
        }


        #region Utilities

        public static List<T> GetAllTextServicesSettingsAssets<T>() where T : TextServicesSettings
        {
#if UNITY_EDITOR
            return FindAssets( $"t:{typeof( T ).Name}" )
                                       .Select( guid => GUIDToAssetPath( guid ) )
                                       .Select( path => LoadAssetAtPath( path, typeof( T ) ) )
                                       .Select( obj => (T)obj )
                                       .OrderBy( a => a.name )
                                       .ToList();
#else
            return null;
#endif
        }

        #endregion
    }
}