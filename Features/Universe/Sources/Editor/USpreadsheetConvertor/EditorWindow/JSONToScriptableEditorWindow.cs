using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Universe.Editor
{
    using static Application;
    using static AssetDatabase;
    using static Color;
    using static Debug;
    using static Directory;
    using static EditorGUILayout;
    using static EditorGUIUtility;
    using static EditorUtility;
    using static GUILayout;
    using static JSONObject;
    using static Path;

    public class JSONToScriptableEditorWindow : EditorWindow
    {
        #region System

        private void OnGUI()
        {
            titleContent = new GUIContent( TITLE );

            CreateStyles();

            LabelField( INTRO, _introStyle );

            EditorGUILayout.Space( VERTICAL_SPACE );
            LabelField( _folderOutputPath );
            CreateChangeOutputFolderButton();

            EditorGUILayout.Space( VERTICAL_SPACE );
            CreateConversionButton();
        }

        #endregion


        #region GUI

        private void CreateStyles()
        {
            GUIStyle skinLabel = GUI.skin.GetStyle( "label" );
            _introStyle = new GUIStyle( skinLabel )
            {
                wordWrap = true,
                normal = { textColor = isProSkin ? white : black }
            };
        }

        private static void CreateChangeOutputFolderButton()
        {
            if( !Button( CHANGE_OUTPUT_FOLDER_BUTTON_LABEL ) ) return;

            _folderOutputPath = OpenFolderPanel( CHANGE_OUTPUT_FOLDER_BUTTON_LABEL, _folderOutputPath, "" );
            _folderOutputPath = AppendSlashTo( _folderOutputPath );
            _folderOutputPath = RemoveDataPathFrom( _folderOutputPath );
        }

        private static void CreateConversionButton()
        {
            if ( !Button( CONVERSION_BUTTON_LABEL ) ) return;

            string sourceDirectoryPath = OpenFolderPanel( FOLDER_PANEL_TITLE, FOLDER_INPUT_PATH, "" );
            var paths = GetAllSheetPathFrom( sourceDirectoryPath );
            GenerateJSONFact( paths );
        }

        #endregion


        #region Main

        private static List<string> GetAllSheetPathFrom( string sourceDirectoryPath )
        {
            var content = new List<string>();

            try
            {
                content.AddRange( GetAllNonMetaFiles( sourceDirectoryPath ) );

                foreach( var directory in GetDirectories( sourceDirectoryPath ) )
                {
                    content.AddRange( GetAllSheetPathFrom( directory ) );
                }
            }
            catch ( Exception exception )
            {
                LogError( $"[JSON To Scriptable]::GetAllSheetContent => {exception.Message}" );
            }

            return content;
        }

        private static void GenerateJSONFact( List<string> paths )
        {
            var spreadsheetDico = new Dictionary<string, SpreadsheetData>();

            foreach ( var path in paths )
            {
                var folderPath = GetOrCreateFolderAt( path );
                var json = CreateJSONObjectFrom( path );

                foreach ( var sheetName in json.keys )
                {
                    var spreadsheetData = CreateSpreadsheetData( sheetName );

                    var assetPathName = $"{folderPath}{spreadsheetData.name}{ASSET_EXTENSION}";

                    CreateAsset( spreadsheetData, assetPathName );
                    spreadsheetDico.Add( assetPathName, spreadsheetData );

                    var count = 1;
                    var lines = GetJSONLinesFrom( json, sheetName );
                    foreach ( var line in lines )
                    {
                        CreateAndPopulateLineData( spreadsheetDico, assetPathName, line, ref count );
                    }
                }
            }

            SaveAssets();
            Refresh();
        }

        #endregion


        #region Scriptable Utilities

        private static SpreadsheetData CreateSpreadsheetData( string sheetName )
        {
            var spreadsheetData = CreateInstance<SpreadsheetData>();

            spreadsheetData.name = GetFileNameWithoutExtension( sheetName );
            spreadsheetData.m_lines = new List<SpreadsheetLineData>();

            return spreadsheetData;
        }

        private static void CreateAndPopulateLineData( Dictionary<string, SpreadsheetData> spreadsheetDictionary, string assetPathName, JSONObject line, ref int count )
        {
            SpreadsheetLineData sheetData = CreateLineData( ref count );

            AddEntriesToSheetData( assetPathName, sheetData, line );
            AddLineToSpreadsheetData( spreadsheetDictionary, assetPathName, sheetData );
        }

        private static SpreadsheetLineData CreateLineData( ref int count )
        {
            var sheetData = CreateInstance<SpreadsheetLineData>();

            sheetData.name = count.ToString();
            count++;

            return sheetData;
        }

        private static void AddEntriesToSheetData( string assetPathName, SpreadsheetLineData sheetData, JSONObject line )
        {
            var jsonLine = Create( line.ToString() );
            sheetData.m_entries = new List<SpreadsheetEntry>();

            foreach ( var keyValuePair in jsonLine.ToDictionary() )
            {
                if ( keyValuePair.Key == COLUMN_UNIQUE_ID )
                {
                    sheetData.name = keyValuePair.Value;
                }

                SpreadsheetEntry entry = new SpreadsheetEntry { m_column = keyValuePair.Key, m_value = keyValuePair.Value };
                sheetData.m_entries.Add( entry );
            }

            AddObjectToAsset( sheetData, assetPathName );
        }

        private static void AddLineToSpreadsheetData( Dictionary<string, SpreadsheetData> spreadsheetDictionary, string assetPathName, SpreadsheetLineData sheetData )
        {
            var spreadsheet = spreadsheetDictionary[assetPathName];
            if( spreadsheet != null )
            {
                spreadsheet.m_lines.Add( sheetData );
            }
        }

        #endregion


        #region Utilities

        private static string RemoveDataPathFrom( string path )
        {
            if ( path.StartsWith( dataPath ) )
            {
                path = $"Assets{path.Remove( 0, dataPath.Length )}";
            }

            return path;
        }

        private static string AppendSlashTo( string path )
        {
            if( path.Last() == '/' ) return path;

            return $"{path}/";
        }

        private static string GetOrCreateFolderAt( string path )
        {
            var folderPath = $"{_folderOutputPath}/{GetFileNameWithoutExtension( path )}";

            CreateDirIfNotExist( folderPath );

            return $"{folderPath}/";
        }

        private static void CreateDirIfNotExist( string folderPath )
        {
            if( IsNotSystemPath( folderPath ) )
            {
                folderPath = RemoveAssetsPathFrom( folderPath );
            }

            if( !Exists( folderPath ) )
            {
                CreateDirectory( folderPath );
            }
        }

        private static string GetAllTextContentFrom( string path )
        {
            string content;

            var reader = new StreamReader( path );
            content = reader.ReadToEnd();
            reader.Close();

            return content;
        }

        private static List<JSONObject> GetJSONLinesFrom( JSONObject json, string sheetName )
        {
            var jsonContent = GetJSONContentFrom( json, sheetName );
            var lines = GetLinesFrom( jsonContent );

            return lines;
        }

        private static JSONObject CreateJSONObjectFrom( string path )
        {
            var content = GetAllTextContentFrom( path );
            var json = Create( content );

            return json;
        }

        private static string RemoveAssetsPathFrom( string folderPath ) => $"{dataPath.Remove( dataPath.Length - 6, 6 )}{folderPath}";
        private static bool IsNotSystemPath( string folderPath ) => folderPath[1] != ':' && folderPath[2] != '/';
        private static List<JSONObject> GetLinesFrom( JSONObject jsonContent ) => Create( jsonContent.ToString() ).list;
        private static JSONObject GetJSONContentFrom( JSONObject json, string sheetName ) => json.GetField( sheetName );
        private static IEnumerable<string> GetAllNonMetaFiles( string sourceDirectoryPath ) =>
            GetFiles( sourceDirectoryPath ).Where( file => !file.Contains( ".meta" ) );

        #endregion


        #region Private

        private const string TITLE = "JSON To Scriptable";
        private const string INTRO = "This tool will take a parent/source folder and find every txt file in it recursively and will output a scriptable version of all content regardless of spreadsheet validity";

        private const string CHANGE_OUTPUT_FOLDER_BUTTON_LABEL = "Change Output destination";
        private const string CONVERSION_BUTTON_LABEL = "Convert Folder Content To Scriptable";
        private const string FOLDER_PANEL_TITLE = "Folder To Convert";
        private const string FOLDER_INPUT_PATH = "/../../Datas/Spreadsheet/";

        private const string COLUMN_UNIQUE_ID = "UNIQUE_ID";

        private const string ASSET_EXTENSION = ".asset";

        private const float VERTICAL_SPACE = 20f;

        private GUIStyle _introStyle;

        private static string _folderOutputPath = "Assets/Datas/Spreadsheet/Output/";

        #endregion
    }
}