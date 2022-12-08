using UnityEngine;

namespace Universe
{
    public class UGraphicsManager : UBehaviour
    {
        #region Public

        public UGraphicsSettings m_settings;
        private static UGraphicsSettings s_settings;
        public static UGraphicsSettings GetSettings() => s_settings;
        private static UAssetsPathTable s_pathTable;
        public static UAssetsPathTable GetPathTable() => s_pathTable;

        #endregion


        #region Unity API

        public override void Awake()
        {
            base.Awake();
            s_settings = m_settings;
            s_pathTable = m_settings.m_pathTable;
        }

        #endregion
    }
}