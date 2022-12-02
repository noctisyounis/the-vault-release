using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using Universe.SceneTask.Runtime;

using static UnityEditor.AssetDatabase;
using static Universe.Editor.USettingsHelper;
using static UnityEngine.UIElements.FlexDirection;

namespace Universe.Overlays
{
	[EditorToolbarElement(ID, typeof(EditorWindow))]
	public class SelectTask : EditorToolbarDropdown
	{
		#region Exposed

		public const string ID = "Level/SelectTask";
		
		public int m_value;

		#endregion
		
		
		#region Events
		
		public delegate void OnValueChangedHandler(int index);
		public OnValueChangedHandler OnValueChanged; 
		
		#endregion
		
		
		#region Constructor

		public SelectTask(string label)
		{
			Initialize(label);
			
			clicked += ShowDropdown;
		}
		
		#endregion
		
		
		#region Main

		private void Initialize(string label)
		{
			InitializeLayout();
			InitializeValues(label);
		}

		private void InitializeLayout()
		{
			style.flexDirection = Row;
		}

		private void InitializeValues(string label)
		{
			text = label;
			
			var settings	= GetSettings<LevelSettings>();
			var checkpoint = settings.m_editorCheckpoint;
			var situation = checkpoint.m_situation;
			
			_currentLevel = checkpoint.m_level;
			m_value = _currentLevel.IndexOf(situation);
		}

		public void Refresh(string path) => Refresh(path, true);

		public void Refresh(string path, bool saveInSettings)
		{
			UpdateLevel(path, saveInSettings);
			PopulateTaskNames();

			if (_taskNames is null) return;
			if (!_taskNames.GreaterThan(m_value)) return;

			text = _taskNames[m_value];
		}

		private void ShowDropdown()
		{
			var menu	= new GenericMenu();
			var i		= 0;
			var length = _taskNames.Count;
			
			while (i < length)
			{
				var taskName = _taskNames[i];
				var index = i;
				
				menu.AddItem(new GUIContent(taskName), m_value == i, () =>
				{
					text	= taskName;
					m_value = index;
					
					UpdateCheckpoint();
					
					OnValueChanged?.Invoke(m_value);
				});
				
				i++;
			}
			
			menu.ShowAsContext();
		}
		
		#endregion
		
		
		#region Utils
		
		private void UpdateLevel( string path, bool saveInSettings )
		{
			if (string.IsNullOrEmpty(path)) return;
			if( path.Equals( _currentPath ) && _currentLevel ) return;

			var level = LoadAssetAtPath<LevelData>( path );
			
			_currentPath = path;

			if (!_currentLevel.Equals(level))
			{
				_currentLevel = level;
				m_value	= 0;
			}
			
			PopulateTaskNames();

			if( !saveInSettings ) return;
			
			UpdateCheckpoint();
		}

		private void UpdateCheckpoint()
		{
			var settings	= GetSettings<LevelSettings>();
			var checkpoint	= settings.m_editorCheckpoint;

			if( !_taskNames.GreaterThan( m_value ) )
				m_value = 0;

			checkpoint.m_level		= _currentLevel;
			checkpoint.m_situation	= _currentLevel.GetSituation( m_value );

			settings.SaveAsset();
		}

		private void PopulateTaskNames()
		{
			if (!_currentLevel) return;
			
			var situations = _currentLevel.Situations;
			if (situations is null) return;

			_taskNames = new();
			foreach( var situation in situations )
			{
				var taskName = situation.m_name;
				
				_taskNames.Add( taskName );
			}
		}
		
		#endregion
		
		
		#region Private

		private string			_currentPath;
		private LevelData		_currentLevel;
		private List<string>	_taskNames;

		#endregion
	}
}