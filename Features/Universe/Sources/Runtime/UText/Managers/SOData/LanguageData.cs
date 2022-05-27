using System.Collections.Generic;

namespace Universe
{
    public class LanguageData : UniverseScriptableObject
    {
        #region Public Members

        public string m_name;
        public FontSettingsTable m_fontSettingsCollection;
        public List<StringFact> m_localisedTexts;

        #endregion


        #region Extension

        public override string ToString() => $"[LanguageData] {m_name} with {m_localisedTexts.Count} localised lines.";
        public bool HasFontSettingsCollection() => m_fontSettingsCollection != null;

        #endregion
    }
}