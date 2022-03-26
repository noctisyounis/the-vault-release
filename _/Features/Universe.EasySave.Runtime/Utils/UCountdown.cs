using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Universe
{
    public class UCountdown : UBehaviour
    {
        #region Public

        public float m_timeBeforeEvent;
        public FloatFact m_timeBeforeEventVariable;
        public bool m_isRepeated;
        
        public enum TimeMode { Normal, Unscaled };
        [EnumToggleButtons]
        public TimeMode m_timeMode;
        public UnityEvent OnCountdownReached;

        [Header( "Internal" ), Space( 10 )]
        [SerializeField]
        public bool m_isConsumed;

        #endregion
        
        
        #region Properties

        private float GetTimeFromTimeMode() => 
            m_timeMode == TimeMode.Normal ? UTime.Time : UTime.UnscaledTime;
        
        #endregion


        #region Unity API

        public override void Awake()
        {
            base.Awake();
            if (!enabled) return;
            
            Initialize();
        }

        private void OnEnable()
        {
            Initialize();
        }

        private void OnDisable()
        {
            _started = false;
        }

        public override void OnUpdate(float deltaTime)
        {
            if (!_started) return;
            
            var timeBeforeEvent = m_timeBeforeEvent;
            if (m_timeBeforeEventVariable != null)
            {
                timeBeforeEvent = m_timeBeforeEventVariable.Value;
            }
            
            var timeMode = GetTimeFromTimeMode();
            Tick(timeMode, timeBeforeEvent);
        }

        #endregion


        #region Utils

        private void Initialize()
        {
            _started = true;
            _startTime = GetTimeFromTimeMode();
            
            Verbose( $"Countdown.Initialize _startTIme = {_startTime}" );
        }

        private void Tick(float time, float timeBeforeEvent)
        {
            if (!(time > _startTime + timeBeforeEvent) || m_isConsumed) return;
                
            Verbose( $"Countdown.Update m_isRepeated = {m_isRepeated}, current = {time}, Invoke event" );
            OnCountdownReached.Invoke();
                
            if( m_isRepeated )
            {
                Initialize();
                return;
            }

            m_isConsumed = true;
        }

        #endregion


        #region Private

        private float _startTime;
        private bool _started = false;

        #endregion
    }
}