using System;
using System.Collections.Generic;
using UnityEngine;

using static Universe.TextServices;

namespace Universe
{
    [Serializable]
    public class FontSettingsTable : FactBase
    {
        #region Public

        [HideInInspector]
        public string[] m_names;
        [HideInInspector]
        public List<FontSettings> m_list;

        #endregion


        #region Unity API

        private void Awake()
        {
            var enumNames = GetListOfEnumNames();

            if( IsNotInitialized() )
            {
                InitializeFontSettingsCollection( enumNames );
            }

            AddOrRemoveEntriesInList( enumNames );
        }

        private void OnValidate() => RefreshTexts();

        #endregion


        #region Private

        private void AddOrRemoveEntriesInList( string[] enumNames )
        {
            var enumCount = enumNames.Length;
            var nameListCount = m_names.Length;

            if( enumCount == nameListCount ) return;

            if( enumCount > nameListCount )
            {
                AddMissingEntries( enumCount, nameListCount );
            }
            else if( enumCount < nameListCount )
            {
                RemoveUnnecessaryEntries( enumCount, nameListCount );
            }
        }
        
        private void AddMissingEntries( int enumCount, int nameListCount )
        {
            var numberOfMissingEntries = enumCount - nameListCount;
            for( int i = 0; i < numberOfMissingEntries; i++ ) m_list.Add( null );
        }

        private void RemoveUnnecessaryEntries( int enumCount, int nameListCount )
        {
            var numberOfUselessEntries = nameListCount - enumCount;
            m_list.RemoveRange( nameListCount - numberOfUselessEntries, numberOfUselessEntries );
        }

        private void InitializeFontSettingsCollection( string[] enumNames )
        {
            m_names = enumNames;
            m_list = new List<FontSettings>( m_names.Length );
        }

        private bool IsNotInitialized() => m_list == null || m_list.Count == 0;
        private static string[] GetListOfEnumNames() => Enum.GetNames( typeof( FontSettingsType ) );

        #endregion


        #region Extension

        public override string ToString() => $"[FontSettingsCollection] count = {m_list.Count}";
        public bool ListIsNullOrEmpty() => m_list == null || m_list.Count == 0;

        #endregion
    }
}