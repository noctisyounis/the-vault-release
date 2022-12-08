using UnityEditor;
using UnityEngine;
using Universe.DebugWatchTools.Runtime;
using Universe.Editor;
using Universe.SceneTask.Runtime;

using static UnityEditor.EditorGUIUtility;
using static UnityEditor.EditorGUILayout;

namespace Universe.Toolbar.Editor
{
    public class CreateLevelWindow : EditorWindow
	{
		#region Public

		public static float m_fieldSize = 20.0f;

		public static CreateLevelSettings m_settings;

		public string m_newLevelName 	= "NewLevel";
		public TaskData m_playerTask;
		public TaskData m_audioTask;
		public SituationInfos m_initialSituation;

		#endregion


		#region Main
	
		public void OnGUI()
        {
            var preferedMinSize = minSize;
            var preferedMaxSize = maxSize;

            preferedMinSize.y = 115.0f;
            preferedMaxSize.y = 115.0f;

            m_newLevelName = TextField("Name : ", m_newLevelName);

            DrawTaskField("Player", ref m_playerTask, ref _useExistingPlayer, ref preferedMinSize, ref preferedMaxSize);
            DrawTaskField("Audio", ref m_audioTask, ref _useExistingAudio, ref preferedMinSize, ref preferedMaxSize);
			CreateSituationWindow.DrawForm("Initial Situation", ref m_initialSituation, ref _useExistingBlockMesh, ref _useExistingArt, ref preferedMinSize, ref preferedMaxSize);

            GUILayout.BeginHorizontal();
            GUI.enabled = CanCreate();
            if (GUILayout.Button("Create"))
            {
	            m_initialSituation.m_isCheckpoint = true;
                CreateLevelHelper.CreateLevel(m_newLevelName, m_audioTask, m_playerTask, m_initialSituation);
                LevelManagement.BakeLevelDebug();

                GUIUtility.ExitGUI();
            }
            GUI.enabled = true;

            if (GUILayout.Button("Cancel"))
            {
                Close();
                GUIUtility.ExitGUI();
            }
            GUILayout.EndHorizontal();

            minSize = preferedMinSize;
            maxSize = preferedMaxSize;
        }

		private static void DrawTaskField(string label, ref TaskData task, ref bool useExisting, ref Vector2 preferedMinSize, ref Vector2 preferedMaxSize )
		{
			useExisting = Toggle( $"Use existing {label} : ", useExisting );
			preferedMinSize.y += m_fieldSize;
			preferedMaxSize.y += m_fieldSize;

			if( useExisting )
			{
				task = (TaskData)ObjectField( $"\t{label} : ", task, typeof( TaskData ), false );
				preferedMinSize.y += m_fieldSize;
				preferedMaxSize.y += m_fieldSize;
				return;
			}

			task = null;
		}

        public static void ShowLevelWindow()
        {
            var window	= CreateInstance<CreateLevelWindow>();
            var title	= new GUIContent("Create new level", IconContent(@"SceneSet Icon").image);

			LoadSettings();

            window.titleContent = title;
            window.ShowUtility();
        }

        #endregion


        #region Utils

        public bool CanCreate()
		{
			if(!IsUsingAnyExistingTask) return true;
			if(IsUsingInvalidAudio) 	return false;
			if(IsUsingInvalidPlayer)	return false;
			if(IsUsingInvalidBlockMesh)	return false;
			if(IsUsingInvalidArt)		return false;

			return true;
		}

		private static void LoadSettings() =>
			m_settings = USettingsHelper.GetSettings<CreateLevelSettings>();

		public bool IsUsingAnyExistingTask 			=> _useExistingPlayer || _useExistingAudio || _useExistingBlockMesh || _useExistingArt;
		public bool IsUsingInvalidAudio 			=> _useExistingAudio && !m_audioTask;
		public bool IsUsingInvalidPlayer			=> _useExistingPlayer && !m_playerTask;
		public bool IsUsingInvalidBlockMesh			=> _useExistingBlockMesh && !m_initialSituation.m_blockMesh;
		public bool IsUsingInvalidArt				=> _useExistingArt && !m_initialSituation.m_art;

		#endregion

		
		#region Private

		private bool _useExistingPlayer;
		private bool _useExistingAudio;
		private bool _useExistingBlockMesh;
		private bool _useExistingArt;

		#endregion
	}
}