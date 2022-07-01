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

namespace Universe.Toolbar.Editor
{
    public class ToggleEnvironment
    {
        #region Main

        public static void Draw(string playerPref, Environment environment )
        {
            var currentEnvironment = Situation.CurrentEnvironment;
            var currentLevelPath    = GetString(playerPref);
            var willLoad = (currentEnvironment & environment) == 0;

            if( !IsValidPath( currentLevelPath ) ) return;

            var texName   = willLoad ? "d_CacheServerDisconnected" : "d_CacheServerConnected";
            var tex       = IconContent(texName).image;
            var labelText =  willLoad ? "Load" : "Unload";

            if( !Button( new GUIContent( environment.ToString(), tex, $"{labelText} {environment}" ) ) ) return;
            
            Situation.CurrentEnvironment ^= environment;

            var level           = LoadAssetAtPath<LevelData>(currentLevelPath);
            var situations = level.Situations;

            foreach (var situation in situations)
            {
                var gameplayGuid = situation.m_gameplay.m_assetReference.AssetGUID;
                var gameplayPath = GUIDToAssetPath(gameplayGuid);
                var gameplayScene = EditorSceneManager.GetSceneByPath(gameplayPath);
                if (!gameplayScene.IsValid()) return;
                
                var blockMeshGuid   = situation.m_blockMeshEnvironment.m_assetReference.AssetGUID;
                var artGuid         = situation.m_artEnvironment.m_assetReference.AssetGUID;
                var environmentGuid = IsArtEnvironment(environment) ? artGuid : blockMeshGuid;
                var environmentPath = GUIDToAssetPath(environmentGuid);
                
                if( willLoad )
                {
                    OpenScene( environmentPath, Additive );
                    return;
                }

                var environmentScene = EditorSceneManager.GetSceneByPath( environmentPath );
                SaveCurrentModifiedScenesIfUserWantsTo();
                CloseScene( environmentScene, false );
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

        private static bool IsArtEnvironment(Environment environment) => 
            ( ( environment & Environment.ART ) != 0 );

        #endregion
    }
}