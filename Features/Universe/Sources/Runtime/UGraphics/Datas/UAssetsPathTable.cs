using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;

using static UnityEditor.AssetDatabase;
#endif

namespace Universe
{
    public class UAssetsPathTable : UniverseScriptableObject
    {
        #region Exposed

        [Header("Target")]
        public string[] m_targetFolders;

        [Header("Lists")]
        public List<string> m_guids = new();
        public List<string> m_paths = new();

        #endregion


        #region Main

        [Button("Populate")]
        public void Populate()
        {
#if UNITY_EDITOR
            m_paths.Clear();
            m_guids.Clear();

            var allPaths = GetAllAssetPaths();
            
            foreach (var path in allPaths)
            {
                var isTarget = false;
                foreach( var folder in m_targetFolders )
                {
                    if(!path.Contains(folder)) continue;
                    isTarget = true;
                }

                if( !isTarget ) continue;
                if(IsValidFolder(path)) continue;

                var guid = AssetPathToGUID(path);
                
                m_paths.Add(path);
                m_guids.Add(guid);
            }

            SaveAsset();
#endif
        }

        public string PathToGUID(string path)
        {
            var index = m_paths.IndexOf(path);

            try
            {
                var result = m_guids[index];
                return result; 
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string GUIDToPath(string guid)
        {
            var index = m_guids.IndexOf(guid);

            try
            {
                var result = m_paths[index];
                return result; 
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        #endregion
    }
}