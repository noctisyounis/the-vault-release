using System;
using System.Collections.Generic;
using UnityEngine;
using Universe.SceneTask.Runtime;

using static UnityEngine.SceneManagement.SceneManager;
using static Universe.UTime;

namespace Universe
{
    public class TaskManager : UBehaviour
    {
        #region Exposed

        [Header("Settings")] 
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
            
        #endregion
        
        
        #region Unity API

        public override void Awake()
        {
            base.Awake();
            Task.Register(this);
            InitializeUTrackedAliases();
        }

        public void Update()
        {
            if (gameObject.scene.name != Task.GetFocusSceneName()) return;
            foreach (var u in _registeredUpdate)
            {
                u.OnUpdate(DeltaTime);
            }
        }

        public void FixedUpdate()
        {
            if (gameObject.scene.name != Task.GetFocusSceneName()) return;
            foreach (var u in _registeredFixedUpdate)
            {
                u.OnFixedUpdate(FixedDeltaTime);
            }
        }

        public void LateUpdate()
        {
            if (gameObject.scene.name != Task.GetFocusSceneName()) return;
            foreach (var u in _registeredLateUpdate)
            {
                u.OnLateUpdate(DeltaTime);
            }
        }
        
        public override void OnDestroy()
        {
            base.OnDestroy();
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
        
        public void AddToUpdate(UBehaviour target) => 
            SafeAddTargetToList(target, _registeredUpdate);
        
        public void AddToFixedUpdate(UBehaviour target) => 
            SafeAddTargetToList(target, _registeredFixedUpdate);
        
        public void AddToLateUpdate(UBehaviour target) =>
            SafeAddTargetToList(target, _registeredLateUpdate);

        public void RemoveFromLateUpdate(UBehaviour target) =>
            SafeRemoveTargetFromList(target, _registeredLateUpdate);
        
        public void RemoveFromFixedUpdate(UBehaviour target) =>
            SafeRemoveTargetFromList(target, _registeredFixedUpdate);

        public void RemoveFromUpdate(UBehaviour target) =>
            SafeRemoveTargetFromList(target, _registeredUpdate);


        private void SafeAddTargetToList(UBehaviour target, List<UBehaviour> list)
        {
            var alreadyExist = list.Contains(target);
            if (alreadyExist) return;
            
            list.Add(target);
        }
        
        private void SafeRemoveTargetFromList(UBehaviour target, List<UBehaviour> list)
        {
            var alreadyExist = list.Contains(target);
            if (!alreadyExist) return;

            list.Remove(target);
        }
        
        #endregion


        #region Private And Protected
        
        private UTrackedAlias _headsetTrackedAlias;
        private UTrackedAlias _playAreaTrackedAlias;
        private UTrackedAlias _leftControllerTrackedAlias;
        private UTrackedAlias _rightControllerTrackedAlias;

        private List<UBehaviour> _registeredUpdate = new();
        private List<UBehaviour> _registeredFixedUpdate = new();
        private List<UBehaviour> _registeredLateUpdate = new();

        #endregion
    }
}