using UnityEditor;
using UnityEngine;
using Universe.DebugWatch.Editor;
using Universe.DebugWatchTools.Runtime;
using Universe.Editor;
using Universe.SceneTask.Runtime;

using static UnityEditor.EditorGUIUtility;
using static UnityEditor.EditorGUILayout;

namespace Universe.Toolbar.Editor
{
	public class CreateSituationWindow : EditorWindow
	{
	    #region Exposed

	    public static float m_spacingSize = 5.0f;
	    public static float m_fieldSize = 20.0f;
	    
	    public static string m_editorWindowText = "Name : ";
	    public static LevelData m_target;
	    
	    public string m_newSituationName = "";
	    public TaskData m_blockMeshTask;
	    public TaskData m_artTask;
	    public bool m_isCheckpoint;
	    
	    #endregion


	    #region Main

	    public void OnGUI()
	    {
		    var preferedMinSize = minSize;
		    var preferedMaxSize = maxSize;

		    preferedMinSize.y = 65.0f;
		    preferedMaxSize.y = 65.0f;

		    m_newSituationName = TextField(m_editorWindowText, m_newSituationName);
		    
		    DrawTaskField( "Block mesh", ref m_blockMeshTask, ref _useExistingBlockMesh, ref preferedMinSize, ref preferedMaxSize);
		    DrawTaskField( "Art", ref m_artTask, ref _useExistingArt, ref preferedMinSize, ref preferedMaxSize);
		    m_isCheckpoint = Toggle("Is Checkpoint", m_isCheckpoint);

		    GUILayout.BeginHorizontal();
		    GUI.enabled = CanCreate();
		    if (GUILayout.Button("Create"))
		    {
			    var situationInfos = new SituationInfos
			    {
					m_name = m_newSituationName,
					m_blockMesh = m_blockMeshTask,
					m_art = m_artTask,
					m_gameplay = null,
					m_isCheckpoint = m_isCheckpoint
			    };
			    
			    if (m_target)
				    CreateLevelHelper.AddSituation(m_target, situationInfos);
			    else
				    CreateSituationHelper.CreateSituation(situationInfos);
			    
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

	    public static void DrawForm(string label, ref SituationInfos infos, ref bool useExistingBlockMesh, ref bool useExistingArt, ref Vector2 preferedMinSize, ref Vector2 preferedMaxSize)
	    {
		    BeginVertical();
		    
		    Space(m_spacingSize);
		    
		    LabelField(label);
		    
		    Space(m_spacingSize);

		    infos.m_name = TextField("Name : ", infos.m_name);
		    DrawTaskField("Block mesh", ref infos.m_blockMesh, ref useExistingBlockMesh, ref preferedMinSize, ref preferedMaxSize);
		    DrawTaskField("Art", ref infos.m_art, ref useExistingArt, ref preferedMinSize, ref preferedMaxSize);
		    
		    EndVertical();
		    
		    preferedMinSize.y += 10.0f;
		    preferedMaxSize.y += 10.0f;
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

	    public static void ShowSituationWindow(LevelData target = null)
	    {
		    var window = CreateInstance<CreateSituationWindow>();
		    var title = new GUIContent("Create new situation", IconContent(@"SceneSet Icon").image);
		    
		    m_target = target;
		    window.titleContent = title;
		    window.ShowUtility();
	    }

	    #endregion
	    
	    
	    #region Utils
	    
	    public bool CanCreate()
	    {
		    if(!IsUsingAnyExistingTask) 	return true;
		    if(IsUsingInvalidBlockMesh) 	return false;
		    if(IsUsingInvalidArt)			return false;

		    return true;
	    }

	    public bool IsUsingAnyExistingTask => 
		    _useExistingBlockMesh || _useExistingArt;
	    
	    public bool IsUsingInvalidBlockMesh => 
		    _useExistingBlockMesh && !m_blockMeshTask;
	    
	    public bool IsUsingInvalidArt => 
		    _useExistingArt && !m_artTask;
	    
	    #endregion
	    
	    
	    #region Private

	    private bool _useExistingBlockMesh;
	    private bool _useExistingArt;

	    #endregion
	}
}