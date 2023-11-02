using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
            if( IsFontSettingsCollectionNull() ) FontSettings = Settings.m_defaultFontSettings;
            return GetFontSettingsCorrespondingTo( fontSettingsType );
        }

        public static object GetFont( FontSettingsType fontSettingsType )
        {
            //if( AssertFontCollectionIsNull( $".GetFont({fontSettingsType})" ) ) return null;
            if( IsFontSettingsCollectionNull() ) FontSettings = Settings.m_defaultFontSettings;
            
            var settings = GetFontSettingsCorrespondingTo( fontSettingsType );

            if (!settings)
            {
                Debug.LogError($"[TextService] No settings found for {fontSettingsType}");
                return default;
            }
            var fontRef = settings.m_font;

#if UNITY_EDITOR
            if (!isPlaying)
            {
                return fontRef.editorAsset;
            }
#endif
            if( IsNotLoaded( fontRef ) )
            {
                if( Settings && Settings.m_showErrors ) LogError( $"font {GetAssetKey( fontRef )} {fontRef} not loaded" );
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
                if( font == null )
                {
                    //_fontsDictionary.Remove( key );
                    continue;
                }
                var correspondingSetting = _fontSettings.m_list.Find( x => GetAssetKey( x.m_font ) == key );
                if( correspondingSetting == null )
                {
                    //#if !UNITY_EDITOR
                    if( isPlaying && _fontsHandle.ContainsKey( font ) )
                    {
                        Release( font );
                        _fontsHandle.Remove( font );
                    }
                    //#endif
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
            if( _refreshAfterLoad )
            {
                _refreshAfterLoad = false;
                RefreshAllUTexts();
            }
            
            if( InPlayMode() )
                OnFontsLoaded?.Invoke( _fontsDictionary );
        }

        private static void LoadFont( AssetReference fontRef )
        {
            if( _assetsToLoad.Contains( fontRef ) ) return;

            var key = GetAssetKey(fontRef);
            if ( _fontsDictionary.ContainsKey( key ) ) return;

            _assetsToLoad.Add( fontRef );
            _fontToLoadCount++;
            
            ////#if UNITY_EDITOR
            //            if (!isPlaying)
            //            {
            ////                OnFontLoaded( fontRef, fontRef.editorAsset );
            //            }
            ////#endif
            
            var casted = LoadAssetAsync<UnityEngine.Object>( fontRef );
            if( !_fontsHandleRef.ContainsKey( casted ) )
                _fontsHandleRef.Add( casted, fontRef );
            casted.Completed += OnFontLoaded_Completed;
        }

        private static void OnFontLoaded_Completed( AsyncOperationHandle<UnityEngine.Object> handle )
        {
            handle.Completed -= OnFontLoaded_Completed; ;
            if( handle.Status == AsyncOperationStatus.Succeeded )
            {
                var fontref = _fontsHandleRef[handle];
                OnFontLoaded( fontref, handle.Result );
                if( !_fontsHandle.ContainsKey( handle.Result ) )
                    _fontsHandle.Add( handle.Result, handle );
            }
        }

        private static void OnFontLoaded( AssetReference fontRef, object result/*, AsyncOperationHandle<object> obj*/)
        {
            _fontLoadedCount++;

            var key = GetAssetKey( fontRef );

            if( !_fontsDictionary.ContainsKey( key ) )
                _fontsDictionary.Add( key, result );

            _assetsToLoad.Remove( fontRef );
            if( _fontToLoadCount == _fontLoadedCount )
            {
                OnAllFontsLoaded();
                _assetsToLoad.Clear();
            }
        }

        private static void FillUTextsList()
        {
            var uTexts = TextManager?.GetUTexts();
            if( uTexts == null ) return;
            _listUTexts = new List<UText>( uTexts );
        }

        private static void RefreshAllUTexts()
        {
            if( FontsAreLoading() )
            {
                _refreshAfterLoad = true;
                return;
            }
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
        public static bool FontsAreLoading() => _assetsToLoad.Count > 0;
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
            if( !_onFontsLoadedListeners.Contains( handler ) )
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

        public static FontSettingsTable FontSettings
        {
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
        private static List<UText> _listUTexts = new ();
        private static TextManager _textManager;
        private static FontSettingsTable _fontSettings;
        private static Dictionary<string, object> _fontsDictionary = new ();
        private static Dictionary<object, AsyncOperationHandle<UnityEngine.Object>> _fontsHandle = new ();
        private static Dictionary<AsyncOperationHandle<UnityEngine.Object>, AssetReference> _fontsHandleRef = new ();
        private static List<AssetReference> _assetsToLoad = new ();
        private static TextServicesSettings _settings;
        private static bool _refreshAfterLoad;

        #endregion
    }
}