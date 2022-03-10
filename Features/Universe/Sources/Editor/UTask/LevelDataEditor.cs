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
			DrawAddTaskButton();
			DrawDefaultInspector();
		}

		#endregion


		#region Main

		private void DrawAddTaskButton()
		{
			if(GUILayout.Button("Add Task"))
			{
				var level = target as LevelData;

				if(!level) return;

				CreateLevelHelper.AddTask(level);
			}
		}

		#endregion
	}
}