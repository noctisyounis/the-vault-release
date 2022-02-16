using UnityEngine;

namespace Universe
{
    public class UGraphicsSettings : ScriptableObject
    {
        #region Public 

        public string m_rootFolder = "Assets/_/GraphicsTiers";
        public string m_targetFolder = "Assets/_/GraphicsTiers/HD";
        public string m_fallbackFolder = "Assets/_/GraphicsTiers/Standard";
        public string[] m_existingFolders;

        #endregion
    }
}