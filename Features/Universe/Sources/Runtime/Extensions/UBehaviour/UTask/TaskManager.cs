using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Universe.SceneTask;
using Universe.SceneTask.Runtime;

using static UnityEngine.SceneManagement.SceneManager;
using static Universe.UTime;

namespace Universe
{
    [DefaultExecutionOrder(-50)]
    public class TaskManager : UBehaviour
    {
        #region Exposed

        [Header("Settings")] public ScriptableDebugSizeOfTaskData m_debugUpdateCount;
        public GameObject m_inputManager;
        public XR m_xr;
        
        [Serializable]
        public struct XR
        {
            public bool m_useXRInput;
            [Space(10)]

            public Transform m_headset;
            public Transform m_playArea;
            public Transform m_dominantHand;
            public Transform m_nonDominantHand;    
        }

        public TaskPriority Priority
        {
            get => _priority;
            set => _priority = value;
        }
            
        #endregion
        
        
        #region Events

        public delegate void OnDestroyedHandler();
        public event OnDestroyedHandler OnDestroyed;
        
        #endregion
        
        
        #region Unity API

        public override void Awake()
        {
            Task.Register(this);
            InitializeUTrackedAliases();
        }

        public void Update()
        {
            if (!CanUpdate) return;
            
            ApplyUnregistingUpdate();
            
            var deltaTime = DeltaTime;

            for (var i = 0; i < _registeredUpdateAmount; i++)
            {
                var u = _registeredUpdate[i];
                if(!u.UseUpdates) continue;

                u.OnUpdate(deltaTime);
            }
        }

        public void FixedUpdate()
        {
            SetActiveTaskInput(IsFocused);
            if (!CanUpdate) return;
            
            ApplyUnregistingFixedUpdate();

            var fixedDeltaTime = FixedDeltaTime;

            for (var i = 0; i < _registeredFixedUpdateAmount; i++)
            {
                var u = _registeredFixedUpdate[i];
                if(!u.UseUpdates) continue;
                
                u.OnFixedUpdate(fixedDeltaTime);
            }
        }

        public void LateUpdate()
        {
            if (!CanUpdate) return;
            
            ApplyUnregistingLateUpdate();
            
            var deltaTime = DeltaTime;
            
            for (var i = 0; i < _registeredLateUpdateAmount; i++)
            {
                var u = _registeredLateUpdate[i];
                if(!u.UseUpdates) continue;
                
                u.OnLateUpdate(deltaTime);
            }
        }
        
        public override void OnDestroy()
        {
            OnDestroyed?.Invoke();
            Task.Unregister(this);
        }

        #endregion
        
        
        #region XR API
        
        public void SetHeadset(XRAnchorPulledData data) =>
            SetXRAliasFrom(data, ref m_xr.m_headset, ref _headsetTrackedAlias);

        public void SetPlayArea(XRAnchorPulledData data) =>
            SetXRAliasFrom(data, ref m_xr.m_playArea, ref _playAreaTrackedAlias);

        public void SetLeftController(XRAnchorPulledData data) =>
            SetXRAliasFrom(data, ref m_xr.m_dominantHand, ref _leftControllerTrackedAlias);

        public void SetRightController(XRAnchorPulledData data) =>
            SetXRAliasFrom(data, ref m_xr.m_nonDominantHand, ref _rightControllerTrackedAlias);
        
        #endregion

        public void SetXRAliasFrom(XRAnchorPulledData data, ref Transform transformAlias, ref UTrackedAlias trackedAlias)
        {
            if (!m_xr.m_useXRInput) return;
            
            if (transformAlias == null)
            {
                Verbose($"You are missing a reference in the task: {GetActiveScene().name}");
                return;
            }

            var sceneObject = data.m_sceneObject;
            var position = sceneObject.position;
            
            transformAlias.position = position;
            transformAlias.rotation = sceneObject.rotation;
            trackedAlias.Velocity = data.GetVelocity(DeltaTime);
            trackedAlias.Magnitude = position.magnitude;
        }
        
        private void InitializeUTrackedAliases()
        {
            if (!m_xr.m_useXRInput) return;
            
            _headsetTrackedAlias = m_xr.m_headset.GetComponent<UTrackedAlias>();
            _playAreaTrackedAlias = m_xr.m_playArea.GetComponent<UTrackedAlias>();
            _leftControllerTrackedAlias = m_xr.m_dominantHand.GetComponent<UTrackedAlias>();
            _rightControllerTrackedAlias = m_xr.m_nonDominantHand.GetComponent<UTrackedAlias>();
        }
        
        #region Update Management
        
        public void AddToUpdate(UBehaviour target)
        {
            SafeAddTargetToHashSet(target, _registeredUpdateSet);
            SafeRemoveTargetFromList(target, _unregisteringUpdate);
            _registeredUpdateSet.IntoArray(ref _registeredUpdate);
            _registeredUpdateAmount = _registeredUpdate.Length;
            m_debugUpdateCount.m_sizeOfEverything.UpdateSizeOfUpdate(_registeredUpdateAmount);
        }

        public void AddToFixedUpdate(UBehaviour target)
        {
            SafeAddTargetToHashSet(target, _registeredFixedUpdateSet);
            SafeRemoveTargetFromList(target, _unregisteringFixedUpdate);
            _registeredFixedUpdateSet.IntoArray(ref _registeredFixedUpdate);
            _registeredFixedUpdateAmount = _registeredFixedUpdate.Length;
            m_debugUpdateCount.m_sizeOfEverything.UpdateOfFixedUpdate(_registeredFixedUpdateAmount);

        }
        
        public void AddToLateUpdate(UBehaviour target)
        {
            SafeAddTargetToHashSet(target, _registeredLateUpdateSet);
            SafeRemoveTargetFromList(target, _unregisteringLateUpdate);
            _registeredLateUpdateSet.IntoArray(ref _registeredLateUpdate);
            _registeredLateUpdateAmount = _registeredLateUpdate.Length;
            m_debugUpdateCount.m_sizeOfEverything.UpdateOfLateUpdate(_registeredLateUpdateAmount);

        }

        public void RemoveFromLateUpdate(UBehaviour target) =>
            SafeAddTargetToList(target, _unregisteringLateUpdate);
        
        public void RemoveFromFixedUpdate(UBehaviour target) =>
            SafeAddTargetToList(target, _unregisteringFixedUpdate);

        public void RemoveFromUpdate(UBehaviour target) =>
            SafeAddTargetToList(target, _unregisteringUpdate);

        private void SafeAddTargetToList(UBehaviour target, List<UBehaviour> list)
        {
            var alreadyExist = list.Contains(target);
            if (alreadyExist) return;
            
            list.Add(target);
        }
        
        private void SafeAddTargetToHashSet(UBehaviour target, HashSet<UBehaviour> list)
        {
            list.Add(target);
        }
        
        private void SafeRemoveTargetFromList(UBehaviour target, List<UBehaviour> list)
        {
            var alreadyExist = list.Contains(target);
            if (!alreadyExist) return;

            list.Remove(target);
        }
        
        private void SafeRemoveTargetFromHashSet(UBehaviour target, HashSet<UBehaviour> set)
        {
            set.Remove(target);
        }

        private void SafeRemoveTargetsFromHashSet(List<UBehaviour> targets, HashSet<UBehaviour> set)
        {
            set.ExceptWith(targets);
        }

        private void ApplyUnregistingUpdate()
        {
            if (_unregisteringUpdate.Count == 0) return;
            
            SafeRemoveTargetsFromHashSet(_unregisteringUpdate, _registeredUpdateSet);
            
            _unregisteringUpdate.Clear();
            _registeredUpdateSet.IntoArray(ref _registeredUpdate);
            _registeredUpdateAmount = _registeredUpdate.Length;
        }
        
        private void ApplyUnregistingFixedUpdate()
        {
            if (_unregisteringFixedUpdate.Count == 0) return;
            
            SafeRemoveTargetsFromHashSet(_unregisteringFixedUpdate, _registeredFixedUpdateSet);
            
            _unregisteringFixedUpdate.Clear();
            _registeredFixedUpdateSet.IntoArray(ref _registeredFixedUpdate);
            _registeredFixedUpdateAmount = _registeredFixedUpdate.Length;
        }
        
        private void ApplyUnregistingLateUpdate()
        {
            if (_unregisteringLateUpdate.Count == 0) return;
            
            SafeRemoveTargetsFromHashSet(_unregisteringLateUpdate, _registeredLateUpdateSet);
            
            _unregisteringLateUpdate.Clear();
            _registeredLateUpdateSet.IntoArray(ref _registeredLateUpdate);
            _registeredLateUpdateAmount = _registeredLateUpdate.Length;
        }

        public bool IsAlwaysUpdated() => Priority == TaskPriority.ALWAYS_UPDATE;

        private bool CanUpdate => IsAlwaysUpdated() || IsFocused;
        private bool IsFocused => Priority == Task.GetFocusPriority();

        #endregion


        #region Input Management

        public void SetActiveTaskInput(bool next)
        {
            if(next)    EnableTaskInputs();
            else        DisableTaskInputs();
        }

        public void DisableTaskInputs()
        {
            if(!m_inputManager) return;
            m_inputManager.SetActive(false);
        }

        public void EnableTaskInputs()
        {
            if(!m_inputManager) return;
            m_inputManager.SetActive(true);
        }

        #endregion


        #region Private And Protected

        private TaskPriority _priority;
        
        private UTrackedAlias _headsetTrackedAlias;
        private UTrackedAlias _playAreaTrackedAlias;
        private UTrackedAlias _leftControllerTrackedAlias;
        private UTrackedAlias _rightControllerTrackedAlias;

        private HashSet<UBehaviour> _registeredUpdateSet = new();
        private HashSet<UBehaviour> _registeredFixedUpdateSet = new();
        private HashSet<UBehaviour> _registeredLateUpdateSet = new();
        private UBehaviour[] _registeredUpdate;
        private UBehaviour[] _registeredFixedUpdate;
        private UBehaviour[] _registeredLateUpdate;
        private int _registeredUpdateAmount;
        private int _registeredFixedUpdateAmount;
        private int _registeredLateUpdateAmount;
        private List<UBehaviour> _unregisteringUpdate = new();
        private List<UBehaviour> _unregisteringFixedUpdate = new();
        private List<UBehaviour> _unregisteringLateUpdate = new();

        #endregion
    }
}