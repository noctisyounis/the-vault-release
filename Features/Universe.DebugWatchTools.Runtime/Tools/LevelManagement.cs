using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Universe.SceneTask.Runtime;

using static Universe.SceneTask.Runtime.Environment;
using static Universe.SceneTask.Runtime.Level;
using static Universe.SceneTask.Runtime.Situation;
using Environment = Universe.SceneTask.Runtime.Environment;

#if UNITY_EDITOR
using static UnityEditor.AssetDatabase;
#endif

namespace Universe.DebugWatchTools.Runtime
{
    public class LevelManagement : UBehaviour
    {
        #region Exposed
        public const string BAKED_FILE_PATH = "{AssetPath}\\_\\Features\\UniverseGenerated";
        public const string BAKED_FILE_NAME = "LevelNavigation.cs";
        public const string BAKED_FILE_HEADER = "using Universe.DebugWatch.Runtime;\nusing Universe.DebugWatchTools.Runtime;\n\nnamespace Universe.DebugWatchTools.Generated\n{\n\tpublic static class LevelNavigation\n\t{";
        public const string BAKED_FILE_FOOTER = "\t}\n}";

        public static string s_levelFolderPath = "_/Content/Levels";

        public CheckpointData m_debugCheckpoint;

        #endregion


        #region Unity API

        public override void Awake() => 
            SubscribeToEvents();

        public override void OnDestroy() => 
            UnsubscribeFromEvents();

        #endregion


        #region Public API

        public static void SaveDebugCheckpointRequest() =>
            OnSaveStateRequested?.Invoke();

        public static void LoadDebugCheckpointRequest() =>
            OnLoadStateRequested?.Invoke();

        public static void ReloadGameplayTaskRequest() =>
            OnReloadTaskRequested?.Invoke();

        public static void ToggleEnvironmentRequest( Environment environment ) =>
            OnEnvironmentToggleRequested?.Invoke(environment);

        public static void ChangeLevelRequest( string levelPath, int taskIndex ) =>
            OnChangeLevelRequested?.Invoke( levelPath, taskIndex );

        #endregion


        #region Main

#if UNITY_EDITOR
        [MenuItem("Vault/Debug Watch/Bake Levels")]
#endif
        public static void BakeLevelDebug()
        {
#if UNITY_EDITOR
            var folderPath      = BAKED_FILE_PATH.Replace("{AssetPath}", Application.dataPath);
            if( !Directory.Exists( folderPath ) )
                Directory.CreateDirectory( folderPath );

            var fullPath        = $"{folderPath}\\{BAKED_FILE_NAME}";
            if( !File.Exists( fullPath ) )
                File.Create( fullPath ).Close();

            var sw              = new StreamWriter(fullPath);
            var pathTableGuids  = FindAssets($"t:{typeof(UAssetsPathTable)}");
            var pathTablePath   = GUIDToAssetPath(pathTableGuids[0]);
            var pathTable       = LoadAssetAtPath<UAssetsPathTable>(pathTablePath);
            var paths           = pathTable.m_paths;
            var levelPaths      = paths.FindAll((path) => path.Contains(s_levelFolderPath));

            sw.WriteLine( BAKED_FILE_HEADER );

            foreach( var path in levelPaths )
            {
                var type = GetMainAssetTypeAtPath(path);
                if (type == null)
                    continue;
                if( !type.Equals( typeof( LevelData ) ) )
                    continue;

                var situationIndex = 0;
                var level = LoadAssetAtPath<LevelData>( path );
                var levelName = level.name;
                levelName = levelName.Replace("-", "");
                levelName = levelName.Replace("\"", "");
                levelName = levelName.Replace("\'", "");
                levelName = levelName.Replace(",", "");
                levelName = levelName.Trim();
                
                foreach( var situation in level.Situations )
                {
                    sw.WriteLine( $"\t\t[DebugMenu(\"Tasks.../Levels.../{levelName}/{situation.m_name}\")] public static void ChangeLevelTo{levelName}{situationIndex:00}() => LevelManagement.ChangeLevelRequest(\"{path}\", {situationIndex++});" );
                }
            }
            
            sw.WriteLine( BAKED_FILE_FOOTER );
            sw.Close();
            
            Refresh();
#endif
        }

        private void SaveProgress() =>
            SaveCheckpoint(m_debugCheckpoint);

        private void LoadProgress() =>            
            LoadCheckpoint(m_debugCheckpoint);

        private void ReloadGameplayTask()
        {
            var level = Level.s_currentLevel;
            var task = Level.CurrentSituation;

            ChangeLevel( level, task );
        }
        

        private void ToggleEnvironment(Environment environment)
        {
            var next = CurrentEnvironment ^ environment;

            if( next != 0 )
            {
                CurrentEnvironment = next;
                ReloadLevel();
                return;
            }

            CurrentEnvironment = BOTH ^ environment;
            ReloadLevel();
        }

        private void ChangeLevel( string path, int situationIndex )
        {
            var handle = Addressables.LoadAssetAsync<LevelData>( path );

            handle.Completed += ( data ) =>
            {
                var level = data.Result;
                var situation = level.GetSituation(situationIndex);

                ChangeLevel( level, situation );
            };

        }

        #endregion


        #region Utils

        private void SubscribeToEvents()
        {
            OnSaveStateRequested += SaveProgress;
            OnLoadStateRequested += LoadProgress;
            OnReloadTaskRequested += ReloadGameplayTask;
            OnEnvironmentToggleRequested += ToggleEnvironment;
            OnChangeLevelRequested += ChangeLevel;
        }

        private void UnsubscribeFromEvents()
        {
            OnSaveStateRequested -= SaveProgress;
            OnLoadStateRequested -= LoadProgress;
            OnReloadTaskRequested -= ReloadGameplayTask;
            OnEnvironmentToggleRequested += ToggleEnvironment;
            OnChangeLevelRequested -= ChangeLevel;
        }

        #endregion


        #region Private and Protected

        private static event Action OnSaveStateRequested;
        private static event Action OnLoadStateRequested;
        private static event Action OnReloadTaskRequested;
        private static event Action<Environment> OnEnvironmentToggleRequested;
        private static event Action<string, int> OnChangeLevelRequested;

        #endregion
    }
}