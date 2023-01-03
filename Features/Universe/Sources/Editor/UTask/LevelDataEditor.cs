using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Universe.SceneTask.Runtime;
using Universe.Toolbar.Editor;

namespace Universe.SceneTask.Editor
{
	[CustomEditor(typeof(LevelData))]
	public class LevelDataEditor : OdinEditor
	{
		#region Unity API

		public override void OnInspectorGUI()
		{
			DrawAddSituationButton();
			DrawDefaultInspector();
			
			serializedObject.ApplyModifiedProperties();
		}

		#endregion


		#region Main

		private void DrawAddSituationButton()
		{
			if(!GUILayout.Button("Add Situation")) return;
			
			var level = target as LevelData;
			if(!level) return;

			CreateSituationWindow.ShowSituationWindow(level);
		}

		#endregion
	}
}