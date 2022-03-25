using Sirenix.OdinInspector;
using UnityEngine;

using static UnityEngine.GUILayout;

namespace Universe
{
    public class LocalisationManager : UBehaviour
    {
        #region Public Members

        public LanguageData m_currentLanguage;

        [EnableIf( "IsDebug" ), Header("Debug"), Space(15)]
        public LanguageData m_debugLanguage;

        #endregion


        #region Main

        public void ChangeLanguage( LanguageData languageData )
        {
            if( IsDebug ) Debug.Log( $"ChangeLanguage to {languageData.m_name}" );
            EmitEventIfExists( languageData );
        }

        #endregion


        #region Unity API

        private void Start()
        {
            ChangeLanguage( m_currentLanguage );
        }

        private void OnGUI()
        {
            if( IsNotDebug() ) return;

            EnableGUI( DebugLanguageExist() );

            if( Button( "Switch to debug Language" ) ) ChangeLanguage( m_debugLanguage );

            EnableGUI( true );
        }

        #endregion


        #region Private

        private void EmitEventIfExists( LanguageData languageData )
        {
            if( IsDebug ) Debug.Log( $"EmitEventIfExists languageData = {languageData}" );
            if( languageData != null ) OnLanguageChanged?.Invoke( this, languageData );
        }

        private void EnableGUI( bool enable ) => GUI.enabled = enable;
        private bool DebugLanguageExist() => m_debugLanguage != null;
        private bool IsNotDebug() => !IsDebug;

        #endregion


        #region Event

        public static event LanguageDataEventHandler OnLanguageChanged;
        public delegate void LanguageDataEventHandler( object sender, LanguageData languageData );

        #endregion
    }
}