using TMPro;

namespace Universe
{
    public class VersionDisplay : UBehaviour
    {
        #region Exposed

        public CISettings m_version;
        public TMP_Text m_text;

        #endregion


        #region Unity API

        public override void Awake()
        {
            var version = m_version.m_version;
            var date = m_version.m_buildTime;

            m_text.text = $"{version} made {date}";
        }

        #endregion
    }
}