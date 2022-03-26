using UnityEditor.SceneManagement;

using static UnityEditor.AssetDatabase;
using static UnityEditor.EditorApplication;
using static UnityEditor.SceneManagement.EditorSceneManager;
using static UnityEngine.Debug;

namespace Universe.Editor
{
    static class SceneHelper
    {
        static string sceneToOpen;

        public static void StartScene(string sceneName)
        {
            if(isPlaying)
            {
                isPlaying = false;
            }

            sceneToOpen = sceneName;
            update += OnUpdate;
        }

        static void OnUpdate()
        {
            if (sceneToOpen == null ||
                isPlaying || 
                isPaused ||
                isCompiling || 
                isPlayingOrWillChangePlaymode)
            {
                return;
            }

            update -= OnUpdate;

            if(SaveCurrentModifiedScenesIfUserWantsTo())
            {
				
                var guids = FindAssets("t:scene " + sceneToOpen, null);
                if (guids.Length == 0)
                {
                    LogWarning("Couldn't find scene file");
                }
                else
                {
                    var scenePath = GUIDToAssetPath(guids[0]);
                    EditorSceneManager.OpenScene(scenePath);
                    isPlaying = true;
                }
            }
            sceneToOpen = null;
        }
    }
    
}
