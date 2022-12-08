using UnityEngine;
using UnityEngine.UI;

using static Universe.TextServices;

namespace Universe
{
    [RequireComponent( typeof( Text ) )]
    public class UTextUnity : UTextGeneric<Text>
    {
        #region Public

        [HideInInspector]
        public Font m_font;

        #endregion


        #region Utilities

        protected override void SetTextAttributes()
        {
            if( IsFontSettingsNull() ) SetFontSettings();

            if( IsComponentOrFontSettingsNull() ) return;

            SetTextComponent();
            UpdateText();
        }

        protected override void SetTextComponent()
        {
            if( m_component == null ) GetTextComponent();

            if( m_component == null ) return;

            m_component.fontStyle = GetRelevantFontStyle();
            m_component.fontSize = GetRelevantFontSize();
            m_component.color = GetRelevantFontColor();
        }

        protected override void UpdateFont()
        {
            m_font = (Font)GetFont( m_fontSettingsType );
            m_component.font = m_font;

            Verbose( $"UTextUnity.UpdateFont m_font = {m_font}" );
        }

        protected override void UpdateText() =>
            m_component.text = m_text;

        #endregion
    }
}