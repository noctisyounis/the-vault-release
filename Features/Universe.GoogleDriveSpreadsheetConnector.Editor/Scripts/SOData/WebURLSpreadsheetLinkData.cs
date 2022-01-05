using UnityEngine;

namespace Universe
{
    public class WebURLSpreadsheetLinkData : USpreadsheetLinkData
    {
        #region Public

        [Tooltip( "The Spreadsheet Web Service Url" )]
        public string m_serviceUrl;
        public GETParameterData[] m_GETparameters;

        #endregion


        #region Main Methods

        public string GetConstructedUrl()
        {
            var constructedUrl = $"{m_serviceUrl}?";

            for( var i = 0; i < m_GETparameters.Length; i++ )
            {
                var param = m_GETparameters[i];
                constructedUrl += $"{param.m_name}={param.m_value}&";
            }

            return constructedUrl;
        }

        #endregion
    }
}