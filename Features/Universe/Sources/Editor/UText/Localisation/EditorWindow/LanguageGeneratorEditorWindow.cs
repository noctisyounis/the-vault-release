using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using static UnityEngine.GUILayout;
using static UnityEditor.AssetDatabase;
using static Universe.UFile;
using static UnityEditor.EditorUtility;
using static UnityEngine.Debug;

namespace Universe.Editor
{
    public class LanguageGeneratorEditorWindow : EditorWindow
    {
        #region Unity API

        private void OnGUI()
        {
            var space = 25f;
            Label( $"This tool allow you to generate all the language data existing in your localisation spreadsheet. \nIt will put them into {_folderOutputPath}." );
            Space( space );

            if( Button( "Generate Language" ) ) GenerateAllLanguages();

            Space( space );
            _isDebug = Toggle( _isDebug, "Debug" );
        }

        #endregion


        #region Public

        public static FontSettingsTable m_defaultFontSettings;

        public static void GenerateLanguages(string inputFolder, string outputFolder)
        {
            var window = CreateInstance<LanguageGeneratorEditorWindow>();

            window._folderOutputPath = outputFolder;
            UGetOrCreateFolderAt( outputFolder );

            var locAsset = GetSpreadsheeDataAssetIn( inputFolder );
            if( IsNull( locAsset ) )
            {
                window.DisplayError( $"No SpreadsheetData found at {inputFolder}. Make sure to select a .asset typed as SpreadsheetData." );
                return;
            }

            var lineCount = GetLocalizedLineCount( locAsset );
            for( int i = 0; i < lineCount; i++ )
            {
                var line = GetLineAtIndex( locAsset, i );
                if(!window.GetAllDataFromLine( line ))
                {
                    window.DisplayError( string.Format( $"{SpreadsheetLineData.ERROR_NO_COLUMN} in {locAsset.name}.", ENTRY_COLUMN_ID ) );
                    break;
                }
            }

            DestroyImmediate(window);
            SaveAndRefreshAssetDatabase();
        }

        #endregion


        #region Main

        private void GenerateAllLanguages()
        {
            UGetOrCreateFolderAt( _folderOutputPath );

            var inputFolder = UGetPathRelativeToProject( SelectInputSpreadsheetData() );
            if( IsStringNullOrEmpty( inputFolder ) ) return;

            LogIfDebug( $"input folder = {inputFolder}" );

            var locAsset = GetSpreadsheeDataAssetIn( inputFolder );
            if( IsNull( locAsset ) )
            {
                DisplayError( $"No SpreadsheetData found at {inputFolder}. Make sure to select a .asset typed as SpreadsheetData." );
                return;
            }

            var lineCount = GetLocalizedLineCount( locAsset );
            for( int i = 0; i < lineCount; i++ )
            {
                var line = GetLineAtIndex( locAsset, i );
                if( !GetAllDataFromLine( line ) )
                {
                    DisplayError( string.Format( $"{SpreadsheetLineData.ERROR_NO_COLUMN} in {locAsset.name}.", ENTRY_COLUMN_ID ) );
                    break;
                }
            }

            SaveAndRefreshAssetDatabase();
        }

        private bool GetAllDataFromLine(SpreadsheetLineData line)
        {
            var locKey = line.GetIfExist( ENTRY_COLUMN_ID );

            if(locKey == SpreadsheetLineData.ERROR_NO_COLUMN)
                return false;

            if( IsStringNullOrEmpty( locKey ) )
            {
                DisplayError( $"Column {ENTRY_COLUMN_ID} is empty in SpreadsheetData at line {line.ToString()}" );
                return true;
            }

            var entryCount = line.m_entries.Count;
            for( int i = 0; i < entryCount; i++ )
            {
                var entry = line.m_entries[i];
                GetDataFromEachEntries( locKey, entry );
            }
            return true;
        }

        private void GetDataFromEachEntries( string locKey, SpreadsheetEntry entry )
        {
            if( IsNotUniqueID( entry ) )
            {
                var languageDataName = entry.m_column;
                var languageDataPath = $"{_folderOutputPath}{languageDataName}.asset";
                var languageData = GetOrCreateLanguageData( languageDataName, languageDataPath );
                var existingLine = GetAssetWithLocKey( locKey, languageDataPath );

                if( LineDoesntExist( existingLine ) )
                {
                    var localisedLine = CreateNewLocalisedString( locKey, entry, languageData );
                    AddObjectToAsset( localisedLine, languageDataPath );

                    LogIfDebug( $"ADDED: {locKey} \n TO: {languageDataName} WITH: {entry.m_value}" );
                }
                else
                {
                    existingLine.Value = entry.m_value;
                    EditorUtility.SetDirty( existingLine );

                    LogIfDebug( $"UPDATED: {locKey} \n TO: {languageDataName} WITH: {entry.m_value}" );
                }
            }
        }

        #endregion


        #region Utilities

        private static void SaveAndRefreshAssetDatabase()
        {
            SaveAssets();
            Refresh();
        }

        private static StringFact CreateNewLocalisedString( string locKey, SpreadsheetEntry entry, LanguageData languageData )
        {
            var localisedLine = CreateInstance<StringFact>();

            localisedLine.name = locKey;
            localisedLine.Value = entry.m_value;
            localisedLine.m_washOnAwakeAndCompilation = false;
            languageData.m_localisedTexts.Add( localisedLine );

            return localisedLine;
        }

        private static StringFact GetAssetWithLocKey( string locKey, string languageDataPath )
        {
            var existingLines = LoadAllAssetsAtPath( languageDataPath );
            var correspondance = existingLines.FirstOrDefault( x => x.name == locKey ) as StringFact;

            return correspondance;
        }

        private LanguageData GetOrCreateLanguageData( string languageDataName, string languageDataPath )
        {
            LanguageData languageData;
            if( _dicoLanguages.ContainsKey( languageDataName ) )
            {
                languageData = _dicoLanguages[languageDataName];
            }
            else
            {
                languageData = CreateOrLoadLanguageData( languageDataName, languageDataPath );
            }

            if(m_defaultFontSettings != null && languageData.m_fontSettingsCollection == null)
            {
                languageData.m_fontSettingsCollection = m_defaultFontSettings;
            }

            return languageData;
        }

        private LanguageData CreateOrLoadLanguageData( string languageDataName, string languageDataPath )
        {
            LanguageData languageData;
            if( FindAssets( $"{languageDataName} t:LanguageData", new[] { _folderOutputPath } ).Length == 0 )
            {
                languageData = CreateInstance<LanguageData>();

                languageData.name = languageDataName;
                languageData.m_name = languageDataName;
                languageData.m_localisedTexts = new List<StringFact>();
                CreateAsset( languageData, languageDataPath );

                LogIfDebug( $"CREATE new LanguageData {languageDataName}" );
            }
            else
            {
                languageData = LoadAssetAtPath( languageDataPath, typeof( LanguageData ) ) as LanguageData;
            }

            _dicoLanguages.Add( languageDataName, languageData );

            return languageData;
        }

        private static string SelectInputSpreadsheetData() => OpenFilePanel( "Localisation SpreadsheeData", FOLDER_INPUT_PATH, "" );
        private static SpreadsheetLineData GetLineAtIndex( SpreadsheetData locAsset, int i ) => locAsset.m_lines[i];
        private static SpreadsheetData GetSpreadsheeDataAssetIn( string inputFolder ) => LoadAssetAtPath( inputFolder, typeof( SpreadsheetData ) ) as SpreadsheetData;
        private static int GetLocalizedLineCount( SpreadsheetData locAsset ) => locAsset.m_lines.Count;
        private static bool LineDoesntExist( StringFact correspondance ) => IsNull( correspondance );
        private static bool IsNull( Object obj ) => obj == null;
        private static bool IsStringNullOrEmpty( string locKey ) => string.IsNullOrEmpty( locKey );
        private static bool IsNotUniqueID( SpreadsheetEntry entry ) => entry.m_column != ENTRY_COLUMN_ID;

        private void LogIfDebug( string message )
        {
            if( _isDebug ) Log( $"[LanguageGenerator] {message}" );
        }

        private void DisplayError( string message )
        {
            LogError( $"[LanguageGenerator] ERROR ! {message}" );
        }

        #endregion


        #region Private

        private Dictionary<string, LanguageData> _dicoLanguages = new Dictionary<string, LanguageData>();
        private bool _isDebug;

        private const string FOLDER_INPUT_PATH = "/../../Datas/Spreadsheet/";
        private string _folderOutputPath = "Assets/Datas/Localisation/";
        private const string ENTRY_COLUMN_ID = "UNIQUE_ID";

        #endregion
    }
}