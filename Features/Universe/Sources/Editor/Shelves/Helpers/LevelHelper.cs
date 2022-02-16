using System.Collections;
using System;
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

using static System.IO.Path;
using static UnityEditor.AssetDatabase;
using static UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject;
using static UnityEditor.SceneManagement.NewSceneMode;
using static Universe.UAddressableUtility;
using static Universe.SceneTask.TaskPriority;

namespace Universe.Toolbar.Editor
{
    static class CreateLevelHelper
	{
		#region Main

		public static void NewLevel(string name, TaskData blockData, TaskData artData)
        {
            ReadSettings();

            _levelName = name;
            _defaultLevelFolder = Join(_targetFolder, _levelName);
            _currentLevelFolder = _defaultLevelFolder;

            GenerateHierarchy();
            GenerateAaGroup();
            EditorCoroutineUtility.StartCoroutineOwnerless(GenerateAssets(blockData, artData));
        }

        private static IEnumerator GenerateAssets(TaskData blockData, TaskData artData)
        {
            var level = GenerateLevelAsset();
            artData ??= GenerateTask(_targetArt, ENVIRONMENT, false);
            blockData ??= GenerateTask(_targetBlock, ENVIRONMENT, false);
            var gameplayData = GenerateTask(_targetGameplay, GAMEPLAY, true);

			yield return new WaitForSecondsRealtime(0.5f);

            level.m_blockMeshEnvironment = blockData;
            level.m_artEnvironment = artData;
            level.m_gameplay = gameplayData;
        }

        #endregion


        #region Utils

        private static void ReadSettings()
		{
			if(!_settings) LoadSettings();

			_targetFolder = _settings.m_levelFolder;
			
			var helperName = _settings.m_addressableGroupHelperName;
			_targetHelper = Join(_targetFolder, $"{helperName}.asset");

			_blockMeshTaskName 		= _settings.m_blockMeshTaskName;
			_artTaskName			= _settings.m_artTaskName;
			_gameplayTaskName		= _settings.m_gameplayTaskName;
			_sceneTemplate 			= _settings.m_sceneTemplate; 
			_addressableTemplate	= _settings.m_addressableGroupTemplate;
		}

		private static void LoadSettings()
		{
			var fullPath = GetFullPath(_settingsPath);

			if(!IsValidFolder(_settingsFolder)) FolderHelper.CreatePath(_settingsFolder);
			if(!File.Exists(fullPath))
			{
				_settings = ScriptableObject.CreateInstance<CreateLevelSettings>();
				
				CreateAsset(_settings, _settingsPath);
				SaveAssets();
			}
			else
			{
				_settings = LoadAssetAtPath<CreateLevelSettings>(_settingsPath);
			}
		}

        private static void GenerateHierarchy()
        {
            var helperFullPath = GetFullPath(_targetHelper);

            if (!IsValidFolder(_targetFolder)) FolderHelper.CreatePath(_targetFolder);
            if (!File.Exists(helperFullPath))
            {
                _helper 			= ScriptableObject.CreateInstance<UAddressableGroupHelper>();
                _helper.m_groupName = "levels";
				_helper.m_template 	= _addressableTemplate;

                CreateAsset(_helper, _targetHelper);
                SaveAssets();
            }
			else
			{
				_helper = AssetDatabase.LoadAssetAtPath<UAddressableGroupHelper>(_targetHelper);
			}

			_currentLevelFolder = FolderHelper.CreatePath(_defaultLevelFolder);

			_targetBlock 	= Join(_currentLevelFolder, $"{_levelName}_{_blockMeshTaskName}.unity");
			_targetArt 		= Join(_currentLevelFolder, $"{_levelName}_{_artTaskName}.unity");
			_targetGameplay = Join(_currentLevelFolder, $"{_levelName}_{_gameplayTaskName}.unity");
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
			SaveAssets();
			level.name = _levelName;

			return level;
		}

		private static TaskData GenerateTask(string path, TaskPriority priority, bool isAdditive)
		{
			var fullPath = GetFullPath(path);
			var dataPath = path.Replace(".unity", ".asset");

			if(File.Exists(fullPath)) return LoadAssetAtPath<TaskData>(dataPath);

			if(_sceneTemplate)
			{
				SceneTemplateService.Instantiate(_sceneTemplate, isAdditive, path);
			}
			else
			{
				var mode 	= isAdditive ? Additive : NewSceneMode.Single;
				var scene 	= EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, mode);
				
				EditorSceneManager.SaveScene(scene, path);

				var directory = new DirectoryInfo(path).Name;

				Debug.LogWarning($"No scene template found, an empty scene has been generated for {directory}.");
			}
			
			var sceneGuid = GUIDFromAssetPath(path).ToString();
			CreateAaEntry(Settings, sceneGuid);

			var taskData = ScriptableObject.CreateInstance<TaskData>();
			taskData.m_assetReference		= new AssetReference(sceneGuid);
			taskData.m_priority				= priority;
			taskData.m_alwaysUpdated 		= false;
			taskData.m_canBeLoadOnlyOnce 	= true;
			CreateAsset(taskData, dataPath);
			SaveAssets();

			return taskData;
		}

		#endregion


		#region Private

		private static string _settingsFolder = "Assets/Settings/Universe";
		private static string _settingsPath = Join(_settingsFolder, "CreateLevelSettings.asset");
		private static CreateLevelSettings _settings;
		private static string _targetFolder;
		private static string _levelName;
		private static string _defaultLevelFolder;
		private static string _currentLevelFolder;
		private static string _targetHelper;
		private static UAddressableGroupHelper _helper;
		private static string _blockMeshTaskName;
		private static string _artTaskName;
		private static string _gameplayTaskName;
		private static string _addressableGroupHelperName;
		private static string _targetBlock;
		private static string _targetArt;
		private static string _targetGameplay;
		private static SceneTemplateAsset _sceneTemplate;
		private static AddressableAssetGroupTemplate _addressableTemplate;
		
		#endregion
    }
}