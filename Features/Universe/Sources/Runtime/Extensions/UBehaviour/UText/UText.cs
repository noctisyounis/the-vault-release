using Sirenix.OdinInspector;
using UnityEngine;

using static Universe.LocalisationManager;
using static Universe.TextServices;

namespace Universe
{
    public abstract class UText : UBehaviour
    {
        #region Public Members

        [Header("Text To Display"), Space(15)]
        public FontSettingsType m_fontSettingsType;
        [LabelText( "default text" )]
        public string m_text;
        public StringFact m_localisedData;

        [Header( "Override parameters" ), Space( 15 )]
        public bool m_overrideFontSize;
        [ShowIf( "m_overrideFontSize" )]
        public int m_fontSize;

        public bool m_overrideFontColor;
        [ShowIf( "m_overrideFontColor" )]
        public Color m_fontColor;

        public bool m_overrideFontStyle;
        [ShowIf( "m_overrideFontStyle" )]
        public FontStyle m_fontStyle;

        #endregion


        #region Main

        public void Refresh()
        {
            SetFontSettings();
            SetTextAttributes();
            
            if (!CurrentLanguage) UpdateText();
            else OnLanguageChangedCallback(this, CurrentLanguage);
        }

        #endregion


        #region Unity API

        public new void Awake()
        {
            base.Awake();

            Register( this );
            GetTextComponent();
            OnLanguageChanged += OnLanguageChangedCallback;
            AddListenerToOnFontsLoaded( OnFontsLoaded );

            if (!CurrentLanguage) return;

            OnLanguageChangedCallback(this, CurrentLanguage);
        }

        private void OnFontsLoaded( object font )
        {
            UpdateFont();
            Refresh();
        }

        private void Start() => 
            Refresh();

        private void OnValidate() => 
            Refresh();

        #endregion


        #region Callbacks

        virtual protected void OnLanguageChangedCallback( object sender, LanguageData languageData )
        {
            if( HasLocalisationInfo( languageData ) ) return;

            var translatedText = GetLocalisedText( languageData );

            if( IsLocalisedTextNull( translatedText ) )
                m_text = m_localisedData.name;
            else
                m_text = translatedText;

            UpdateText();
        }

        #endregion


        #region Utilities

        protected void SetFontSettings()
        {
            _fontSettings = GetFontSettings( m_fontSettingsType );
        }

        private static bool IsLocalisedTextNull( StringFact translatedText ) => translatedText == null;
        private static bool IsLanguageDataNull( LanguageData languageData ) => languageData == null;

        protected bool IsFontSettingsNull() => _fontSettings == null;
        protected bool IsLocalisedDataNull() => m_localisedData == null;
        protected bool HasLocalisationInfo( LanguageData languageData ) => IsLocalisedDataNull() || IsLanguageDataNull( languageData );

        protected StringFact GetLocalisedText( LanguageData languageData ) => languageData.m_localisedTexts.Find( x => x.name == m_localisedData.name );
        protected FontStyle GetRelevantFontStyle() => m_overrideFontStyle ? m_fontStyle : _fontSettings.m_fontStyle;
        protected Color GetRelevantFontColor() => m_overrideFontColor ? m_fontColor : _fontSettings.m_fontColor;
        protected int GetRelevantFontSize() => m_overrideFontSize ? m_fontSize : _fontSettings.m_fontSize;

        #endregion


        #region Abstract Methods

        protected abstract void GetTextComponent();
        protected abstract void SetTextComponent();
        protected abstract void SetTextAttributes();
        protected abstract void UpdateText();
        protected abstract void UpdateFont();

        #endregion


        #region Private Members

        protected FontSettings _fontSettings;

        #endregion
    }
}