using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using static UnityEngine.InputSystem.InputActionPhase;

namespace Universe.DebugWatch.Runtime
{
    public class DebugWatchInputManager : UBehaviour
    {
        #region Public

        [Header("Settings")]
        [Header("Open Action Bindings")]
        public InputAction m_openAction;
        [Header("Close Action Bindings")]
        public InputAction m_closeAction;

        [Header("Events")]
        public UnityEvent OnTripleClick;
        public UnityEvent OnHideDebugMenu;

        #endregion


        #region Unity API

        public override void Awake()
        {
            base.Awake();

            m_openAction.Enable();
            m_closeAction.Enable();
        }

        public override void OnUpdate(float deltaTime)
        {
            if (!Debug.isDebugBuild) return;
            if(IsCloseKeyDown)  TryHideDebugMenu();
            if(IsOpenKeyDown)   TryShowDebugMenuOnClick();

            UpdatePreviousPhases();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            m_openAction.Dispose();
            m_closeAction.Dispose();
        }

        #endregion


        #region Main

        public void TryShowDebugMenuOnClick()
        {
            if (CanReset) _clickCount = 0;

            _clickCount++;
            if (IsThirdClick)
            {
                OnTripleClick.Invoke();
                _clickCount = 0;
            }

            _lastPressTime = Time.time;
        }

        public void TryHideDebugMenu()
        {
            OnHideDebugMenu.Invoke();
        }

        #endregion


        #region Utils

        private void UpdatePreviousPhases()
        {
            _previousOpenPhase = m_openAction.phase;
            _previousClosePhase = m_closeAction.phase;
        }

        #endregion


        #region Private Properties

        private bool CanReset => 
            ((Time.time - _lastPressTime) > _buttonPressSpeed);

        private bool IsOpenKeyDown =>
            IsPreviousOpenWaiting && IsOpenPerformed;

        private bool IsOpenPerformed =>
            m_openAction.phase == Performed;

        private bool IsPreviousOpenWaiting =>
            _previousOpenPhase == Waiting;

        private bool IsCloseKeyDown =>
            IsPreviousCloseWaiting && IsClosePerformed;

        private bool IsClosePerformed =>
            m_closeAction.phase == Performed;

        private bool IsPreviousCloseWaiting =>
            _previousClosePhase == Waiting;

        private bool IsThirdClick => 
            (_clickCount >= 3);

        #endregion


        #region Privates Members

        private int _clickCount = 0;
        private float _buttonPressSpeed = 0.4f;
        private float _lastPressTime = -10f;
        private InputActionPhase _previousOpenPhase;
        private InputActionPhase _previousClosePhase;

        #endregion
    }
}