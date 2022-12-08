using UnityEditor;
using UnityEngine;

namespace Universe
{
    public class SpreadsheetConnectorMenuItem : MonoBehaviour
    {
        #region Menu Item

        [MenuItem( "Vault/Content/Spreadsheet Connector" )]
        public static void LaunchSpreadSheetConnector()
        {
            EditorWindow.GetWindow<SpreadsheetConnectorEditorWindow>();
        }

        #endregion
    }
}