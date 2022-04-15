using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Collections.Generic;

using static UnityEngine.AddressableAssets.Addressables;
using static UnityEngine.SceneManagement.LoadSceneMode;

namespace Universe.SceneTask.Runtime
{
    public static class Task
    {
        #region Public

        public static AsyncOperationHandle<SceneInstance> m_focusSceneHandle;
        public static TaskData m_focusTask;
        public static SceneInstance m_focusScene;

        public static bool IsTheSameAsFocusedScene( TaskData taskData ) =>
            m_focusTask.m_assetReference != taskData.m_assetReference;

        public static bool HaveSamePriorityAsFocusScene( TaskData taskData ) =>
            m_focusTask.m_priority == taskData.m_priority;

        public static Scene GetFocusScene() =>
            m_focusScene.Scene;

        public static string GetFocusSceneName() =>
            m_focusScene.Scene.name;

        #endregion


        #region Main

        public static void ULoadTask( this UBehaviour source, TaskData task )
        {
            if( CheckIfSceneCanBeLoaded( task ) )
            {
                if( DicoDoesntContain( task ) )
                {
                    InitializeHandleList( task );
                }

                LoadScene( task );
            }
            else
            {
                Debug.LogError( $"ERROR Task: {task.name} is already loaded and cannot be loaded twice." );
            }
        }

        public static void UUnloadLastTaskAndLoad( this UBehaviour source, TaskData task )
        {
            if( HaveLoadedOneOrMoreTasks() )
            {
                Debug.LogError( "cannot unload previous scene, no scene are loaded." );
                LoadScene( task );
            }
            else
            {
                var previous = GetPreviousSceneLoaded();
                RemoveSceneHandleInDico( previous );
                UnloadScene( previous, task );
            }
        }

        public static void UUnloadTask( this UBehaviour source, TaskData task )
        {
            if( DicoDoesntContain( task ) ) return;
            var handle = GetLastHandleOfScene(task);
            RemoveHandleFromDico( task, handle );
            UnloadScene( handle );
        }

        public static SceneInstance GetFocusedScene()
        {
            if( IsSceneDicoEmpty() )
            {
                m_focusSceneHandle = new AsyncOperationHandle<SceneInstance>();
                return new SceneInstance();
            }

            _orderedKeys = _dicoOfTasks.Keys.OrderByDescending( task => task.m_priority );

            m_focusTask = _orderedKeys.First();
            var entry = _dicoOfTasks[m_focusTask];
            m_focusSceneHandle = entry[0];
            return m_focusSceneHandle.Result;
        }

        public static SceneInstance GetLoadedScene( TaskData from )
        {
            if( !_dicoOfTasks.ContainsKey( from ) )
            {
                Debug.LogError( $"Task: {from.name} isn't currently loaded" );
                return default( SceneInstance );
            }

            var task = _dicoOfTasks[from];
            var sceneHandle = task[0];

            return sceneHandle.Result;
        }

        #endregion


        #region Main Internal

        private static void LoadScene( TaskData sceneData )
        {
            var handle = LoadSceneAsync(sceneData.m_assetReference, Additive);

            _dicoOfTasks[sceneData].Add( handle );
            _taskHandlesList.Add( handle );
            handle.Completed += OnSceneLoaded;
        }

        private static void UnloadScene( AsyncOperationHandle<SceneInstance> scene, TaskData sceneToLoadAfter = null )
        {
            _taskHandlesList.Remove( scene );
            RemoveSceneHandleInDico( scene );
            UnloadSceneAsync( scene ).Completed += SceneUnloadComplete( sceneToLoadAfter );
        }

        #endregion


        #region Callbacks

        private static Action<AsyncOperationHandle<SceneInstance>> SceneUnloadComplete( TaskData task = null )
        {
            var highestScene = GetFocusedScene();
            var taskManager = GetTaskManagerOf(highestScene.Scene);

            taskManager.gameObject.SetActive( true );
            m_focusScene = highestScene;

            if( task ) ULoadTask( null, task );

            return null;
        }

        private static void OnSceneLoaded( AsyncOperationHandle<SceneInstance> go )
        {
            var highestScene = GetFocusedScene();
            var taskData = GetTaskDataOf(go);
            var taskManager = GetTaskManagerOf(go);

            if( !string.IsNullOrEmpty( m_focusScene.Scene.name ) )
            {
                var previousTaskManager = GetTaskManagerOf(m_focusScene.Scene);
                previousTaskManager.DisableTaskInputs();

                if( !previousTaskManager.IsAlwaysUpdated() )
                {
                    previousTaskManager.gameObject.SetActive( false );
                }
            }

            taskManager.SetAlwaysUpdated( taskData.m_alwaysUpdated );

            m_focusScene = highestScene;
        }

        #endregion


        #region TaskManager

        public static void Register( TaskManager target )
        {
            if( _taskManagers.Contains( target ) ) return;

            _taskManagers.Add( target );
        }

        public static void Unregister( TaskManager target )
        {
            if( !_taskManagers.Contains( target ) ) return;

            _taskManagers.Remove( target );
        }

        public static TaskManager GetFocusedTaskManager()
        {
            var focusedScene = GetFocusScene();

            return _taskManagers.FirstOrDefault( taskManager =>
                 focusedScene.name == taskManager.gameObject.scene.name );
        }

        public static TaskManager GetTaskManagerOf(UBehaviour target)
        {
            if(target.name.Contains("[TaskManager]")) Register((TaskManager)target);
            
            return _taskManagers.FirstOrDefault(taskManager =>
                target.gameObject.scene.name == taskManager.gameObject.scene.name);
        }

        private static TaskManager GetTaskManagerOf(AsyncOperationHandle<SceneInstance> go)
        {
            var scene = go.Result.Scene;

            return GetTaskManagerOf(scene);
        }

        private static TaskManager GetTaskManagerOf(Scene scene)
        {
            var rootGameObjects = new List<GameObject>();
            scene.GetRootGameObjects(rootGameObjects);

            var taskManagerRoot = rootGameObjects.Find((GameObject root) => root.name.Contains("[TaskManager]") || root.transform.Find("[TaskManager]"));
            var taskManager = taskManagerRoot.GetComponentInChildren<TaskManager>(true);

            return taskManager;
        }

        public static void RegisterUpdate(UBehaviour target)
        {
            var taskManager = GetTaskManagerOf(target);
            if( taskManager == null ) return;
            taskManager.AddToUpdate(target);
        }

        public static void RegisterFixedUpdate(UBehaviour target)
        {
            var taskManager = GetTaskManagerOf(target);
            if( taskManager == null ) return;
            taskManager.AddToFixedUpdate(target);
        }

        public static void RegisterLateUpdate(UBehaviour target)
        {
            var taskManager = GetTaskManagerOf(target);
            if( taskManager == null ) return;
            taskManager.AddToLateUpdate(target);
        }
        
        public static void UnregisterUpdate(UBehaviour target)
        {
            var taskManager = GetTaskManagerOf(target);
            taskManager?.RemoveFromUpdate(target);
        }

        public static void UnregisterFixedUpdate(UBehaviour target)
        {
            var taskManager = GetTaskManagerOf(target);
            taskManager?.RemoveFromFixedUpdate(target);
        }

        public static void UnregisterLateUpdate(UBehaviour target)
        {
            var taskManager = GetTaskManagerOf(target);
            taskManager?.RemoveFromLateUpdate(target);
        }
        #endregion


        #region Utils

        public static void LogDisplaySceneOrder()
        {
            for (var i = 0; i < _orderedKeys.Count(); i++)
            {
                var taskData = _orderedKeys.ElementAt(i);
                var sceneList = _dicoOfTasks[taskData];
                for (var j = sceneList.Count - 1; j >= 0; j--)
                {
                    Debug.Log($"sceneTaskData.name = {taskData.name}, priority = {taskData.m_priority}");
                    Debug.Log($"       scene index = {j}, sceneHandle = {sceneList[j]}");
                }
            }
        }

        private static TaskData GetTaskDataOf(AsyncOperationHandle<SceneInstance> scene)
        {
            var entry = _dicoOfTasks.FirstOrDefault((KeyValuePair<TaskData, List<AsyncOperationHandle<SceneInstance>>> entry) => entry.Value.Contains(scene));
            var taskData = entry.Key;

            return taskData;
        }

        #region Load Scene

        private static bool CheckIfSceneCanBeLoaded(TaskData task)
        {
            return (DicoContains(task) && CanHaveMultipleInstancesOf(task)) || DicoDoesntContain(task);
        }

        private static void InitializeHandleList(TaskData task)
        {
            _dicoOfTasks.Add(task, new());
        }

        private static bool CanHaveMultipleInstancesOf(TaskData task) => !task.m_canBeLoadOnlyOnce;
        private static bool DicoDoesntContain(TaskData task) => !_dicoOfTasks.ContainsKey(task);
        private static bool DicoContains(TaskData task) => _dicoOfTasks.ContainsKey(task);

        #endregion


        #region Unload Scene

        private static void RemoveSceneHandleInDico(AsyncOperationHandle<SceneInstance> previous)
        {
            foreach (var entry in _dicoOfTasks)
            {
                if (entry.Value.Contains(previous))
                {
                    RemoveHandleFromDico(entry.Key, previous);
                    break;
                }
            }
        }

        private static void RemoveHandleFromDico(TaskData task, AsyncOperationHandle<SceneInstance> handle)
        {
            _dicoOfTasks[task].Remove(handle);

            if (_dicoOfTasks[task].Count == 0)
            {
                _dicoOfTasks.Remove(task);
            }
        }

        private static AsyncOperationHandle<SceneInstance> GetLastHandleOfScene(TaskData task) => _dicoOfTasks[task].Last();
        private static AsyncOperationHandle<SceneInstance> GetPreviousSceneLoaded() => _taskHandlesList[^1];
        private static bool HaveLoadedOneOrMoreTasks() => _taskHandlesList.Count() == 0;
        private static bool IsSceneDicoEmpty() => _dicoOfTasks.Count == 0;

        #endregion

        #endregion


        #region Private and Protected Members

        private static Dictionary<TaskData, List<AsyncOperationHandle<SceneInstance>>> _dicoOfTasks = new();
        private static List<AsyncOperationHandle<SceneInstance>> _taskHandlesList = new();
        private static IEnumerable<TaskData> _orderedKeys = new List<TaskData>();

        private static List<TaskManager> _taskManagers = new();

        #endregion
    }
}
