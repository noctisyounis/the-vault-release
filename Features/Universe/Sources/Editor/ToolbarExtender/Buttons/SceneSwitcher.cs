using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

using static UnityEditor.AssetDatabase;
using static UnityEditor.EditorApplication;
using static UnityEditor.EditorGUIUtility;
using static UnityEditor.SceneManagement.EditorSceneManager;
using static UnityEngine.Debug;
using static UnityEngine.GUILayout;
using static UnityEngine.PlayerPrefs;
using static UnityEngine.SceneManagement.SceneManager;
using static Universe.ToolbarExtention.ToolbarExtender;
using static Universe.ToolbarExtention.ToolbarStyles;

namespace Universe.ToolbarExtention
{
	static class ToolbarStyles
	{
		public static readonly GUIStyle commandButtonStyle;

		static ToolbarStyles()
		{
			commandButtonStyle = new GUIStyle("Command")
			{
				fontSize = 16,
				alignment = TextAnchor.MiddleCenter,
				imagePosition = ImagePosition.ImageAbove,
				fontStyle = FontStyle.Bold
			};
		}
	}

	[InitializeOnLoad]
	public class SceneSwitchLeftButton
	{
		static SceneSwitchLeftButton()
		{
			RightToolbarGUI.Add(OnToolbarGUI);
		}

		static void OnToolbarGUI()
		{
			FlexibleSpace();

			var tex = IconContent(@"UnityEditor.GameView@2x").image;
			if (Button(new GUIContent(null, tex, "Focus SceneView when entering play mode"), commandButtonStyle))
			{
				if (!isPlaying)
				{
					var c = sceneCount;

					for (var i = 1; i < c; i++) 
					{
						var scene = GetSceneAt (i);
						SetString($"[CustomPlayMode]{i.ToString()}", scene.path);
					}

					playModeStartScene = LoadAssetAtPath<SceneAsset>("Assets/Features/Universe/Sources/Runtime/GameStarter/GameStarter.unity");
					isPlaying = true;
				}

				if (isPlaying)
				{
					var i = 1;
					while (HasKey($"[CustomPlayMode]{i.ToString()}"))
					{
						LoadScene(GetString($"[CustomPlayMode]{i.ToString()}"), LoadSceneMode.Additive);
						DeleteKey($"[CustomPlayMode]{i.ToString()}");
						i++;
					}

					isPlaying = false;
				}
			}
		}
	}

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