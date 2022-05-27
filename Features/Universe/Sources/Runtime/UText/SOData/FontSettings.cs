using UnityEngine;
using UnityEngine.AddressableAssets;

using static Universe.TextServices;

namespace Universe
{
    public class FontSettings : ScriptableObject
    {
        #region Public

        [Header("Font Parameters"), Space(15)]
        public AssetReference m_font;
        public int m_fontSize;
        public Color m_fontColor;
        public FontStyle m_fontStyle;

        #endregion


        #region Unity API

        private void OnValidate() => RefreshTexts();

        #endregion


        #region Extension

        public override string ToString() => $"[FontSettings] {name}";

        #endregion
    }
}