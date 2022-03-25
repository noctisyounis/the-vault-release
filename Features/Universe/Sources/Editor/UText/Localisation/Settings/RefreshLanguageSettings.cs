using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

using static System.IO.Directory;
using static UnityEditor.AssetDatabase;
using static UnityEngine.Debug;

namespace Universe.Editor
{
    public class RefreshLanguageSettings : ScriptableObject
    {

        #region Public Members

        public USpreadsheetLinkData m_spreadsheetLinkData;
        [SerializeField]
        public FontSettingsTable m_defaultFontSettings;
        [SerializeField]
        public string m_folderInputPath = "/../../Datas/Spreadsheet/";
        [SerializeField]
        public string m_folderOutputPath = "Assets/Datas/Localisation/";
        [SerializeField]
        public List<string> m_localisationTabs = new List<string>();

        public static int m_selectedIndex = 0;

        #endregion


        #region Constants

        public const string SETTINGS_ASSETS_DIRECTORY = "Assets/Settings/Universe";
        public const string SETTINGS_ASSETS_NAME = "RefreshLanguageSettings.asset";

        #endregion


        internal static RefreshLanguageSettings GetOrCreateSettings()
        {
            var languageSettings = GetAllRefreshLanguageSettingsAssets<RefreshLanguageSettings>();
            
            if(languageSettings.Count == 0)
            {
                var path = $"{SETTINGS_ASSETS_DIRECTORY}\\{SETTINGS_ASSETS_NAME}";
                var settings = LoadAssetAtPath<RefreshLanguageSettings>( path );

                if( settings != null ) return settings;

                return CreateSettingsAt( path );
            }
            else
            {
                return languageSettings[m_selectedIndex];
            }
        }

        private RefreshLanguageSettings CopySettings()
        {
            var settings = CreateInstance<RefreshLanguageSettings>();

            return settings;
        }

        private static RefreshLanguageSettings CreateSettingsAt( string relativePath )
        {
            var settings = CreateInstance<RefreshLanguageSettings>();

            if( !Exists( relativePath ) ) CreateDirectory( relativePath );

            CreateAsset( settings, $"{relativePath}" );
            SaveAssets();
            LogWarning( $"Created new settings at <color=cyan>'{relativePath}\\{SETTINGS_ASSETS_NAME}'</color>", settings );

            return settings;
        }

        #region Properties

        public static SerializedObject SerializedSettings => new SerializedObject( GetOrCreateSettings() );

        #endregion


        #region Utilities

        public static List<T> GetAllRefreshLanguageSettingsAssets<T>() where T : RefreshLanguageSettings
        {
            return FindAssets( $"t:{typeof( T ).Name}" )
                                       .Select( guid => GUIDToAssetPath( guid ) )
                                       .Select( path => LoadAssetAtPath( path, typeof( T ) ) )
                                       .Select( obj => (T)obj )
                                       .OrderBy( a => a.name )
                                       .ToList();
        }

        #endregion
    }
}