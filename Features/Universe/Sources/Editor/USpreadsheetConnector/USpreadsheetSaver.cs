using System.IO;
using UnityEngine;

namespace Universe
{
    public class USpreadsheetSaver
    {
        public static string SaveSheetInTxt( string content, USpreadsheetLinkData data, string title, string nameOfFolder )
        {
            string folderPath = Application.dataPath + "/../../Datas/GoogleSpreadsheet/" + nameOfFolder;

            if( !Directory.Exists( folderPath ) )
            {
                Directory.CreateDirectory( folderPath );
            }

            //folderPath += "/" + data.m_nameForExport;
            if( !Directory.Exists( folderPath ) )
            {
                Directory.CreateDirectory( folderPath );
            }

            var finalPath = folderPath + "/" + title + ".txt";
            File.WriteAllText( finalPath, content );
            Debug.Log( "File saved, path : " + finalPath );

            return finalPath;
        }
    }
}