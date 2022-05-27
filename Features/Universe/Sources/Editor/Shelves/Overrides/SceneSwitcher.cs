using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

using static UnityEditor.AssetDatabase;
using static UnityEditor.EditorApplication;
using static UnityEditor.EditorBuildSettings;
using static UnityEditor.PlayModeStateChange;
using static UnityEditor.SceneManagement.EditorSceneManager;
using static UnityEngine.PlayerPrefs;
using static UnityEngine.SceneManagement.SceneManager;

namespace Universe.Toolbar.Editor
{
	[InitializeOnLoad]
	public class SceneSwitchLeftButton
	{
		#region Constructor
		
		static SceneSwitchLeftButton()
		{
			playModeStateChanged += OverridePlayMode;
		}
		
		#endregion
		
		
		#region Main

		static void OverridePlayMode(PlayModeStateChange state)
		{
			if (state == ExitingEditMode)
			{
				CacheCurrentScenes();
				LoadGameStarter();

				isPlaying = true;
				return;
			}
			
			if (state == ExitingPlayMode)
			{
				LoadCachedScenes();
				isPlaying = false;
			}
		}
		
		#endregion
		

		#region Utils

		static void CacheCurrentScenes()
		{
			var c = sceneCount;

			for (var i = 1; i < c; i++) 
			{
				var scene = GetSceneAt (i);
				SetString($"[CustomPlayMode]{i.ToString()}", scene.path);
			}
		}
		
		static void LoadCachedScenes()
		{
			var j = 1;
			while (HasKey($"[CustomPlayMode]{j.ToString()}"))
			{
				LoadScene(GetString($"[CustomPlayMode]{j.ToString()}"), LoadSceneMode.Additive);
				DeleteKey($"[CustomPlayMode]{j.ToString()}");
				j++;
			}
		}
		
		static void LoadGameStarter()
		{
			if (scenes.Length <= 0)
			{
				Debug.LogError("[CustomPlayMode]Can't enter play mode because GameStarter is not in build settings at index 0");
				return;
			}
			
			playModeStartScene = LoadAssetAtPath<SceneAsset>(scenes[0].path);
		}
		
		#endregion
	}
}