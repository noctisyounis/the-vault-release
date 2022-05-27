using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AddressableAssets;
using static Universe.TextServices;

namespace Universe
{
    //[ExecuteAlways]
    public class TextManager : UBehaviour
    {
        #region Public

        public FontSettingsTable m_fontSettings;

        public void UpdateTextServices()
        {
            TextServices.FontSettings = m_fontSettings;
            RefreshTexts();
#if UNITY_EDITOR
            SceneView.RepaintAll();
#endif
        }

        #endregion


        #region Unity API

        public new void Awake()
        {
            base.Awake();

            RegisterManager( this );
            AddListenerToOnLanguageChanged();

            Addressables.InitializeAsync();
            _awaken = true;
        }

        private void OnValidate()
        {
            if( Application.isPlaying && !_awaken ) return;
            UpdateTextServices();
        }

        #endregion


        #region Utilities

        private void AddListenerToOnLanguageChanged() => LocalisationManager.OnLanguageChanged += OnLanguageChanged;

        private void OnLanguageChanged( object sender, LanguageData languageData )
        {
            if( IsDebug ) Debug.Log( $"OnLanguageChanged languageData = {languageData}, {languageData.m_name}" );
            if( languageData.HasFontSettingsCollection() )
            {
                m_fontSettings = languageData.m_fontSettingsCollection;
            }

            UpdateTextServices();
        }


        internal UText[] GetUTexts()
        {
            if(Application.isPlaying)
            {
                Debug.LogError( "ERROR this function should not be called outside edit mode. UTexts need to be registered on Awake." );
                return null;
            }

#if UNITY_EDITOR
            return FindObjectsOfType( typeof(UText) ) as UText[];
#else
            return null;
            //The call of this function must be made only in edit mode.
#endif
        }

        #endregion


        #region Private

        private bool _awaken;

        #endregion
    }
}