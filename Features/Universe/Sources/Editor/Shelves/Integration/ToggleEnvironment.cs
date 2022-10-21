using UnityEditor.SceneManagement;
using UnityEngine;
using Universe.SceneTask.Runtime;

using static System.IO.File;
using static System.IO.Path;
using static UnityEditor.AssetDatabase;
using static UnityEditor.EditorGUIUtility;
using static UnityEditor.SceneManagement.EditorSceneManager;
using static UnityEditor.SceneManagement.OpenSceneMode;
using static UnityEngine.GUILayout;
using static UnityEngine.PlayerPrefs;
using static Universe.Editor.USettingsHelper;

namespace Universe.Toolbar.Editor
{
    public class ToggleEnvironment
    {
        #region Main

        public static void Draw(string playerPref, Environment environment )
        {
            var levelSettings = GetSettings<LevelSettings>();
            var currentEnvironment = levelSettings.m_startingEnvironment;
            var currentLevelPath    = GetString(playerPref);
            var willLoad = (currentEnvironment & environment) == 0;

            if( !IsValidPath( currentLevelPath ) ) return;

            var texName   = willLoad ? "d_CacheServerDisconnected" : "d_CacheServerConnected";
            var tex       = IconContent(texName).image;
            var labelText =  willLoad ? "Load" : "Unload";

            if( !Button( new GUIContent( environment.ToString(), tex, $"{labelText} {environment}" ) ) ) return;
            
            var level           = LoadAssetAtPath<LevelData>(currentLevelPath);
            var situations = level.Situations;
            var next = currentEnvironment ^ environment;

            if( next != 0 )
                currentEnvironment = next;
            else
                currentEnvironment = Environment.BOTH ^ environment;

            Situation.CurrentEnvironment = currentEnvironment;
            levelSettings.m_startingEnvironment = currentEnvironment;
            levelSettings.SaveAsset();

            foreach (var situation in situations)
            {
                var gameplayGuid = situation.m_gameplay.m_assetReference.AssetGUID;
                var gameplayPath = GUIDToAssetPath(gameplayGuid);
                var gameplayScene = EditorSceneManager.GetSceneByPath(gameplayPath);
                if (!gameplayScene.IsValid()) return;
                
                var blockMeshGuid   = situation.m_blockMeshEnvironment.m_assetReference.AssetGUID;
                var blockMeshPath   = GUIDToAssetPath(blockMeshGuid);
                var artGuid         = situation.m_artEnvironment.m_assetReference.AssetGUID;
                var artPath         = GUIDToAssetPath(artGuid);
                
                if (IsArtEnvironment(currentEnvironment))
                    OpenScene(artPath, Additive);
                else
                {
                    var scene = EditorSceneManager.GetSceneByPath( artPath );
                    SaveCurrentModifiedScenesIfUserWantsTo();
                    CloseScene( scene, false );
                }
                
                if (IsBlockMeshEnvironment(currentEnvironment))
                    OpenScene(blockMeshPath, Additive);
                else
                {
                    var scene = EditorSceneManager.GetSceneByPath( blockMeshPath );
                    SaveCurrentModifiedScenesIfUserWantsTo();
                    CloseScene( scene, false );
                }
            }
        }

        #endregion


        #region Utils

        private static bool IsValidPath( string path )
        {
            if( string.IsNullOrEmpty( path ) ) return false;

            var fullPath = GetFullPath(path);
            return Exists( fullPath );
        }

        private static bool IsBlockMeshEnvironment(Environment environment) => 
            ( ( environment & Environment.BLOCK_MESH ) != 0 );
        
        private static bool IsArtEnvironment(Environment environment) => 
            ( ( environment & Environment.ART ) != 0 );

        #endregion
    }
}