using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Universe.SceneTask.Runtime;

using static System.IO.File;
using static System.IO.Path;
using static UnityEditor.AssetDatabase;
using static UnityEditor.EditorGUIUtility;
using static UnityEditor.SceneManagement.EditorSceneManager;
using static UnityEditor.SceneManagement.OpenSceneMode;
using static UnityEngine.Mathf;
using static UnityEngine.TextAnchor;
using static UnityEngine.GUILayout;
using static UnityEngine.PlayerPrefs;
using static UnityEngine.SceneManagement.SceneManager;
using static Universe.SceneTask.Runtime.Environment;

namespace Universe.Toolbar.Editor
{
    public class OpenLevelWindow : EditorWindow
	{
		#region Public API

		public string m_levelPlayerPref = "EditorLevelPath";
		public int m_maxEntryPerPage = 15;
		public float m_spacing = 20.0f;

        #endregion


        #region Unity API

        public static void ShowWindow()
        {
            var window = GetWindow<OpenLevelWindow>();
            
            SetStyleTo( window );
        }

        private void OnGUI()
        {
			DrawOpenButton();

			Space( m_spacing );

			BeginHorizontal();
			SelectLevel.Draw("Level", m_levelPlayerPref );
			EndHorizontal();

			var currentLevelPath = GetString(m_levelPlayerPref);

			if( !IsValidPath( currentLevelPath ) ) return;

			UpdateLevel( currentLevelPath );

			Space( m_spacing );

			BeginHorizontal();
			DrawAudioToggle();
			DrawPlayerToggle();
			EndHorizontal();

			Space( m_spacing );

			BeginHorizontal();
			Label( "Environment" );
			DrawEnvironmentToggle( BLOCK_MESH );
			DrawEnvironmentToggle( ART );
			EndHorizontal();

			Space( m_spacing );

			BeginVertical();
			BeginHorizontal();
			Label( "Tasks" );
			DrawSelectAllButton();
			DrawDeselectAllButton();
			EndHorizontal();

			DrawPageContent();

			Space( 7.0f );
			
			DrawPageSelection();
			EndVertical();
        }

		#endregion


		#region Main

		private void UpdateLevel( string path )
		{
			if( path.Equals(_levelPath) ) return;

			var level = LoadAssetAtPath<LevelData>(path);

			_levelPath	= path;
			_level		= level;
			_taskAmount = _level.Situations.Count;
			_audioUsed = IsTaskOpen( _level.m_audio );
			_playerUsed = IsTaskOpen( _level.m_player );
			_currentEnvironment = BLOCK_MESH;
			PopulateTaskUsed( _level.Situations );

			var floatPageAmount = 1.0f * _taskAmount / m_maxEntryPerPage;

			_pageAmount = CeilToInt( floatPageAmount );
		}

		private void DrawAudioToggle()
		{
			var willTurnOn = !_audioUsed;
			var texName    = willTurnOn ? "d_CacheServerDisconnected" : "d_CacheServerConnected";
			var tex        = IconContent(texName).image;

			if( !Button( new GUIContent( "Audio", tex, $"{( willTurnOn ? "Load" : "Unload" )}" ) ) )
				return;

			_audioUsed = !_audioUsed;
		}

		private void DrawPlayerToggle()
		{
			var willTurnOn = !_playerUsed;
			var texName    = willTurnOn ? "d_CacheServerDisconnected" : "d_CacheServerConnected";
			var tex        = IconContent(texName).image;

			if( !Button( new GUIContent( "Player", tex, $"{( willTurnOn ? "Load" : "Unload" )}" ) ) )
				return;

			_playerUsed = !_playerUsed;
		}

		private void DrawEnvironmentToggle( Environment environment )
		{
			var willTurnOn = (_currentEnvironment & environment) == 0;
			var texName    = willTurnOn ? "d_CacheServerDisconnected" : "d_CacheServerConnected";
			var tex        = IconContent(texName).image;

			if( !Button( new GUIContent( environment.ToString(), tex, $"{( willTurnOn ? "Load" : "Unload" )} {environment}" ) ) )
				return;
		
			var next = _currentEnvironment ^ environment;

			if( next != 0 )
			{
				_currentEnvironment = next;
				return;
			}

			_currentEnvironment = BOTH ^ environment;
		}

		private void DrawSelectAllButton()
		{
			if( !Button( "Select All" ) ) return;
			
			for( var i = 0; i < _taskAmount; i++ )
				_taskUsed[i] = true;
		}

		private void DrawDeselectAllButton()
		{
			if( !Button( "Deselect All" ) ) return;

			for( var i = 0; i < _taskAmount; i++ )
				_taskUsed[i] = false;
		}

		private void DrawPageSelection()
		{
			if( _pageAmount <= 1 ) return;
			
			var texPrev = IconContent("d_Animation.PrevKey").image;
			var texNext = IconContent("d_Animation.NextKey").image;

			BeginHorizontal();
			if( Button( new GUIContent( texPrev ) ) )
			{
				if( _currentPage == 0 )
					_currentPage = _pageAmount - 1;
				else
					_currentPage--;
			}

            var labelStyle = new GUIStyle( GUI.skin.label )
            {
                alignment = MiddleCenter
            };

            Label( $"Page {( _currentPage + 1 ):00}/{_pageAmount:00}", labelStyle );
			
			if( Button( new GUIContent( texNext ) ) )
			{
				if( _currentPage < _pageAmount - 1 )
					_currentPage++;
				else
					_currentPage = 0;
			}
			EndHorizontal();
		}

		private void DrawPageContent()
		{
			var start = _currentPage * m_maxEntryPerPage;

			BeginVertical();

			for( var i = 0; i < m_maxEntryPerPage; i++ )
			{
				var index = start + i;
				if( !_level.Situations.GreaterThan( index ) ) break;

				DrawSituationEntry( index );
			}

			EndVertical();
		}

		private void DrawSituationEntry(int index)
		{
			var willTurnOn		= !_taskUsed[index];
			var texName		= willTurnOn ? "d_CacheServerDisconnected" : "d_CacheServerConnected";
			var tex			= IconContent(texName).image;
			var situationName	= _level.Situations[index].m_name;
            var style = new GUIStyle( GUI.skin.button )
            {
                alignment = MiddleLeft
            };

            if( !Button(new GUIContent($"{(index + 1):000}:\t{situationName}", tex), style ) ) return;

			_taskUsed[index] = willTurnOn;
		}

		private void DrawOpenButton()
		{
            var style = new GUIStyle( GUI.skin.button )
            {
                fontStyle	= FontStyle.Bold,
                fixedHeight = 27.0f,
                fontSize	= 20
            };

            if( !Button( new GUIContent( "Open", "Open selected level" ), style ) ) return;

			var openSceneMode = Single;

			SaveCurrentModifiedScenesIfUserWantsTo();
			if( NeedAudio )
			{
				var audioGuid   = _level.m_audio.m_assetReference.AssetGUID;
				var audio       = GUIDToAssetPath(audioGuid);

				OpenScene( audio, openSceneMode );
				openSceneMode = Additive;
			}

			if( NeedPlayer )
			{
				var playerGuid	= _level.m_player.m_assetReference.AssetGUID;
				var player      = GUIDToAssetPath(playerGuid);

				OpenScene( player, openSceneMode );
				openSceneMode = Additive;
			}

			for( var i = 0; i < _taskAmount; i++ )
			{
				if( !_taskUsed[i] ) continue;

				var situation = _level.Situations[i];
				
				if( NeedBlockMesh )
				{
					var blockMeshGuid   = situation.m_blockMeshEnvironment.m_assetReference.AssetGUID;
					var blockMesh       = GUIDToAssetPath(blockMeshGuid);

					OpenScene( blockMesh, openSceneMode );
					openSceneMode = Additive;
				}

				if( NeedArt )
				{
					var artGuid         = situation.m_artEnvironment.m_assetReference.AssetGUID;
					var art             = GUIDToAssetPath(artGuid);
				
					OpenScene( art, openSceneMode );
					openSceneMode = Additive;
				}

				var gameplayGuid    = situation.m_gameplay.m_assetReference.AssetGUID;
				var gameplay        = GUIDToAssetPath(gameplayGuid);

				OpenScene( gameplay, openSceneMode );
			}

			Situation.CurrentEnvironment = _currentEnvironment;
			Close();
		}

		#endregion


		#region Utils

		private static void SetStyleTo( OpenLevelWindow window )
		{
			var windowRect = window.position;
			var minSize = new Vector2(300.0f, 550.0f);

			window.titleContent = new GUIContent( "Select Level To Open" );
			window._currentEnvironment = BOTH;
			window.minSize = minSize;
			windowRect.size = minSize;
			window.position = windowRect;
		}

		private void PopulateTaskUsed( List<SituationData> source )
		{
			var amount = source.Count;

			_taskUsed = new bool[amount];

			for( var i = 0; i < amount; i++ )
			{
				var situationData    = source[i];

				_taskUsed[i] = IsTaskOpen(situationData.m_gameplay);
			}
		}

		private bool IsTaskOpen( TaskData taskData )
		{
			var taskGuid    = taskData.m_assetReference.AssetGUID;
			var taskPath    = GUIDToAssetPath(taskGuid);
			var task        = GetSceneByPath(taskPath);

			return task.IsValid();
		}

		private static bool IsValidPath( string path )
		{
			if( string.IsNullOrEmpty( path ) ) return false;

			var fullPath = GetFullPath(path);
			
			return Exists( fullPath );
		}

		private bool NeedAudio => 
			_audioUsed;
		private bool NeedPlayer =>
			_playerUsed;
		private bool NeedBlockMesh =>
			(_currentEnvironment & BLOCK_MESH) != 0;
		private bool NeedArt =>
			( _currentEnvironment & ART ) != 0;

		#endregion


		#region Private

		private string _levelPath;
		private LevelData _level;
		private int _taskAmount;
		private int _pageAmount;
		private int _currentPage;
		private bool _audioUsed;
		private bool _playerUsed;
		private Environment _currentEnvironment;
		private bool[] _taskUsed;

        #endregion
    }
}