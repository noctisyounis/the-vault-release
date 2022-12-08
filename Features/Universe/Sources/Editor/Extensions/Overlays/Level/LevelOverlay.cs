using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

using static Universe.SceneTask.Runtime.Environment;
using static UnityEngine.UIElements.FlexDirection;
using static Universe.Editor.UPrefs;

namespace Universe.Overlays
{
	[Overlay(typeof(EditorWindow), ID, true)]
	[Icon(ICON_NAME)]
	public class LevelOverlay : ToolbarOverlay
	{
		#region Exposed

		public const string ID			= "Level";
		public const string ICON_NAME	= "d_SceneAsset Icon";

		public static string s_rootName				= "root";
		public static string s_labelName			= "Level";
		public static string s_contentName			= "content";
		public static string s_environmentName		= "environment";
		public static string s_playmodeOverrideName = "playmodeOverride";
		public static string s_playmodeLevelName	= "playmodeLevel";

		#endregion


		#region Main
		
		public override VisualElement CreatePanelContent()
		{
			var panel = new VisualElement { name = s_rootName };
			var contentBlock = BuildContentBlock();
			
			collapsed = true;

			panel.style.flexDirection = Column;
			
			panel.Add(new Label(s_labelName));
			panel.Add(contentBlock);

			return panel;
		}

		#endregion


		#region Utils

		private VisualElement BuildContentBlock()
		{
			var contentBlock = new VisualElement { name = s_contentName };
			var environmentBlock = BuildEnvironmentBlock();
			var playmodeOverrideBlock = BuildPlaymodeOverrideBlock();
			
			contentBlock.style.flexDirection = Row;

			contentBlock.Add(new OpenLevel());
			contentBlock.Add(new ToolbarSpacer());
			contentBlock.Add(environmentBlock);
			contentBlock.Add(new ToolbarSpacer());
			contentBlock.Add(playmodeOverrideBlock);

			return contentBlock;
		}

		private VisualElement BuildEnvironmentBlock()
		{
			var environmentBlock = new VisualElement { name = s_environmentName };
			
			environmentBlock.style.flexDirection = Column;
			
			_toggleBlock	= new(EDITOR_LEVEL_PATH, BLOCK_MESH, "Toggle the block mesh situations");
			_toggleArt		= new(EDITOR_LEVEL_PATH, ART, "Toggle the art situations"); 
			_toggleBlock.RegisterValueChangedCallback(_toggleArt.Refresh);
			_toggleArt.RegisterValueChangedCallback(_toggleBlock.Refresh);
			
			environmentBlock.Add(_toggleBlock);
			environmentBlock.Add(_toggleArt);

			return environmentBlock;
		}

		private VisualElement BuildPlaymodeOverrideBlock()
		{
			var playmodeOverrideBlock = new VisualElement { name = s_playmodeOverrideName };
			var playmodeSceneBlock = BuildPlaymodeSceneBlock();
			
			playmodeOverrideBlock.style.flexDirection = Column;
			
			playmodeOverrideBlock.Add(new TogglePlayOverride());
			playmodeOverrideBlock.Add(playmodeSceneBlock);

			return playmodeOverrideBlock;
		}

		private VisualElement BuildPlaymodeSceneBlock()
		{
			var playmodeSceneBlock = new VisualElement { name = s_playmodeLevelName };
			
			playmodeSceneBlock.style.flexDirection = Row;
			
			_selectLevel	= new("Play:", EDITOR_LEVEL_PATH);
			_selectTask		= new("On:");
			_selectTask.Refresh(_selectLevel.m_value);
			_selectLevel.OnValueChanged += _selectTask.Refresh;
			
			playmodeSceneBlock.Add(_selectLevel);
			playmodeSceneBlock.Add(_selectTask);

			return playmodeSceneBlock;
		}
		
		#endregion
		
		
		#region Private

		private SelectLevel			_selectLevel;
		private SelectTask			_selectTask;
		private ToggleEnvironment	_toggleBlock;
		private ToggleEnvironment	_toggleArt;

		#endregion
	}
}