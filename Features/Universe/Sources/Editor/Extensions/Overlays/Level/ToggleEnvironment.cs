using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;
using Universe.SceneTask.Runtime;

using static System.IO.Path;
using static System.IO.File;
using static UnityEditor.AssetDatabase;
using static UnityEditor.EditorGUIUtility;
using static UnityEditor.SceneManagement.OpenSceneMode;
using static UnityEditor.SceneManagement.EditorSceneManager;
using static UnityEngine.SceneManagement.SceneManager;
using static UnityEngine.PlayerPrefs;
using static UnityEngine.TextAnchor;
using static Universe.Editor.USettingsHelper;
using static Universe.SceneTask.Runtime.Environment;

namespace Universe.Overlays
{
    [EditorToolbarElement(ID, typeof(EditorWindow))]
	public class ToggleEnvironment : EditorToolbarToggle
	{
        #region Exposed

        public const string ID = "Level/ToggleEnvironment";

        public string m_onIcon = "d_winbtn_mac_max_h";
        public string m_offIcon = "d_winbtn_mac_close_h";
        
        #endregion
        
        
        #region Constructors

        public ToggleEnvironment(string playerPref, Environment environment, string tooltip = "")
        {
            _managedEnvironment = environment;
            _playerPref = playerPref;
            
            Initialize(tooltip);
            
            this.RegisterValueChangedCallback(OnToggle);
        }

        #endregion

        
        #region Main

        private void Initialize(string tooltip)
        {
            InitializeLayout();
            InitializeValues(tooltip);
            InitializeIcons();
        }

        private void InitializeLayout() =>
            style.unityTextAlign = MiddleLeft;

        private void InitializeValues(string tooltip)
        {
            text = _managedEnvironment.ToString();
            this.tooltip = tooltip;
            
            _currentLevelPath   = GetString(_playerPref);
            _settings           = GetSettings<LevelSettings>();
            _currentEnvironment = _settings.m_startingEnvironment;
            value = (_currentEnvironment & _managedEnvironment) > 0;
        }

        private void InitializeIcons()
        {
            var onIconTexture       = IconContent(m_onIcon).image as Texture2D;
            var offIconTexture       = IconContent(m_offIcon).image as Texture2D;

            onIcon = onIconTexture;
            offIcon = offIconTexture;
        }

        private void OnToggle(ChangeEvent<bool> context)
        {
            var nextEnvironment = _currentEnvironment ^ _managedEnvironment;

            if( nextEnvironment != 0 )
                _currentEnvironment = nextEnvironment;
            else
                _currentEnvironment = BOTH ^ _managedEnvironment;

            Situation.CurrentEnvironment = _currentEnvironment;
            _settings.m_startingEnvironment = _currentEnvironment;
            _settings.SaveAsset();
            
            var nextValue = (_currentEnvironment & _managedEnvironment) > 0;
            
            UpdateScenes();
            UpdateTooltip();
            SetValueWithoutNotify(nextValue);
        }

        public void Refresh(ChangeEvent<bool> context)
        {
            _currentEnvironment = _settings.m_startingEnvironment;
            
            var current = (_currentEnvironment & _managedEnvironment) > 0;
            
            SetValueWithoutNotify(current);
            UpdateTooltip();
        }

        private void UpdateScenes()
        {
            var level = LoadAssetAtPath<LevelData>(_currentLevelPath);
            var situations = level.Situations;
            
            foreach (var situation in situations)
            {
                var gameplayGuid = situation.m_gameplay.m_assetReference.AssetGUID;
                var gameplayPath = GUIDToAssetPath(gameplayGuid);
                var gameplayScene = GetSceneByPath(gameplayPath);
                if (!gameplayScene.IsValid()) return;
                
                var blockMeshGuid   = situation.m_blockMeshEnvironment.m_assetReference.AssetGUID;
                var blockMeshPath   = GUIDToAssetPath(blockMeshGuid);
                var artGuid         = situation.m_artEnvironment.m_assetReference.AssetGUID;
                var artPath         = GUIDToAssetPath(artGuid);
                
                if (IsArtEnvironment(_currentEnvironment))
                    OpenScene(artPath, Additive);
                else
                {
                    var scene = GetSceneByPath( artPath );
                    SaveCurrentModifiedScenesIfUserWantsTo();
                    CloseScene( scene, false );
                }
                
                if (IsBlockMeshEnvironment(_currentEnvironment))
                    OpenScene(blockMeshPath, Additive);
                else
                {
                    var scene = GetSceneByPath( blockMeshPath );
                    SaveCurrentModifiedScenesIfUserWantsTo();
                    CloseScene( scene, false );
                }
            }
        }
        
        #endregion
        

        #region Utils

        private void UpdateTooltip()
        {
            var action =  value ? "Load" : "Unload";
            
            tooltip = $"{action} {_managedEnvironment}";
        }

        private bool IsValidPath( string path )
        {
            if( string.IsNullOrEmpty( path ) ) return false;

            var fullPath = GetFullPath(path);
            return Exists( fullPath );
        }

        private bool IsBlockMeshEnvironment(Environment environment) => 
            ( ( environment & BLOCK_MESH ) != 0 );
        
        private bool IsArtEnvironment(Environment environment) => 
            ( ( environment & ART ) != 0 );

        #endregion

        
        #region Private

        private LevelSettings _settings;

        private string _playerPref;
        private string _currentLevelPath;
        private Environment _managedEnvironment;
        private Environment _currentEnvironment;

        #endregion
    }
}