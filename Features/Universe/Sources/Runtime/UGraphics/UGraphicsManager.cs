using UnityEngine;

namespace Universe
{
    public class UGraphicsManager : UBehaviour
    {
        #region Public

        public UGraphicsSettings m_settings;
        private static UGraphicsSettings s_settings;
        public static UGraphicsSettings GetSettings() => s_settings;
        
        public UAssetsPathTable m_pathTable;
        private static UAssetsPathTable s_pathTable;
        public static UAssetsPathTable GetPathTable() => s_pathTable;

        #endregion


        #region Unity API

        private void Awake() 
        {
            s_settings = m_settings;
            s_pathTable = m_pathTable;
        }

        #endregion
    }
}