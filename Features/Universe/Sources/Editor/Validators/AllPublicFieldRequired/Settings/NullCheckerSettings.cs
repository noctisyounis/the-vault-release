using UnityEditor;
using UnityEngine;
using static System.IO.Directory;
using static System.IO.Path;
using static UnityEditor.AssetDatabase;
using static UnityEditor.FileUtil;
using static UnityEngine.Application;
using static UnityEngine.Debug;

namespace Universe.Editor.NullChecker
{
    public class NullCheckerSettings : ScriptableObject
    {
        #region Exposed

        [SerializeField]
        private float _linePixelSize = 18f;
        [SerializeField]
        private float _linePixelSpacing = 2f;
        [SerializeField]
        private Color _validColor = new Color(0f, 79f/255, 5f/255);
        [SerializeField]
        private Color _errorColor = new Color(79f/255, 0f, 0f);
        [SerializeField]
        private string _defaultWarning = "Value is Null. Need to FIX before play !";
        [SerializeField]
        private string _settingPathOverride;
        
        #endregion
        
        #region Public Properties

        public static NullCheckerSettings Instance {get; private set;}
        public float LinePixelSize => _linePixelSize;
        public float LinePixelSpacing => _linePixelSpacing;
        public Color ValidColor => _validColor;
        public Color ErrorColor => _errorColor;        
        public string DefaultWarning => _defaultWarning;

        #endregion
        
        
        #region Constants

        public const string SETTINGS_ASSETS_DIRECTORY = "Settings\\NullChecker";
        public const string SETTINGS_ASSETS_NAME = "NullCheckerSettings.asset";

        #endregion


        #region Properties

        public static SerializedObject SerializedSettings => new(GetOrCreateSettings());
             
        #endregion


        #region Unity API

        private void OnEnable()
        {
            _pathOverride = Instance != null ? Instance._settingPathOverride : _settingPathOverride;
        }
             
        #endregion
        

        #region Main

        internal static NullCheckerSettings GetOrCreateSettings()
        {

            if (Instance != null) return Instance;
            var settingsPath = _pathOverride;
            var path = $"{settingsPath}/{SETTINGS_ASSETS_NAME}".Replace(dataPath, "Assets");
            var settings = LoadAssetAtPath<NullCheckerSettings>(path);

            if (settings != null) return Instance = settings;
            
            settingsPath = $"Assets\\{SETTINGS_ASSETS_DIRECTORY}";
            settings = LoadAssetAtPath<NullCheckerSettings>($"{settingsPath}\\{SETTINGS_ASSETS_NAME}");

            if(settings != null) return Instance = settings;

            LogWarning($"No setting found in project, creating a new one at <color=cyan>'{settingsPath}'</color>");

            return Instance = CreateSettingsAt(settingsPath);
        }

        public void CheckPath()
        {
            if(_settingPathOverride.Length < 1) 
            {
                ResetSettingPath();
                return;
            }

            Instance._settingPathOverride = _settingPathOverride.Replace('/', '\\');
            if(_settingPathOverride[^1].Equals('\\'))
            {
                LogWarning($"Invalid synthax, the setting path can't end with '/' or '\\'");
                Instance._settingPathOverride = _settingPathOverride.Remove(_settingPathOverride.Length - 1, 1);
            }
            
            if(_pathOverride.Equals(_settingPathOverride)) return;
            if (!_settingPathOverride.Contains("Assets"))
            {
                LogWarning("Path need to begin with 'Assets/'");
                ResetSettingPath();
                return;
            }
            
            if(_settingPathOverride.Contains("."))
            {
                LogWarning("Path must be a folder, '.' or extension are not allowed.");
                ResetSettingPath();
                return;
            }

            MoveSettingsTo(_settingPathOverride);
        }

        private void ResetSettingPath()
        {
            Instance._settingPathOverride = _pathOverride;
        }

        #endregion


        #region Plumbery

        private void MoveSettingsTo(string newPath)
        {
            LogWarning($"initialize settings movement to <color=cyan>'{newPath}'</color>");
            LogWarning($"Delete previous settings at <color=cyan>'{_pathOverride}'</color>");
            DeleteAsset(_pathOverride);
            var parentDir = GetParentPathOf(_pathOverride);
            ClearFoldersOn(parentDir);
            Instance = CreateSettingsAt(newPath);
        }

        private void ClearFoldersOn(string path)
        {
            if (!Exists(path)) return;

            var isDirectoryNotEmpty = GetDirectories(path).Length > 0 || GetFiles(path).Length > 0;
            if (isDirectoryNotEmpty) return;
            
            var isPartOfDestinationPath = Instance._settingPathOverride.StartsWith(path);
            isPartOfDestinationPath = isPartOfDestinationPath && Instance._settingPathOverride[path.Length - 1].Equals('\\');
            if(isPartOfDestinationPath) 
            {
                LogWarning($"<color=cyan>'{path}'</color> is part of the destination path, end of cleaning"); 
                return;
            }

            LogWarning($"Deleting folder at <color=cyan>'{path}'</color>");
            DeleteFileOrDirectory(path);
            DeleteFileOrDirectory($"{path}.meta");
            
            var parentDir = GetParentPathOf(path);
            ClearFoldersOn(parentDir);
        }
        
        #endregion


        #region Utils

        private static NullCheckerSettings CreateSettingsAt(string relativePath)
        {
            var settings = Instance?.CopySettings();
            if(!settings) settings = CreateInstance<NullCheckerSettings>();
            settings._settingPathOverride = relativePath;
            _pathOverride = relativePath;

            if (!Exists(_pathOverride)) CreateDirectory(_pathOverride);
            
            CreateAsset(settings, $"{_pathOverride}\\{SETTINGS_ASSETS_NAME}");
            SaveAssets();
            LogWarning($"Created new settings at <color=cyan>'{_pathOverride}\\{SETTINGS_ASSETS_NAME}'</color>", Instance);

            return settings;
        }

        private string GetParentPathOf(string path) =>
             GetDirectoryName(path);

        private NullCheckerSettings CopySettings()
        {
            var settings = CreateInstance<NullCheckerSettings>();
            settings._defaultWarning = Instance._defaultWarning;
            settings._errorColor = Instance._errorColor;
            settings._linePixelSize = Instance._linePixelSize;
            settings._linePixelSpacing = Instance._linePixelSpacing;
            settings._settingPathOverride = Instance._settingPathOverride;
            settings._validColor = Instance._validColor;

            return settings;
        }

        #endregion
    

        #region Private Fields
            
        private static string _pathOverride;

        #endregion
    }
}