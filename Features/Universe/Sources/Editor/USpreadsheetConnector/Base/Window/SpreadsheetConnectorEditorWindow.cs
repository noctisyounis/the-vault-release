using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Universe
{
    using static GUILayout;
    using static EditorGUILayout;
    using static Debug;
    using static GUI;
    using static AssetDatabase;
    using static UnityEngine.TextAnchor;
    using static USpreadsheetSaver;
    using static System.DateTime;

    public class SpreadsheetConnectorEditorWindow : EditorWindow
    {
        #region Public

        [Header( "Texts" )]
        public static string m_windowName = "Spreadsheet Connector";
        public static string m_helpBoxSpreadsheetSelection = "<color=#ffa500ff>Select Spreadsheet(s) To Download</color>";
        public static string m_helpBoxSaveLocation = "<color=#ffa500ff>Local Save Settings</color>";
        public static string m_refreshButtonText = "Find / Refresh Spreadsheet";
        public static string m_useDefaultNamingText = "Version Name is the Date";
        public static string m_customNameText = "Custom Version Name";

        [Header( "Spacing" )]
        public static int m_spaceBetweenSection = 25;

        #endregion


        #region System

        private void Awake() => FindRefreshSpreadsheets();

        private void OnGUI()
        {
            DrawTitle();

            EditorGUILayout.Space( m_spaceBetweenSection );

            DrawSpreadsheetSelection();

            EditorGUILayout.Space( m_spaceBetweenSection );

            DrawSaveLocation();

            EditorGUILayout.Space( m_spaceBetweenSection );

            DrawDownloadButton();
        }

        #endregion


        #region GUI

        private void DrawTitle()
        {
            titleContent = new GUIContent( m_windowName );
            Label( m_windowName, GetTitleStyle() );
            _isDebug = Toggle("Debug", _isDebug);
        }

        private static void DrawSpreadsheetSelection()
        {
            Box(m_helpBoxSpreadsheetSelection, GetHelpBoxStyle());

            if (Button(m_refreshButtonText)) FindRefreshSpreadsheets(_isDebug);

            if (IsCollectionToDownloadNull()) return;

            CreateAllSpreadsheetsToggle();
        }

        private static void CreateAllSpreadsheetsToggle()
        {
            for (var i = 0; i < _collectionToDownload.Count; i++)
            {
                if( IsDataNullAtIndex( i ) ) continue;

                CreateToggleForSpreadsheetAt( i );
            }
        }

        private static void DrawSaveLocation()
        {
            Box( m_helpBoxSaveLocation, GetHelpBoxStyle() );

            _useDefaultNaming = Toggle( m_useDefaultNamingText, _useDefaultNaming );
            if( !_useDefaultNaming )
            {
                _userCustomVersionName = TextField( m_customNameText, _userCustomVersionName );
            }
        }

        private static void DrawDownloadButton()
        {
            if( Button( "Download" ) )
            {
                if (IsCollectionToDownloadNull()) return;

                GetAllSpreadheetToDownload();
            }
        }

        private static SpreadsheetSettings CreateToggleDownload(SpreadsheetSettings spreadsheetSettings)
        {
            var spreadsheet = spreadsheetSettings;
            spreadsheet.m_downloadable = Toggle(spreadsheet.m_data.name, spreadsheet.m_downloadable);
            spreadsheetSettings = spreadsheet;

            return spreadsheetSettings;
        }

        #endregion


        #region Callback

        private static string OnSpreadsheetGet( USpreadsheetLinkData data, string response )
        {
            if( _isDebug ) Log( $"response = {response}" );

            var folderName = _useDefaultNaming ? _timestamp.ToString() : _userCustomVersionName;
            SaveSheetInTxt( response, data, data.m_nameForExport, folderName );

            return response;
        }

        #endregion


        #region Style

        private static GUIStyle GetTitleStyle()
        {
            var titleStyle = skin.GetStyle( "Label" );
            titleStyle.alignment = UpperCenter;

            return titleStyle;
        }

        private static GUIStyle GetHelpBoxStyle()
        {
            var helpBoxStyle = skin.GetStyle( "Box" );
            helpBoxStyle.richText = true;

            return helpBoxStyle;
        }

        #endregion


        #region Utilities

        private static void FindRefreshSpreadsheets(bool debug = false)
        {
            var sheets = GetAllInstances<USpreadsheetLinkData>();

            _collectionToDownload = new List<SpreadsheetSettings>();

            if (debug) Log($"sheets = {sheets.Length}");
            for (int i = 0; i < sheets.Length; i++)
            {
                if (debug) Log($"   sheet {i} = {sheets[i].name}");
                _collectionToDownload.Add(new SpreadsheetSettings(true, sheets[i]));
            }
        }

        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            var guids = FindAssets( $"t:{typeof( T ).Name}" );
            var a = new T[guids.Length];

            for( int i = 0; i < guids.Length; i++ )
            {
                var path = GUIDToAssetPath( guids[i] );
                a[i] = LoadAssetAtPath<T>( path );
            }

            return a;
        }
        
        private static void GetAllSpreadheetToDownload()
        {
            SetTimeStampVariable();

            SpreadsheetSettings spreadsheetToDownload;
            for (var i = 0; i < _collectionToDownload.Count; i++)
            {
                spreadsheetToDownload = _collectionToDownload[i];

                if (!spreadsheetToDownload.m_downloadable) continue;

                GetSpreadsheet(spreadsheetToDownload);
            }
        }

        private static void GetSpreadsheet(SpreadsheetSettings spreadsheetToDownload)
        {
            var spreadsheetData = spreadsheetToDownload.m_data;

            spreadsheetData.m_spreadsheetProvider.OnCompleteEvent.Add( OnSpreadsheetGet );
            spreadsheetData.GetSpreadsheet();
        }

        private static void CreateToggleForSpreadsheetAt( int index )
        {
            _collectionToDownload[index] = CreateToggleDownload( _collectionToDownload[index] );
        }

        private static void SetTimeStampVariable()
        {
            _timestamp = Now.ToString()
                            .Replace( "/", "." )
                            .Replace( " ", "_" )
                            .Replace( ":", "." );
        }

        private static bool IsCollectionToDownloadNull() => _collectionToDownload == null;
        private static bool IsDataNullAtIndex( int i ) => _collectionToDownload[i].m_data == null;

        #endregion


        #region Private

        private static bool _isDebug;
        private static bool _useDefaultNaming = true;
        private static string _userCustomVersionName;
        private static string _timestamp;

        private static List<SpreadsheetSettings> _collectionToDownload;

        #endregion
    }
}