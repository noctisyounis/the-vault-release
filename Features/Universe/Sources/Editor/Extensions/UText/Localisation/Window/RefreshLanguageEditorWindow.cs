using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

using static UnityEngine.GUILayout;

namespace Universe.Editor
{
    using static JSONToScriptableEditorWindow;
    using static LanguageGeneratorEditorWindow;
    using static RefreshLanguageSettings;
    using static USpreadsheetSaver;
    using static Directory;
    using static Debug;

    public class RefreshLanguageEditorWindow : EditorWindow
    {
        #region Unity API

        private void OnGUI()
        {
            var space = 25f;
            Label( $"This tool allow you to generate all the language data existing in your localisation spreadsheet. \nIt will put them into {FOLDER_OUTPUT_PATH}." );
            Space( space );

            if( Button( "Refresh All Languages" ) ) RefreshAllLanguages();

            Space( space );
            _isDebug = Toggle( _isDebug, "Debug" );
        }

        #endregion


        #region Main

        public static void RefreshAllLanguages()
        {
            _isDebug = true;

            GetCurrentSettings();
            DownloadLocalisationSpreadsheet();

            _isDebug = false;
        }

        private static string OnSpreadsheetDownloaded( USpreadsheetLinkData data, string response )
        {
            RemoveSpreadsheetDataOnCompleteEventListener( data );
            LogIfDebug( $"localisation spreadsheet retrieved = {response}" );

            var path = SaveSpreadsheetToTxt( data, response );
            
            GenerateSpreadsheetFacts( path );

            var assetPathName = ConvertJSONToScriptable( path );
            var assets = GetAssetsForLocalisation( assetPathName );
            RefreshLanguages( assets );

            DeleteTemporaryFiles();

            Log( $"Languages refresh completed, see above if errors." );

            return response;
        }

        #endregion


        #region Utilities

        private static void RefreshLanguages( List<string> assets )
        {
            m_defaultFontSettings = _currentSettings.m_defaultFontSettings;
            foreach( var asset in assets )
            {
                LogIfDebug( $"Generate language from {asset}" );
                AddToTemporaryFilesList( asset );
                GenerateLanguages( asset, GetOutputFolder() );
            }
        }


        private static List<string> GetAssetsForLocalisation( string assetPathName )
        {
            bool getAllAssets = _currentSettings.m_localisationTabs.Count == 0;

            if( getAllAssets )
                return GetAllAssetFrom( assetPathName );

            List<string> assets = new List<string>();
            foreach( var tabName in _currentSettings.m_localisationTabs )
            {
                assets.Add( $"{assetPathName}{tabName}.asset" );
            }

            return assets;
        }

        private static string ConvertJSONToScriptable( string path )
        {
            var assetPathName = GetOrCreateFolderAt( path );
            LogIfDebug( $"path = {path}, assetPathName = {assetPathName}" );

            return assetPathName;
        }

        private static void GenerateSpreadsheetFacts( string path )
        {
            var pathlist = new List<string>() { path };
            GenerateJSONFact( pathlist );
            LogIfDebug( $"Facts generated" );
        }

        private static string SaveSpreadsheetToTxt( USpreadsheetLinkData data, string response )
        {
            var folderName = GetTimeStamp(); ;
            var path = SaveSheetInTxt( response, data, data.m_nameForExport, folderName );
            AddToTemporaryFilesList( path );

            return path;
        }

        private static void LogIfDebug( string response )
        {
            if( _isDebug ) Log( response );
        }

        private static void GetCurrentSettings()
        {
            var settings = GetAllRefreshLanguageSettingsAssets<RefreshLanguageSettings>();
            _currentSettings = settings[m_selectedIndex];
        }

        private static void DownloadLocalisationSpreadsheet()
        {
            var spreadsheetData = _currentSettings.m_spreadsheetLinkData;
            AddSpreadsheetDataOnCompleteListener( spreadsheetData );
            spreadsheetData.GetSpreadsheet();
        }

        private static List<string> GetAllAssetFrom( string sourceDirectoryPath )
        {
            var content = new List<string>();

            try
            {
                content.AddRange( GetAllAssetFiles( sourceDirectoryPath ) );

                foreach( var directory in GetDirectories( sourceDirectoryPath ) )
                {
                    content.AddRange( GetAllAssetFrom( directory ) );
                }
            }
            catch( Exception exception )
            {
                LogError( $"[Refresh languages]::GetAllSheetContent => {exception.Message}" );
            }

            return content;
        }

        private static void DeleteTemporaryFiles()
        {
            foreach( var file in _tempFiles )
            {
                var dirToDel = file.Substring( 0, file.LastIndexOf( '/' ) );

                LogIfDebug( $"delete dir {dirToDel}" );

                if( File.Exists( $"{dirToDel}.meta" ) ) File.Delete( $"{dirToDel}.meta" );
                if( Exists( dirToDel ) ) Delete( dirToDel, true );
            }

            _tempFiles.Clear();
        }

        private static void AddToTemporaryFilesList( string path )
        {
            if( _tempFiles.IndexOf( path ) == -1 ) _tempFiles.Add( path );
        }

        private static string GetTimeStamp() => DateTime.Now.ToString()
                            .Replace( "/", "." )
                            .Replace( " ", "_" )
                            .Replace( ":", "." );

        private static IEnumerable<string> GetAllAssetFiles( string sourceDirectoryPath ) =>
            GetFiles( sourceDirectoryPath ).Where( file => file.Contains( ".asset" ) && !file.Contains( ".meta" ) );

        private static void RemoveSpreadsheetDataOnCompleteEventListener( USpreadsheetLinkData spreadsheetData ) =>
            spreadsheetData.m_spreadsheetProvider.OnCompleteEvent.Remove( OnSpreadsheetDownloaded );

        private static void AddSpreadsheetDataOnCompleteListener( USpreadsheetLinkData spreadsheetData ) =>
            spreadsheetData.m_spreadsheetProvider.OnCompleteEvent.Add( OnSpreadsheetDownloaded );

        private static string GetOutputFolder() => _currentSettings == null || string.IsNullOrEmpty( _currentSettings.m_folderOutputPath ) ? FOLDER_OUTPUT_PATH : _currentSettings.m_folderOutputPath;


        #endregion


        #region Private Members

        private static bool _isDebug;
        private static RefreshLanguageSettings _currentSettings;

        private static List<string> _tempFiles = new List<string>();

        private const string FOLDER_OUTPUT_PATH = "Assets/Datas/Localisation/";

        #endregion
    }
}