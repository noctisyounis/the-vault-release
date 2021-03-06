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

        

        #endregion


        #region Events

        public static Action<TaskData> OnTaskLoaded;

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

        public static SceneInstance GetLoadedScene( TaskData from )
        {
            if( !_dicoOfTasks.ContainsKey( from ) )
            {
                return default;
            }

            var task = _dicoOfTasks[from];
            var sceneHandle = task[0];

            return sceneHandle.Result;
        }

        public static bool IsLoaded( TaskData target ) =>
            GetLoadedScene( target ).Scene.IsValid();

        public static void SetFocus( TaskData to )
        {
            if( IsSceneDicoEmpty() )
            {
                m_focusSceneHandle = new AsyncOperationHandle<SceneInstance>();
                m_focusScene = default( SceneInstance );
                m_focusTask = null;
                _focusPriority = TaskPriority.ALWAYS_UPDATE;
                return;
            }

            var scene = GetLoadedScene(to);
            if( !scene.Scene.IsValid() )
                return;

            var previousManager = GetTaskManagerOf(m_focusScene.Scene);
            previousManager?.DisableTaskInputs();

            m_focusScene = scene;
            m_focusTask = to;
            _focusPriority = m_focusTask.m_priority;

            var manager = GetTaskManagerOf(m_focusScene.Scene);
            manager.EnableTaskInputs();

            var handles = _dicoOfTasks[m_focusTask];
            m_focusSceneHandle = handles[0];
        }

        public static SceneInstance GetFocusSceneInstance() =>
            m_focusSceneHandle.Result;

        public static Scene GetFocusScene() =>
            m_focusScene.Scene;
        
        public static string GetFocusSceneName() =>
            GetFocusScene().name;
        
        public static TaskPriority GetFocusPriority() =>
            _focusPriority;

        public static bool IsSubscribedOnTaskLoaded(Type owner, string methodName)
        {
            var action = Task.OnTaskLoaded;
            if( action is null )
                return false;

            var invocations = action.GetInvocationList();
            if( invocations is null )
                return false;

            var length = invocations.Length;
            foreach( var invocation in invocations )
            {
                var method = invocation.Method;
                var ownerType = method.DeclaringType;
                if( ownerType.Equals(owner) && method.Name.Equals( methodName ) )
                    return true;
            }

            return false;
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

        private static void RefreshFocusedScene()
        {
            if( IsSceneDicoEmpty() )
            {
                SetFocus( null );
                return;
            }

            _orderedKeys = _dicoOfTasks.Keys.OrderByDescending(task => task);

            var task = _orderedKeys.First();

            if( m_focusTask )
            {
                var focusScene = GetLoadedScene( m_focusTask ).Scene;
                if( focusScene.IsValid() && task.m_priority <= m_focusTask.m_priority )
                    return;
            }

            SetFocus( task );
        }

        #endregion


        #region Callbacks

        private static Action<AsyncOperationHandle<SceneInstance>> SceneUnloadComplete( TaskData task = null )
        {
            RefreshFocusedScene();

            if( task ) ULoadTask( null, task );

            return null;
        }

        private static void OnSceneLoaded( AsyncOperationHandle<SceneInstance> go )
        {
            var taskData = GetTaskDataOf(go);
            var taskManager = GetTaskManagerOf(go);

            taskManager.Priority = taskData.m_priority;
            RefreshFocusedScene();
            OnTaskLoaded?.Invoke( taskData );
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
            if( !scene.IsValid() )
                return default;
            var rootGameObjects = new List<GameObject>();
            scene.GetRootGameObjects(rootGameObjects);

            var taskManagerRoot = rootGameObjects.Find(SearchTaskManager);
            var taskManager = taskManagerRoot.GetComponentInChildren<TaskManager>(true);

            return taskManager;
        }

        private static bool SearchTaskManager( GameObject target )
        {
            if( target.name.Contains( "TaskManager" ) )
                return true;

            var transform = target.transform;
            var childCount = transform.childCount;

            for( var i = 0; i < childCount; i++ )
            {
                var child = transform.GetChild(i);

                if( child.name.Contains( "TaskManager" ) )
                    return true;
            }

            return false;
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
        private static TaskPriority _focusPriority;

        #endregion
    }
}
