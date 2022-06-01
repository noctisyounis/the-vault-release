using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Universe.Editor;
using Universe.SceneTask;
using Universe.SceneTask.Runtime;

using static UnityEditor.EditorGUIUtility;


namespace Universe.Toolbar.Editor
{
    public class CreateLevelWindow : EditorWindow
	{
		#region Public

		public static CreateLevelSettings m_settings;

		public string m_editorWindowText = "Type your level's name: ";
		public string m_newLevelName 	= "NewLevel";
		public TaskData m_audioTask;
		public TaskData m_blockMeshTask;
		public TaskData m_artTask;

		#endregion


		#region Main
	
		void OnGUI()
        {
            var preferedMaxSize = maxSize;
            var preferedMinSize = minSize;

            preferedMaxSize.y = 105.0f;
            preferedMinSize.y = 105.0f;

            m_newLevelName = EditorGUILayout.TextField(m_editorWindowText, m_newLevelName);

            DrawAudioFields(ref preferedMaxSize, ref preferedMinSize);
			DrawBlockMeshFields( ref preferedMaxSize, ref preferedMinSize);
            DrawArtFields(ref preferedMaxSize, ref preferedMinSize);

            GUILayout.BeginHorizontal();
            GUI.enabled = CanCreate();
            if (GUILayout.Button("Create"))
            {
                CreateLevelHelper.NewLevel(m_newLevelName, m_audioTask, m_blockMeshTask, m_artTask);
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

		private void DrawAudioFields( ref Vector2 preferedMaxSize, ref Vector2 preferedMinSize )
		{
			_useExistingAudio = EditorGUILayout.Toggle( "Use existing audio : ", _useExistingAudio );

			if( _useExistingAudio )
			{
				m_blockMeshTask = (TaskData)EditorGUILayout.ObjectField( "Audio : ", m_blockMeshTask, typeof( TaskData ), false );

				preferedMaxSize.y += 20.0f;
				preferedMinSize.y += 20.0f;
				return;
			}

			m_audioTask = null;
		}

		private void DrawBlockMeshFields( ref Vector2 preferedMaxSize, ref Vector2 preferedMinSize )
		{
			_useExistingBlockMesh = EditorGUILayout.Toggle( "Use existing block mesh : ", _useExistingBlockMesh );

			if( _useExistingBlockMesh )
			{
				m_blockMeshTask = (TaskData)EditorGUILayout.ObjectField( "Block mesh : ", m_blockMeshTask, typeof( TaskData ), false );

				preferedMaxSize.y += 20.0f;
				preferedMinSize.y += 20.0f;
				return;
			}

			m_blockMeshTask = null;
		}

		private void DrawArtFields(ref Vector2 preferedMaxSize, ref Vector2 preferedMinSize)
        {
            _useExistingArt = EditorGUILayout.Toggle("Use existing art : ", _useExistingArt);

            if (_useExistingArt)
            {
                m_artTask = (TaskData)EditorGUILayout.ObjectField("Art : ", m_artTask, typeof(TaskData), false);
				
                preferedMaxSize.y += 20.0f;
                preferedMinSize.y += 20.0f;
				return;
			}
			
			m_artTask = null;
        }

        public static void ShowLevelWindow()
        {
            var window	= CreateInstance<CreateLevelWindow>();
            var title	= new GUIContent("Create new level", IconContent(@"SceneSet Icon").image);

            ReloadTasks();
			LoadSettings();

            window.titleContent = title;
            window.ShowUtility();
        }

        #endregion


        #region Utils

        public bool CanCreate()
		{
			if(!IsUsingAnyExistingTask) 		return true;
			if(IsUsingInvalidAudio) 			return false;
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
			m_settings = USettingsHelper.GetSettings<CreateLevelSettings>();
		}

		public bool IsUsingAnyExistingTask 			=> _useExistingAudio || _useExistingArt || _useExistingBlockMesh;
		public bool IsUsingInvalidAudio 			=> _useExistingAudio && !m_audioTask;
		public bool IsUsingInvalidBlockMesh			=> _useExistingBlockMesh && !m_blockMeshTask;
		public bool IsUsingInvalidArt 				=> _useExistingArt && !m_artTask;

		#endregion

		
		#region Private

		private bool _useExistingAudio;
		private bool _useExistingBlockMesh;
		private bool _useExistingArt;

		private static List<TaskData> _environmentTasks;
		private static List<string> _taskNames;

		#endregion
	}
}