using TMPro;
using UnityEngine;

using static TMPro.FontStyles;
using static Universe.TextServices;

namespace Universe
{
    [RequireComponent(typeof(TextMeshPro))]
    public class UTextMeshPro : UTextGeneric<TMP_Text>
    {
        #region Public

        [HideInInspector]
        public TMP_FontAsset m_font;

        #endregion


        #region Utilities

        protected override void SetTextAttributes()
        {
            if( IsFontSettingsNull() ) SetFontSettings();
            if( IsComponentNull()) GetTextComponent();

            if( IsComponentOrFontSettingsNull() ) return;

            SetTextComponent();
            UpdateText();
            UpdateFont();
        }

        protected override void SetTextComponent()
        {
            m_component.fontStyle = GetFontStyles( GetRelevantFontStyle() );
            m_component.fontSize = GetRelevantFontSize();
            m_component.color = GetRelevantFontColor();
        }

        protected FontStyles GetFontStyles( FontStyle fontStyle )
        {
            switch( fontStyle )
            {
                case FontStyle.Normal: return Normal;
                case FontStyle.Bold: return Bold;
                case FontStyle.Italic: return Italic;
                case FontStyle.BoldAndItalic: return Bold | Italic;

                default: return Normal;
            }
        }

        protected override void UpdateText() =>
            m_component.text = m_text;

        protected override void UpdateFont()
        {
            m_font = (TMP_FontAsset)GetFont( m_fontSettingsType );
            m_component.font = m_font;

            Verbose( $"UTextMeshPro.UpdateFont m_font = {m_font}" );
        }

        #endregion
    }
}