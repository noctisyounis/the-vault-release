using System;
using UnityEngine;
using Universe;

namespace Universe
{
    public class CISettings : UniverseScriptableObject
    {
        #region Exposed

        public string m_buildTime;
        public string m_version;
        public int m_androidBundleCode;

        #endregion
    }
}