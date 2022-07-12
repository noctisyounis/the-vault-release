using System;
using System.IO;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.SceneManagement;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Universe.Editor;
using Universe.SceneTask;
using Universe.SceneTask.Runtime;

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
	public static class CreateSituationHelper
	{
		#region Events

		public static Action<SituationData> OnSituationCreated;

		#endregion
		
		
		#region Main

		public static SituationData CreateSituation(SituationInfos infos, LevelData target = null) =>
			CreateSituation(infos.m_name, infos.m_blockMesh, infos.m_art, infos.m_gameplay, infos.m_isCheckpoint, target);
		
		public static SituationData CreateSituation(string name, TaskData blockData, TaskData artData, TaskData gameplayData, bool isCheckpoint, LevelData target = null)
		{
			if (_isGenerating)
			{
				LogWarning("Another situation is being generated, wait for its completion before creating another");
				return default;
			}

			ReadSettings();

			_situationName			= name.Replace(' ', '-');
			_defaultSituationFolder = Join(_targetFolder, _situationName);
			_currentSituationFolder = _defaultSituationFolder;

			if (target)
				BindPath(target);
			
			GenerateHierarchy();
			GenerateAaGroup();
			
			var situation = GenerateAssets(blockData, artData, gameplayData, isCheckpoint);

			if (target)
			{
				target.AddSituation(situation);
				target.SaveAsset();
			}
			
			return situation;
		}
		
		private static SituationData GenerateAssets(TaskData blockMeshData, TaskData artData, TaskData gameplayData, bool isCheckpoint)
		{
			var situation 		= GenerateSituationAsset();
			
			blockMeshData		??= GenerateTask(_targetBlockMesh, GAMEPLAY, _settings.m_blockMeshSceneTemplate, true);
			artData				??= GenerateTask(_targetArt, GAMEPLAY, _settings.m_artSceneTemplate, true);
			gameplayData		??= GenerateTask(_targetGameplay, GAMEPLAY, _settings.m_gameplaySceneTemplate, true);

			var shortName = Shorten(situation.name);

			situation.m_name					= shortName;
			situation.m_blockMeshEnvironment	= blockMeshData;
			situation.m_artEnvironment			= artData;
			situation.m_gameplay				= gameplayData;
			situation.m_isCheckpoint			= isCheckpoint;

			Log($"<color=lime>{situation.name} generated successfully</color>");
			OnSituationCreated?.Invoke(situation);
			situation.SaveAsset();
			return situation;
		}
		
		#endregion
		
		
		#region Utils

		private static void ReadSettings()
		{
			if (!_settings) LoadSettings();
			var helperName = _settings.m_addressableGroupHelperName;
			
			_levelFolder		= _settings.m_levelFolder;
			_targetHelper		= Join(_levelFolder, $"{helperName}.asset");
			_targetFolder		= Join(_levelFolder, _settings.m_situationName);
			_blockMeshTaskName	= _settings.m_blockMeshTaskName;
			_artTaskName		= _settings.m_artTaskName;
			_gameplayTaskName	= _settings.m_gameplayTaskName;
		}

		private static void LoadSettings() =>
			_settings = USettingsHelper.GetSettings<CreateLevelSettings>();

		private static void BindPath(LevelData to)
		{
			var path = GetAssetPath(to);
			
			path = path.Replace($"/{to.name}.asset", "");
			_currentSituationFolder = _currentSituationFolder.Replace($"{_targetFolder}\\", "");
			_targetFolder			= Join(path, _settings.m_situationName);
			_currentSituationFolder = Join(_targetFolder, _currentSituationFolder);
		}

		private static string Shorten(string name)
		{
			var fragments = name.Split('-');
			var length = fragments.Length;
			
			if (length < 4)
				return name;

			name = fragments[3];
			for (var i = 4; i < length; i++)
			{
				name += $" {fragments[i]}";
			}
			
			return name;
		}

		private static void GenerateHierarchy()
		{
			if (!IsValidFolder(_targetFolder)) FolderHelper.CreatePath(_targetFolder);
			FindHelper();

			var blockMeshName		= $"{_situationName}-{_blockMeshTaskName}";
			var artName				= $"{_situationName}-{_artTaskName}";
			var gameplayName		= $"{_situationName}-{_gameplayTaskName}";

			_currentSituationFolder = FolderHelper.CreatePath(_currentSituationFolder);
			_targetBlockMesh		= Join(_currentSituationFolder, $"{blockMeshName}.unity");
			_targetArt				= Join(_currentSituationFolder, $"{artName}.unity");
			_targetGameplay			= Join(_currentSituationFolder, $"{gameplayName}.unity");
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
		
		private static SituationData GenerateSituationAsset()
		{
			var situation 	= ScriptableObject.CreateInstance<SituationData>();
			var path 	= Join(_currentSituationFolder, $"{_situationName}.asset");

			CreateAsset(situation, path);
			situation.name = _situationName;

			var levelGuid = GUIDFromAssetPath(path).ToString();

			CreateAaEntry(Settings, levelGuid, _helper.m_group);

			situation.SaveAsset();
			return situation;
		}
		
		private static TaskData GenerateTask(string path, TaskPriority priority, SceneTemplateAsset template, bool isAdditive)
		{
			var fullPath = GetFullPath(path);
			var dataPath = path.Replace(".unity", ".asset");

			if(Exists(fullPath)) return LoadAssetAtPath<TaskData>(dataPath);

			if(template)
				SceneTemplateService.Instantiate(template, isAdditive, path);
			else
			{
				var mode 	= isAdditive ? Additive : NewSceneMode.Single;
				var scene 	= NewScene(NewSceneSetup.EmptyScene, mode);
				
				SaveScene(scene, path);

				var directory = new DirectoryInfo(path).Name;

				LogWarning($"No scene template found, an empty scene has been generated for {directory}.");
			}
			
			var sceneGuid	= GUIDFromAssetPath(path).ToString();
			var taskData	= ScriptableObject.CreateInstance<TaskData>();

			taskData.m_assetReference		= new(sceneGuid);
			taskData.m_priority				= priority;
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
		private static string _levelFolder;
		private static string _targetFolder;
		private static string _situationName;
		private static string _defaultSituationFolder;
		private static string _currentSituationFolder;
		private static string _currentLevelFolder;
		private static string _currentBlockMeshFolder;
		private static string _currentArtFolder;
		private static string _currentGameplayFolder;
		private static string _targetHelper;
		private static UAddressableGroupHelper _helper;
		private static string _blockMeshTaskName;
		private static string _artTaskName;
		private static string _gameplayTaskName;
		private static string _targetBlockMesh;
		private static string _targetArt;
		private static string _targetGameplay;
		private static bool _isGenerating;
		private static AddressableAssetGroupTemplate _addressableTemplate;

		#endregion
	}
}