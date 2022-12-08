using System;
using System.Collections.Generic;

namespace Universe
{
    using static AppDomain;

    public class USpreadsheetLinkData : UniverseScriptableObject
    {
        #region Public

        public string m_nameForExport;
        public USpreadsheetProvider m_spreadsheetProvider;

        public string GetSpreadsheet() => m_spreadsheetProvider.GetSpreadsheet( this );
        public override string ToString() => $"[USpreadsheetData] {m_nameForExport}";

        public static IEnumerable<Type> GetAllSubclassOf( Type parent )
        {
            foreach( var a in CurrentDomain.GetAssemblies() )
            {
                foreach( var t in a.GetTypes() )
                {
                    if( t.IsSubclassOf( parent ) )
                        yield return t;
                }
            }
        }

        #endregion
    }
}