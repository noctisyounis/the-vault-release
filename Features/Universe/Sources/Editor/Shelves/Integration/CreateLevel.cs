using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Universe.SceneTask;
using Universe.SceneTask.Runtime;

using static UnityEditor.EditorGUIUtility;
using static UnityEditor.AssetDatabase;
using static UnityEngine.GUILayout;
using static Universe.Toolbar.Editor.ButtonStyles;


namespace Universe.Toolbar.Editor
{
	public static class CreateLevelButton
	{
		public static void Draw()
		{
			FlexibleSpace();

			var tex = IconContent(@"d_CreateAddNew").image;
			if (Button(new GUIContent("New Level", tex, "Add a new level to the project"), commandButtonStyle))
			{
				CreateLevelWindow.ShowLevelWindow();
			}
		}
	}
	
    public class CreateLevelWindow : EditorWindow
	{
		#region Public

		public static CreateLevelSettings m_settings;
		public static string m_settingsFolder = "Assets/Settings/Universe";
		public static string m_settingsPath = Path.Join(m_settingsFolder, "CreateLevelSettings.asset");

		public string m_editorWindowText = "Type your level's name: ";
		public string m_newLevelName 	= "NewLevel";
		public TaskData m_blockMeshTask;
		public TaskData m_artTask;

		#endregion


		#region Main
	
		void OnGUI()
        {
            var preferedMaxSize = maxSize;
            var preferedMinSize = minSize;

            preferedMaxSize.y = 85.0f;
            preferedMinSize.y = 85.0f;

            m_newLevelName = EditorGUILayout.TextField(m_editorWindowText, m_newLevelName);

            DrawBlockMeshFields(ref preferedMaxSize, ref preferedMinSize);
            DrawArtFields(ref preferedMaxSize, ref preferedMinSize);

            GUILayout.BeginHorizontal();
            GUI.enabled = CanCreate();
            if (GUILayout.Button("Create"))
            {
                CreateLevelHelper.NewLevel(m_newLevelName, m_blockMeshTask, m_artTask);
				ReloadTasks();
                GUIUtility.ExitGUI();
            }
            GUI.enabled = true;

            if (GUILayout.Button("Cancel"))
            {
                Close();
                GUIUtility.ExitGUI();
            }
            GUILayout.EndHorizontal();

            maxSize = preferedMaxSize;
            minSize = preferedMinSize;
        }

        private void DrawBlockMeshFields(ref Vector2 preferedMaxSize, ref Vector2 preferedMinSize)
        {
            _useExistingBlockMesh = EditorGUILayout.Toggle("Use existing block mesh : ", _useExistingBlockMesh);

            if (_useExistingBlockMesh)
            {
                m_blockMeshTask = (TaskData)EditorGUILayout.ObjectField("Block mesh : ", m_blockMeshTask, typeof(TaskData), false);
				
                preferedMaxSize.y += 20.0f;
                preferedMinSize.y += 20.0f;
            }
			else
			{
				m_blockMeshTask = null;
			}
        }
		
		private void DrawArtFields(ref Vector2 preferedMaxSize, ref Vector2 preferedMinSize)
        {
            _useExistingArt = EditorGUILayout.Toggle("Use existing art : ", _useExistingArt);

            if (_useExistingArt)
            {
                m_artTask = (TaskData)EditorGUILayout.ObjectField("Art : ", m_artTask, typeof(TaskData), false);
				
                preferedMaxSize.y += 20.0f;
                preferedMinSize.y += 20.0f;
            }
			else
			{
				m_artTask = null;
			}
        }

        public static void ShowLevelWindow()
        {
            var window = ScriptableObject.CreateInstance<CreateLevelWindow>();
            var title = new GUIContent("Create new level", IconContent(@"SceneSet Icon").image);

            ReloadTasks();
			LoadSettings();

            window.titleContent = title;
            window.ShowUtility();
        }

        #endregion


        #region Utils

        public bool CanCreate()
		{
			if(!IsUsingAnyExistingEnvironment) 	return true;
			if(IsUsingInvalidBlockMesh) 		return false;
			if(IsUsingInvalidArt) 				return false;

			return true;
		}

		private static void ReloadTasks()
        {
            _environmentTasks 	= FindEnvironmentTasks();
            _taskNames 			= GetEnvironmentTaskNames();
        }

		public static List<TaskData> FindEnvironmentTasks()
		{
			var tasks = Resources.FindObjectsOfTypeAll<TaskData>().ToList();
			var environmentTasks = tasks.FindAll((TaskData task) => task.m_priority == TaskPriority.ENVIRONMENT);

			return environmentTasks;
		}

		public static List<string> GetEnvironmentTaskNames()
		{
			var names = new List<string>();

			foreach(var task in _environmentTasks)
			{
				names.Add(task.name);
			}

			return names;
		}

		private static void LoadSettings()
		{
			var fullPath = Path.GetFullPath(m_settingsPath);

			if(!IsValidFolder(m_settingsFolder)) FolderHelper.CreatePath(m_settingsFolder);
			if(!File.Exists(fullPath))
			{
				m_settings = ScriptableObject.CreateInstance<CreateLevelSettings>();
				
				CreateAsset(m_settings, m_settingsPath);
				SaveAssets();
			}
			else
			{
				m_settings = LoadAssetAtPath<CreateLevelSettings>(m_settingsPath);
			}
		}

		public bool IsUsingAnyExistingEnvironment 	=> _useExistingArt || _useExistingBlockMesh;
		public bool IsUsingInvalidBlockMesh 		=> _useExistingBlockMesh && !m_blockMeshTask;
		public bool IsUsingInvalidArt 				=> _useExistingArt && !m_artTask;

		#endregion

		
		#region Private

		private bool _useExistingBlockMesh;
		private bool _useExistingArt;
		private int _currentBlockMeshTask;
		private int _currentArtTask;
		private static List<TaskData> _environmentTasks;
		private static List<string> _taskNames;

		#endregion
	}
}