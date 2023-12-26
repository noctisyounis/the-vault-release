using System;
using UnityEngine;
using Universe.SceneTask;
using Universe.SceneTask.Runtime;
using CSharpExtensions.Runtime;

using static UnityEngine.SceneManagement.SceneManager;
using static Universe.UTime;

namespace Universe
{
    [DefaultExecutionOrder(-50)]
    public class TaskManager : UBehaviour
    {
        #region Exposed

        [Header("Settings")] 

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
            
            if (localUpdate.m_enableOverride)
                _finalUpdateCount = localUpdate.m_countOverride;

            if (!localUpdate.m_enableOverride)
            {
                if (!_updatesCapacity)
                {
                    Debug.LogError("[VAULT::TaskManager] You need to reference in taskdata a UpdateCapacityData", this);
                    return;
                }

                _finalUpdateCount = _updatesCapacity.m_defaultQuantity;
            }

            _registeredUpdate = new EfficientList<UBehaviour>(_finalUpdateCount);
            _registeredFixedUpdate = new EfficientList<UBehaviour>(_finalUpdateCount);
            _registeredLateUpdate = new EfficientList<UBehaviour>(_finalUpdateCount);
        }

        public void Update()
        {
            if (!CanUpdate) return;
            var deltaTime = DeltaTime;
            UBehaviour currentUBehaviour;
            
            for (var i = 0; i < _finalUpdateCount; i++)
            {
                currentUBehaviour = _registeredUpdate[i];
                if(currentUBehaviour is null || !currentUBehaviour.UseUpdates) continue;

                currentUBehaviour.OnUpdate(deltaTime);
            }
        }

        public void FixedUpdate()
        {
            SetActiveTaskInput(IsFocused);
            if (!CanUpdate) return;
            var fixedDeltaTime = FixedDeltaTime;
            UBehaviour currentUBehaviour;

            for (var i = 0; i < _finalUpdateCount; i++)
            {
                currentUBehaviour = _registeredFixedUpdate[i];
                if(currentUBehaviour is null || !currentUBehaviour.UseUpdates) continue;
                
                currentUBehaviour.OnFixedUpdate(fixedDeltaTime);
            }
        }

        public void LateUpdate()
        {
            if (!CanUpdate) return;
            var deltaTime = DeltaTime;
            UBehaviour currentUBehaviour;
            
            for (var i = 0; i < _finalUpdateCount; i++)
            {
                currentUBehaviour = _registeredUpdate[i];
                if(currentUBehaviour is null || !currentUBehaviour.UseUpdates) continue;
                
                currentUBehaviour.OnLateUpdate(deltaTime);
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
        
        public void AddToUpdate(UBehaviour target) => _registeredUpdate.Add(target); 
        
        public void AddToFixedUpdate(UBehaviour target) => _registeredFixedUpdate.Add(target);
                
        public void AddToLateUpdate(UBehaviour target) => _registeredLateUpdate.Add(target);

        public void RemoveFromUpdate(UBehaviour target) => _registeredUpdate.Remove(target);

        public void RemoveFromLateUpdate(UBehaviour target) => _registeredLateUpdate.Remove(target);
            
        public void RemoveFromFixedUpdate(UBehaviour target) => _registeredFixedUpdate.Remove(target);
        
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

        private EfficientList<UBehaviour> _registeredUpdate;
        private EfficientList<UBehaviour> _registeredFixedUpdate;
        private EfficientList<UBehaviour> _registeredLateUpdate;

        public UpdatesCapacityData _updatesCapacity;
        public LocalUpdate localUpdate;

        private int _finalUpdateCount;
        
        [Serializable]
        public struct LocalUpdate
        {
            public bool m_enableOverride;
            public int m_countOverride;
        }

        #endregion
    }
}