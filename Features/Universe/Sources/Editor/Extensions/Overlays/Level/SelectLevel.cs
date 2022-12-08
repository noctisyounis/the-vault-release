using System.Linq;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using Universe.Editor;
using Universe.SceneTask.Runtime;

using static System.IO.Path;
using static UnityEditor.AssetDatabase;
using static UnityEngine.PlayerPrefs;
using static UnityEngine.UIElements.FlexDirection;

namespace Universe.Overlays
{
	[EditorToolbarElement(ID, typeof(EditorWindow))]
	public class SelectLevel : EditorToolbarDropdown
	{
		#region Exposed

		public const string ID = "Level/SelectLevel";
		
		public string m_value;

		#endregion


		#region Events

		public delegate void OnValueChangedHandler(string path);
		public OnValueChangedHandler OnValueChanged; 

		#endregion
		
		
		#region Constructor

		public SelectLevel(string label, string playerPref)
		{
			_playerPref = playerPref;

			Initialize(label);

			clicked += ShowDropdown;
		}
		
		#endregion
		
		
		#region Main

		private void Initialize(string label)
		{
			FindLevelDatas();
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
            
            if (_levelPaths.Length == 0)
            {
            	text = "No existing level found.";
            }
            else
            {
            	var levelPath = GetString(_playerPref);
            	
            	_selection = FindAssociatedLevel(levelPath);
            	text = _levelNames[_selection];
            	m_value = _levelPaths[_selection];
            }
		}

		private void ShowDropdown()
		{
			var menu = new GenericMenu();
			var i = 0;
			var length = _levelNames.Length;
			
			while (i < length)
			{
				var index = i;
				var levelName = _levelNames[i];
				var levelPath = _levelPaths[i];
				
				menu.AddItem(new GUIContent(levelName), _selection == i, () =>
				{
					text = levelName;
					_selection = index;
					m_value = levelPath;
					
					SetString(_playerPref, m_value);
					UpdateCheckpoint();
					
					OnValueChanged?.Invoke(m_value);
				});
				
				i++;
			}
			
			menu.ShowAsContext();
		}
		
		#endregion
		
		
		#region Utils
		
		private int FindAssociatedLevel( string path )
		{
			if( string.IsNullOrEmpty( path ) )
				return 0;

			var pathList    = _levelPaths.ToList();
			var result   = pathList.IndexOf(path);

			return result;
		}
		
		private void FindLevelDatas()
		{
			var levels  = FindAssets($"t:{typeof(LevelData)}");
			var size    = levels.Length;

			_levelNames = new string[size];
			_levelPaths = new string[size];

			for( int i = 0; i < size; i++ )
			{
				var level       = levels[i];
				var path        = GUIDToAssetPath(level);
				var fullPath    = GetFullPath(path);

				_levelPaths[i] = path;
				_levelNames[i] = GetFileNameWithoutExtension( fullPath );
			}
		}

		private void UpdateCheckpoint()
		{
			var settings    = USettingsHelper.GetSettings<LevelSettings>();
			var checkpoint  = settings.m_editorCheckpoint; 
			var level       = LoadAssetAtPath<LevelData>(m_value);

			checkpoint.m_level = level;
			settings.SaveAsset();
		}
		
		#endregion

		
		#region Private

		private string	 _playerPref;
		private int		 _selection;
		private string[] _levelNames;
		private string[] _levelPaths;

		#endregion
	}
}