using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using static UnityEditor.EditorGUILayout;
using static Universe.Editor.RefreshLanguageSettings;

namespace Universe.Editor
{
    public class RefreshLanguageSettingsRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateRefreshLanguageSettingsProvider()
        {
            var provider = new SettingsProvider( "Project/RefreshLanguageSettings", SettingsScope.Project )
            {
                label = "Refresh Language",

                guiHandler = ( searchContext ) =>
                {
                    SerializedObject refSettings = GetRefreshSettingsReference();

                    Space( 15 );

                    CreateObjectField( refSettings, typeof( USpreadsheetLinkData ), "m_spreadsheetLinkData", "Web URL Spreadsheet Link data" );
                    CreateObjectField( refSettings, typeof( FontSettingsTable ), "m_defaultFontSettings", "Default font settings when not set" );

                    Space( 15 );

                    CreatePropertyField( refSettings, "m_folderInputPath", "Path of the spreadsheetLinkData" );
                    CreatePropertyField( refSettings, "m_folderOutputPath", "Ouput folder of the language data" );

                    Space( 15 );

                    CreatePropertyField( refSettings, "m_localisationTabs", "Spreadsheet tabs containing localisation" );

                    refSettings.ApplyModifiedProperties();

                    var input = Event.current;
                    if( input.keyCode != KeyCode.Return ) return;

                },

                deactivateHandler = () =>
                {
                },

                activateHandler = ( searchContext, rootElement ) =>
                {
                },

                keywords = new HashSet<string>( new[]    {          "Refresh", "refresh",
                                                                    "Language", "language",
                                                                    "Settings", "settings"
                                                                } )
            };

            return provider;
        }

        #region Utilities

        private static SerializedObject GetRefreshSettingsReference()
        {
            List<RefreshLanguageSettings> settings = GetOrCreateRefreshLanguagesSettings();

            var index = m_selectedIndex;
            m_selectedIndex = Popup( "Refresh Language Settings", index, settings.Select( a => a.name ).ToArray() );

            var refSettings = new SerializedObject( settings[index] );
            return refSettings;
        }

        private static void CreatePropertyField( SerializedObject refSettings, string propertyName, string label )
        {
            PropertyField( refSettings.FindProperty( propertyName ), new GUIContent( label ) );
        }

        private static void CreateObjectField( SerializedObject refSettings, System.Type objType, string propertyName, string label  )
        {
            var objValue = refSettings.FindProperty( propertyName ).objectReferenceValue;

            BeginHorizontal();
            LabelField( label );
            ObjectField( objValue, objType, false );
            EndHorizontal();
        }

        private static List<RefreshLanguageSettings> GetOrCreateRefreshLanguagesSettings()
        {
            var settings = GetAllRefreshLanguageSettingsAssets<RefreshLanguageSettings>();

            if( settings.Count == 0 )
            {
                settings = new List<RefreshLanguageSettings>();
                settings.Add( GetOrCreateSettings() );
            }

            return settings;
        }

        #endregion
    }
}