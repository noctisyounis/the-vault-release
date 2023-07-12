using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

using static UnityEngine.AddressableAssets.Addressables;
using static UnityEngine.Debug;
using static UnityEngine.Application;

namespace Universe
{
    public class TextServices
    {
        #region Public Methods
        
        public static void Register( UText uText ) => AddToList( uText );
        public static void Unregister( UText uText ) => RemoveFromList( uText );

        public static void RegisterManager( TextManager manager )
        {
            if( DoesTextManagerExist() ) return;

            UpdateTextManager( manager );
        }

        public static void RefreshTexts()
        {
            if( NotInPlayMode() )
                FillUTextsList();


            if( IsUTextListEmpty() ) return;

            RefreshAllUTexts();
        }

        public static FontSettings GetFontSettings( FontSettingsType fontSettingsType )
        {
            //if( AssertFontCollectionIsNull( $".GetFontSettings({fontSettingsType})" ) ) return null;
            if(IsFontSettingsCollectionNull()) FontSettings = Settings.m_defaultFontSettings;

            return GetFontSettingsCorrespondingTo( fontSettingsType );
        }

        public static object GetFont( FontSettingsType fontSettingsType )
        {
            AssetReference fontRef;
            //if( AssertFontCollectionIsNull( $".GetFont({fontSettingsType})" ) ) return null;

            if( IsFontSettingsCollectionNull() ) FontSettings = Settings.m_defaultFontSettings;

            fontRef = GetFontSettingsCorrespondingTo( fontSettingsType ).m_font;

            if( IsNotLoaded( fontRef ) )
            {
                if(Settings.m_showErrors) LogError( $"font {GetAssetKey( fontRef )} not loaded" );
                return null;
            }

            return GetFontOf( fontRef );
        }

        #endregion


        #region Utilities

        private static void LoadFonts()
        {
            if( !FontsAreLoading() )
            {
                _fontToLoadCount = 0;
                _fontLoadedCount = 0;
            }

            ReleaseUnusedFonts();

            foreach( var fontSettings in _fontSettings.m_list )
            {
                if( IsNotLoaded( fontSettings.m_font ) )
                    LoadFont( fontSettings.m_font );
            }

            if( NoFontsToLoad() ) OnAllFontsLoaded();
        }

        private static void ReleaseUnusedFonts()
        {
            string[] keys = GetDictionaryKeysArray();

            var fontCount = keys.Length;
            for( var i = 0; i < fontCount; i++ )
            {
                var key = keys[i];
                if( string.IsNullOrEmpty( key ) ) continue;

                var font = _fontsDictionary[keys[i]];
                var correspondingSetting = _fontSettings.m_list.Find( x => GetAssetKey( x.m_font ) == key );
                if( correspondingSetting == null )
                {
                    if( isPlaying && (font != null) && _fontsHandle.ContainsKey( font ) )
                    {
                        Release( _fontsHandle[font] );
                        _fontsHandle.Remove( font );
                    }
                    _fontsDictionary.Remove( key );
                }
            }
        }

        private static string[] GetDictionaryKeysArray()
        {
            string[] keys = new string[_fontsDictionary.Count];
            _fontsDictionary.Keys.CopyTo( keys, 0 );
            return keys;
        }

        private static void OnAllFontsLoaded()
        {
#if UNITY_EDITOR
            if( NotInPlayMode() )
                RefreshAllUTexts();
#endif

            if( InPlayMode() )
                OnFontsLoaded?.Invoke( _fontsDictionary );
        }

        private static void LoadFont( AssetReference fontRef )
        {
            if( _assetsToLoad.Contains( fontRef ) ) return;

            _assetsToLoad.Add( fontRef );
            _fontToLoadCount++;

            if( !isPlaying )
            {
#if UNITY_EDITOR
                OnFontLoaded( fontRef, fontRef.editorAsset );
#endif
            }
            else
            {
                LoadAssetAsync<object>( fontRef ).Completed += obj =>
                        OnFontLoaded( fontRef, obj.Result, obj );
            }
        }

        private static void OnFontLoaded( AssetReference fontRef, object result, object obj = null )
        {
            _fontLoadedCount++;

            var key = GetAssetKey( fontRef );
            if( !_fontsDictionary.ContainsKey( key ) )
                _fontsDictionary.Add( key, result );

            if( obj != null )
            {
                var casted = (UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>)obj;
                if( _fontsHandle.ContainsValue( casted ) )
                    _fontsHandle.Add( result, casted );
            }

            _assetsToLoad.Remove( fontRef );

            if( _fontToLoadCount == _fontLoadedCount )
                OnAllFontsLoaded();
        }

        private static void FillUTextsList()
        {
            var uTexts = TextManager?.GetUTexts();

            if( uTexts == null ) return;
            _listUTexts = new List<UText>( uTexts );
        }

        private static void RefreshAllUTexts()
        {
            var uTextCount = _listUTexts.Count;
            for( var i = 0; i < uTextCount; i++ )
            {
                _listUTexts[i].Refresh();
            }
        }

        private static void AddToList( UText uText )
        {
            if( _listUTexts.Contains( uText ) ) return;

            _listUTexts.Add( uText );
        }
        
        private static void RemoveFromList( UText uText )
        {
            if( !_listUTexts.Contains( uText ) ) return;

            _listUTexts.Remove( uText );
        }

        private static bool AssertFontCollectionIsNull( string message )
        {
            if( !IsFontSettingsCollectionNull() ) return false;

            if( Settings.m_showErrors ) LogErrorFontSettingsNull( message );
            return true;
        }

        private static void LogErrorFontSettingsNull( string callingFunctionName )
        {
            Debug.LogError( $"[TextServices]{callingFunctionName} no fontSettings Collection found m_fontSettings = {FontSettings}" );
            Debug.LogError( $"    m_fontSettings.m_list = {FontSettings.m_list}" );
            Debug.LogError( $"    m_fontSettings.m_list.Count = {FontSettings.m_list.Count}" );
        }

        private static bool IsUTextListEmpty() => _listUTexts.Count <= 0;
        private static bool IsFontSettingsCollectionNull() => FontSettings == null || FontSettings.ListIsNullOrEmpty();
        private static bool IsNotLoaded( AssetReference fontRef ) => !_fontsDictionary.ContainsKey( GetAssetKey( fontRef ) );
        private static bool NoFontsToLoad() => _fontToLoadCount == 0 && _fontLoadedCount == 0;
        private static bool FontsAreLoading() => _assetsToLoad.Count > 0;
        private static bool DoesTextManagerExist() => _textManager != null;
        private static bool NotInPlayMode() => !isPlaying;
        private static bool InPlayMode() => isPlaying;

        private static object GetFontOf( AssetReference fontRef ) => _fontsDictionary[GetAssetKey( fontRef )];
        private static string GetAssetKey( AssetReference asset ) => $"{asset.RuntimeKey}";
        private static FontSettings GetFontSettingsCorrespondingTo( FontSettingsType fontSettingsType ) => FontSettings.m_list[Convert.ToInt32( fontSettingsType )];

        #endregion


        #region ObjectEventHandler

        public delegate void ObjectEventHandler( object font );

        public static void AddListenerToOnFontsLoaded( ObjectEventHandler handler )
        {
            if (!_onFontsLoadedListeners.Contains(handler))
            {
                OnFontsLoaded += handler;
                _onFontsLoadedListeners.Add(handler);
            }
        }

        public static void RemoveListenerFromOnFontsLoaded( ObjectEventHandler handler )
        {
            if (_onFontsLoadedListeners.Contains(handler))
            {
                OnFontsLoaded -= handler;
                _onFontsLoadedListeners.Remove(handler);
            }
        }

        private static event ObjectEventHandler OnFontsLoaded;
        private static List<ObjectEventHandler> _onFontsLoadedListeners = new List<ObjectEventHandler>();


        #endregion


        #region TextManager Utilities

        private static void GetManager()
        {
            var managers = UnityEngine.Object.FindObjectsOfType<TextManager>();

            if( managers == null || managers.Length == 0 ) return;

            UpdateTextManager( managers[0] as TextManager );
        }

        private static void UpdateTextManager( TextManager textManager )
        {
            _textManager = textManager;
            FontSettings = _textManager.m_fontSettings;
        }

        #endregion


        #region Properties

        public static TextManager TextManager
        {
            get
            {
#if UNITY_EDITOR
                if( !isPlaying )
                {
                    if( _textManager != null ) return _textManager;

                    GetManager();
                }
#endif
                return _textManager;
            }
            set => _textManager = value;
        }

        public static FontSettingsTable FontSettings {
            get => _fontSettings;

            set
            {
                if( value == _fontSettings ) return;

                _fontSettings = value;
                LoadFonts();
            }
        }

        public static TextServicesSettings Settings
        {
            get
            {
                if( _settings == null ) _settings = TextServicesSettings.GetOrCreateSettings();

                return _settings;
            }
        }

        #endregion


        #region Private

        private static int _fontToLoadCount;
        private static int _fontLoadedCount;

        private static List<UText> _listUTexts = new List<UText>();
        private static TextManager _textManager;
        private static FontSettingsTable _fontSettings;
        private static Dictionary<string, object> _fontsDictionary = new Dictionary<string, object>();
        private static Dictionary<object, UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>> _fontsHandle = new Dictionary<object, UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<object>>();
        private static List<AssetReference> _assetsToLoad = new List<AssetReference>();
        private static TextServicesSettings _settings;

#endregion
    }
}