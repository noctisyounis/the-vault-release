using UnityEngine;

#if UNITY_EDITOR
using static System.IO.Path;
using static System.IO.File;
using static UnityEditor.AssetDatabase;
#endif

namespace Universe
{
    public class UGraphicsSettings : USettings
    {
        #region Public 

        public string m_rootFolder = "Assets/_/Content/Graphics Tiers";
        public string m_targetFolder = "Assets/_/Content/Graphics Tiers/HD";
        public string m_fallbackFolder = "Assets/_/Content/Graphics Tiers/Placeholder";
        public UAssetsPathTable m_pathTable;
        public string[] m_existingFolders;

        #endregion


        #region Main

        public UAssetsPathTable GetPathTable()
        {
            if(m_pathTable) return m_pathTable;

            FindPathTable();

            return m_pathTable;
        }

        public void FindPathTable()
        {
#if UNITY_EDITOR

            var path        = Join(m_rootFolder, $"{nameof(UAssetsPathTable)}.asset");
            var fullPath    = GetFullPath(path);

            if(Exists(fullPath))
            {
                m_pathTable = LoadAssetAtPath<UAssetsPathTable>(path);
                return;
            }

            m_pathTable = CreateInstance<UAssetsPathTable>();
            m_pathTable.m_targetFolder = m_rootFolder;
            m_pathTable.Populate();

            CreateAsset(m_pathTable, path);

            Debug.LogWarning($"UAssetPathTable missing, {m_pathTable.name} was created in {m_rootFolder}");

#else

            Debug.LogError($"UAssetPathTable missing");

#endif
        }

        #endregion
    }
}