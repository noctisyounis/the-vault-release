using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.SceneTemplate;
using UnityEditor.AddressableAssets.Settings;
using Unity.EditorCoroutines.Editor;
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
		public static Action OnTaskAdded;

		#endregion


		#region Main

		public static void NewLevel(string name, TaskData audioData, TaskData blockData, TaskData artData)
        {
			if(_isGenerating)
			{
				LogWarning("Another level is being generated, wait for its completion before creating another");
				return;
			}
            ReadSettings();

            _levelName = name;
            _defaultLevelFolder = Join(_targetFolder, _levelName);
            _currentLevelFolder = _defaultLevelFolder;

            GenerateHierarchy();
            GenerateAaGroup();
            EditorCoroutineUtility.StartCoroutineOwnerless(GenerateAssets(audioData, blockData, artData));
        }

		public static void AddTask(LevelData level)
		{
			if(_isGenerating)
			{
				LogWarning("Another level is being generated, wait for its completion before creating another");
				return;
			}

			ReadSettings();
			FindHelper();

			var levelPath = GetAssetPath(level);

			_levelName = level.name;
			
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

			EditorCoroutineUtility.StartCoroutineOwnerless(GenerateAddedGameplay(level));
		}

        private static IEnumerator GenerateAssets(TaskData audioData, TaskData blockData, TaskData artData)
        {
            var level 			= GenerateLevelAsset();
			audioData			??= GenerateTask(_targetAudio, AUDIO, false);
            blockData 			??= GenerateTask(_targetBlock, ENVIRONMENT, true);
            artData 			??= GenerateTask(_targetArt, ENVIRONMENT, true);
            var gameplayData 	= GenerateTask(_targetGameplay, GAMEPLAY, true);

			_isGenerating = true;
			yield return 0;
			_isGenerating = false;

			level.m_audio = audioData;
            level.m_blockMeshEnvironment = blockData;
            level.m_artEnvironment = artData;
            level.m_gameplayTasks.Add(gameplayData);
			EditorUtility.SetDirty(level);

			Log($"<color=lime>{level.name} generated successfully</color>");
			OnLevelCreated?.Invoke();
			SaveAssets();
			Refresh();
        }

		private static IEnumerator GenerateAddedGameplay(LevelData level)
		{
			var taskIndex 	= level.m_gameplayTasks.Count + 1;
			var gameplayName 	= $"{_levelName}-{_gameplayTaskName}-{taskIndex:00}";

			_currentGameplayFolder = Join(_currentLevelFolder, _gameplayTaskName, gameplayName);
			_currentGameplayFolder = FolderHelper.CreatePath(_currentGameplayFolder);
			_targetGameplay = Join(_currentGameplayFolder, $"{gameplayName}.unity");

			var gameplayData = GenerateTask(_targetGameplay, GAMEPLAY, true);

			_isGenerating = true;
			yield return 0;
			_isGenerating = false;

			level.m_gameplayTasks.Add(gameplayData);

			Log($"<color=lime>{gameplayName} generated successfully</color>");
			EditorUtility.SetDirty( level );
			OnTaskAdded?.Invoke();
			SaveAssets();
			Refresh();
		}

        #endregion


        #region Utils

        private static void ReadSettings()
		{
			if(!_settings) LoadSettings();

			_targetFolder = _settings.m_levelFolder;
			
			var helperName = _settings.m_addressableGroupHelperName;
			_targetHelper = Join(_targetFolder, $"{helperName}.asset");

			_audioTaskName			= _settings.m_audioTaskName;
			_blockMeshTaskName 		= _settings.m_blockMeshTaskName;
			_artTaskName			= _settings.m_artTaskName;
			_gameplayTaskName		= _settings.m_gameplayTaskName;
			_sceneTemplate 			= _settings.m_sceneTemplate; 
			_addressableTemplate	= _settings.m_addressableGroupTemplate;
		}

		private static void LoadSettings()
		{
			_settings = USettingsHelper.GetSettings<CreateLevelSettings>();
		}

        private static void GenerateHierarchy()
        {
            if (!IsValidFolder(_targetFolder)) FolderHelper.CreatePath(_targetFolder);
            FindHelper();

			var audioName       = $"{_levelName}-{_audioTaskName}";
			var blockMeshName 	= $"{_levelName}-{_blockMeshTaskName}";
			var artName			= $"{_levelName}-{_artTaskName}";
			var gameplayName 	= $"{_levelName}-{_gameplayTaskName}-01";

			_currentAudioFolder		= Join( _currentLevelFolder, _audioTaskName );
			_currentBlockMeshFolder = Join(_currentLevelFolder, _blockMeshTaskName);
			_currentArtFolder 		= Join(_currentLevelFolder, _artTaskName);
			_currentGameplayFolder 	= Join(_currentLevelFolder, _gameplayTaskName, gameplayName);

			_currentLevelFolder 	= FolderHelper.CreatePath(_defaultLevelFolder);
			_currentAudioFolder		= FolderHelper.CreatePath(_currentAudioFolder);
			_currentBlockMeshFolder = FolderHelper.CreatePath(_currentBlockMeshFolder);
			_currentArtFolder		= FolderHelper.CreatePath(_currentArtFolder);
			_currentGameplayFolder	= FolderHelper.CreatePath(_currentGameplayFolder);

			_targetAudio	= Join(_currentAudioFolder, $"{audioName}.unity");
			_targetBlock 	= Join(_currentBlockMeshFolder, $"{blockMeshName}.unity");
			_targetArt 		= Join(_currentArtFolder, $"{artName}.unity");
			_targetGameplay = Join(_currentGameplayFolder, $"{gameplayName}.unity");
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
			{
				_helper = LoadAssetAtPath<UAddressableGroupHelper>(_targetHelper);
			}
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

			EditorUtility.SetDirty(level);
			SaveAssets();
			Refresh();
			return level;
		}

		private static TaskData GenerateTask(string path, TaskPriority priority, bool isAdditive)
		{
			var fullPath = GetFullPath(path);
			var dataPath = path.Replace(".unity", ".asset");

			if(Exists(fullPath)) return LoadAssetAtPath<TaskData>(dataPath);

			if(_sceneTemplate)
			{
				SceneTemplateService.Instantiate(_sceneTemplate, isAdditive, path);
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
			taskData.m_alwaysUpdated 		= (priority == AUDIO) ? true : false;
			taskData.m_canBeLoadOnlyOnce 	= true;
			CreateAsset(taskData, dataPath);

			var taskGuid = GUIDFromAssetPath(dataPath).ToString();
			var group = _helper.TryToFindGroup();

			CreateAaEntry(Settings, sceneGuid, group);
			CreateAaEntry(Settings, taskGuid, group);

			EditorUtility.SetDirty(taskData);
			SaveAssets();
			Refresh();
			return taskData;
		}

		#endregion


		#region Private

		private static CreateLevelSettings _settings;
		private static string _targetFolder;
		private static string _levelName;
		private static string _defaultLevelFolder;
		private static string _currentLevelFolder;
		private static string _currentAudioFolder;
		private static string _currentBlockMeshFolder;
		private static string _currentArtFolder;
		private static string _currentGameplayFolder;
		private static string _targetHelper;
		private static UAddressableGroupHelper _helper;
		private static string _audioTaskName;
		private static string _blockMeshTaskName;
		private static string _artTaskName;
		private static string _gameplayTaskName;
		private static string _addressableGroupHelperName;
		private static string _targetAudio;
		private static string _targetBlock;
		private static string _targetArt;
		private static string _targetGameplay;
		private static SceneTemplateAsset _sceneTemplate;
		private static AddressableAssetGroupTemplate _addressableTemplate;
		private static bool _isGenerating;
		
		#endregion
    }
}