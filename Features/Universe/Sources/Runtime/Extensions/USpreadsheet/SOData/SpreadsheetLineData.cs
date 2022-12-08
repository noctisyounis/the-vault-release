using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Universe
{
    using static StringComparison;

    public class SpreadsheetLineData : FactBase
    {
        #region Public

        public List<SpreadsheetEntry> m_entries;

        [NonSerialized]
        public static string ERROR_NO_COLUMN = "/!\\ ERROR no column {0} found.";

        #endregion


        #region Extensions

        public bool Have( string columnName ) =>  CheckIfEntriesHaveColumn( columnName );
        
        public bool DoesntHave( string columnName ) => !CheckIfEntriesHaveColumn( columnName );
        
        public string Get( string columnName, StringComparison stringComparison = CurrentCultureIgnoreCase )=>
            BrowseEntriesToFind( columnName, stringComparison );
        

        public string GetIfExist( string columnName )
        {
            if( DoesntHave( columnName ) ) return ERROR_NO_COLUMN;

            var output = Get( columnName );
            if( string.IsNullOrWhiteSpace(output) )
            {
                Debug.LogError( $"[SpreadsheetLineData] > Can't get data from {columnName}" );
            }

            return output;
        }

        #endregion


        #region Utilities

        private bool CheckIfEntriesHaveColumn( string columnName )
        {
            for( var i = 0; i < m_entries.Count; i++ )
            {
                if( EntryHasColumnName( m_entries[i], columnName ) )
                {
                    return true;
                }
            }

            return false;
        }

        private string BrowseEntriesToFind( string columnName, StringComparison stringComparison )
        {
            for( var i = 0; i < m_entries.Count; i++ )
            {
                if( HasNotColumnName( m_entries[i].m_column, columnName, stringComparison ) ) continue;
                
                return m_entries[i].m_value;
            }

            return null;
        }
        
        public override string ToString()
        {
            var toString = $"[SpreadsheetLineData] {name}";

            for( var i = 0; i < m_entries.Count; i++ ) 
                toString += $"\n {m_entries[i].m_column} = {m_entries[i].m_value}";
            

            return toString;
        }

        private bool HasNotColumnName( string entryColumnName, string columnName, StringComparison stringComparison ) => 
            !string.Equals( entryColumnName, columnName, stringComparison );

        private bool EntryHasColumnName( SpreadsheetEntry entry, string columnName ) => entry.m_column == columnName;

        #endregion
    }
}