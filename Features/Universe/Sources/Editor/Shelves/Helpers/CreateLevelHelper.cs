using System;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEditor.SceneTemplate;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Universe.SceneTask;
using Universe.SceneTask.Runtime;
using Universe.Editor;

using static System.IO.Path;
using static System.IO.File;
using static UnityEngine.Debug;
using static UnityEditor.AssetDatabase;
using static UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject;
using static UnityEditor.SceneManagement.NewSceneMode;
using static UnityEditor.SceneManagement.EditorSceneManager;
using static Universe.UAddressableUtility;
using static Universe.SceneTask.TaskPriority;

namespace Universe.Toolbar.Editor
{
    public static class CreateLevelHelper
	{
		#region Events

		public static Action OnLevelCreated;
		public static Action OnSituationAdded;

		#endregion


		#region Main

		public static void CreateLevel(string name, TaskData playerData, TaskData audioData, SituationInfos initialSituation)
		{
			if (_isGenerating)
			{
				LogWarning("Another level is being generated, wait for its completion before creating another");
				return;
			}

			ReadSettings();

			_levelName = name;
			_defaultLevelFolder = Join(_targetFolder, _levelName);
			_currentLevelFolder = _defaultLevelFolder;

			GenerateHierarchy(!playerData, !audioData);
			GenerateAaGroup();
			GenerateAssets(playerData, audioData, initialSituation);
		}

		public static void AddSituation(LevelData target, SituationInfos situationInfos)
		{
			ReadSettings();
			FindHelper();

			var levelPath = GetAssetPath(target);

			_levelName = target.name;
			
			if( !string.IsNullOrEmpty( levelPath ) )
			{
				var levelFolder = levelPath.Replace( $"{_levelName}.asset", "" );
				
				_currentLevelFolder = levelFolder;
			}
			else
			{
				_defaultLevelFolder = Join( _targetFolder, _levelName );
				_currentLevelFolder = _defaultLevelFolder;
			}
			
			GenerateAddedSituation(target, situationInfos);
			OnSituationAdded?.Invoke();
		}

        private static void GenerateAssets(TaskData playerData, TaskData audioData, SituationInfos initialSituation)
        {
            var level 	= GenerateLevelAsset();
            
			playerData	??= GenerateTask(_targetPlayer, GAMEPLAY, true, _settings.m_playerSceneTemplate, true);
			audioData	??= GenerateTask(_targetAudio, GAMEPLAY, false, _settings.m_audioSceneTemplate, true);
			
			if(string.IsNullOrEmpty(initialSituation.m_name)) 
				initialSituation.m_name = $"{_situationName}";
			else
				initialSituation.m_name = $"{_situationName}-{initialSituation.m_name}";

			CreateSituationHelper.CreateSituation(initialSituation, level);

			level.m_audio = audioData;
			level.m_player = playerData;

			Log($"<color=lime>{level.name} generated successfully</color>");
			OnLevelCreated?.Invoke();
			level.SaveAsset();
        }

        private static void GenerateAddedSituation(LevelData level, SituationInfos situation)
        {
	        var situationIndex = level.Situations.Count + 1;
	        
	        if(string.IsNullOrEmpty(situation.m_name)) 
		        situation.m_name = $"{_levelName}-{_situationName}-{situationIndex:00}";
	        else
				situation.m_name = $"{_levelName}-{_situationName}-{situationIndex:00}-{situation.m_name}";

	        CreateSituationHelper.CreateSituation(situation, level);
        }

        #endregion


        #region Utils

        private static void ReadSettings()
		{
			if(!_settings) LoadSettings();

			var helperName = _settings.m_addressableGroupHelperName;
			
			_targetFolder			= _settings.m_levelFolder;
			_targetHelper			= Join(_targetFolder, $"{helperName}.asset");
			_audioTaskName			= _settings.m_audioTaskName;
			_playerTaskName			= _settings.m_playerTaskName;
			_situationName			= _settings.m_situationName;
			_addressableTemplate	= _settings.m_addressableGroupTemplate;
		}

		private static void LoadSettings() =>
			_settings = USettingsHelper.GetSettings<CreateLevelSettings>();

        private static void GenerateHierarchy(bool newPlayer, bool newAudio)
        {
            if (!IsValidFolder(_targetFolder)) FolderHelper.CreatePath(_targetFolder);
            FindHelper();

			var playerName			= $"{_levelName}-{_playerTaskName}";
			var audioName			= $"{_levelName}-{_audioTaskName}";
			
			_situationName			= $"{_levelName}-{_situationName}-01";
			_currentLevelFolder 	= FolderHelper.CreatePath(_defaultLevelFolder);
			_currentPlayerFolder	= Join(_currentLevelFolder, _playerTaskName);
			_currentAudioFolder		= Join( _currentLevelFolder, _audioTaskName );
			
			if (newPlayer)
				_currentPlayerFolder	= FolderHelper.CreatePath(_currentPlayerFolder);
			if (newAudio)
				_currentAudioFolder		= FolderHelper.CreatePath(_currentAudioFolder);

			_targetPlayer	= Join(_currentPlayerFolder, $"{playerName}.unity");
			_targetAudio	= Join(_currentAudioFolder, $"{audioName}.unity");
        }

		private static void FindHelper()
		{
			var helperFullPath = GetFullPath(_targetHelper);

            if (!Exists(helperFullPath))
            {
                _helper 			= ScriptableObject.CreateInstance<UAddressableGroupHelper>();
                _helper.m_groupName = "levels";
				_helper.m_template 	= _addressableTemplate;

                CreateAsset(_helper, _targetHelper);
            }
			else
				_helper = LoadAssetAtPath<UAddressableGroupHelper>(_targetHelper);
		}

		private static void GenerateAaGroup()
		{
			_helper.GenerateNewGroup();
			_helper.SetGroupAsDefault();
		}

		private static LevelData GenerateLevelAsset()
		{
			var level 	= ScriptableObject.CreateInstance<LevelData>();
			var path 	= Join(_currentLevelFolder, $"{_levelName}.asset");

			CreateAsset(level, path);
			level.name = _levelName;

			var levelGuid = GUIDFromAssetPath(path).ToString();

			CreateAaEntry(Settings, levelGuid, _helper.m_group);
			level.SaveAsset();
			return level;
		}

		private static TaskData GenerateTask(string path, TaskPriority priority, bool hasInputPriority, SceneTemplateAsset template, bool isAdditive)
		{
			var fullPath = GetFullPath(path);
			var dataPath = path.Replace(".unity", ".asset");

			if(Exists(fullPath)) return LoadAssetAtPath<TaskData>(dataPath);

			if(template)
			{
				SceneTemplateService.Instantiate(template, isAdditive, path);
			}
			else
			{
				var mode 	= isAdditive ? Additive : NewSceneMode.Single;
				var scene 	= NewScene(NewSceneSetup.EmptyScene, mode);
				
				SaveScene(scene, path);
				
				var directory = new DirectoryInfo(path).Name;
				
				LogWarning($"No scene template found, an empty scene has been generated for {directory}.");
			}
			
			var sceneGuid = GUIDFromAssetPath(path).ToString();
			var taskData = ScriptableObject.CreateInstance<TaskData>();

			taskData.m_assetReference		= new AssetReference(sceneGuid);
			taskData.m_priority				= priority;
			taskData.m_inputPriority		= hasInputPriority;
			taskData.m_canBeLoadOnlyOnce 	= true;
			CreateAsset(taskData, dataPath);

			var taskGuid = GUIDFromAssetPath(dataPath).ToString();
			var group = _helper.TryToFindGroup();

			CreateAaEntry(Settings, sceneGuid, group);
			CreateAaEntry(Settings, taskGuid, group);
			taskData.SaveAsset();
			return taskData;
		}

		#endregion


		#region Private

		private static CreateLevelSettings _settings;
		private static string _targetFolder;
		private static string _levelName;
		private static string _defaultLevelFolder;
		private static string _currentLevelFolder;
		private static string _currentPlayerFolder;
		private static string _currentAudioFolder;
		private static string _currentSituationFolder;
		private static string _targetHelper;
		private static UAddressableGroupHelper _helper;
		private static string _audioTaskName;
		private static string _playerTaskName;
		private static string _situationName;
		private static string _addressableGroupHelperName;
		private static string _targetAudio;
		private static string _targetPlayer;
		private static string _targetSituation;
		private static AddressableAssetGroupTemplate _addressableTemplate;
		private static bool _isGenerating;
		
		#endregion
    }
}