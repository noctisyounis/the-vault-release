using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEngine.Networking;

namespace Universe.GoogleDriveSpreadsheetConnector.Editor
{
    using static Debug;
    using static UnityWebRequest;
    using static EditorCoroutineUtility;
    using static UnityWebRequest.Result;

    public class GoogleDriveSpreadsheetProvider : USpreadsheetProvider
    {
        #region Main

        override public string GetSpreadsheet( USpreadsheetLinkData data )
        {
            if (IsValidDataType(data))
            {
                LogError("ERROR spreadsheet data Provided is not a WebSpreadsheetData !");
                return null;
            }

            StartDownloadCoroutine(data);

            return data.m_nameForExport;
        }

        private IEnumerator DownloadSheet(USpreadsheetLinkData spreadsheetData, string url)
        {
            Log($"DownloadSheet {url}");
            using (var webRequest = Get(url))
            {
                yield return webRequest.SendWebRequest();

                if (HasConnectionError(webRequest))
                {
                    ShowWebRequestError(url, webRequest);
                }
                else
                {
                    CompleteWebRequest(spreadsheetData, webRequest);
                }
            }
        }

        #endregion


        #region Utilities

        private static bool IsValidDataType(USpreadsheetLinkData data) => data.GetType() != typeof(WebURLSpreadsheetLinkData);
        private static bool HasConnectionError(UnityWebRequest webRequest) => webRequest.result == ConnectionError;

        private void StartDownloadCoroutine(USpreadsheetLinkData data)
        {
            var webData = data as WebURLSpreadsheetLinkData;
            var url = webData.GetConstructedUrl();
            StartCoroutineOwnerless(DownloadSheet(webData, url));
        }

        private static void ShowWebRequestError(string url, UnityWebRequest webRequest)
        {
            var pages = url.Split('/');
            var page = pages.Length - 1;
            LogError($"{pages[page]}: Error: {webRequest.error}");
        }

        private void CompleteWebRequest(USpreadsheetLinkData spreadsheetData, UnityWebRequest webRequest)
        {
            var response = webRequest.downloadHandler.text;
            OnCompleteEvent.Invoke( spreadsheetData, response );
        }

        public override string ToString() => $"[GoogleDriveSpreadsheetProvider] {name}";

        #endregion
    }
}