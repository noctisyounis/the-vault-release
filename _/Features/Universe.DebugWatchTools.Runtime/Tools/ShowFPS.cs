using System;
using Universe.DebugWatch.Runtime;

namespace Universe.DebugWatchTools.Runtime
{
    public class ShowFPS : UBehaviour
    {
        #region Public Members

        public GameObjectFact m_debugArrayManager;
        public string m_entryName;

        #endregion


        #region Unity API

        public override void Awake() => OnDisplayChanged += UpdateDisplay;
        public override void OnDestroy() => OnDisplayChanged -= UpdateDisplay;

        #endregion


        #region Main

        public void UpdateDisplay()
        {
            GetDebugArrayManager(out var debugArrayManager);
            if(!debugArrayManager) return;

            if(s_display)
            {
                debugArrayManager.AddEntry(m_entryName, () => 
                {
                    var fps = (int)(1f / UTime.UnscaledDeltaTime);

                    return $"{fps}";
                });

                return;
            }
            
            debugArrayManager.RemoveEntry(m_entryName);
        }

        public static void ToggleDisplay()
        {
            s_display = !s_display;
            OnDisplayChanged?.Invoke();
        }

        #endregion


        #region Utils

        private void GetDebugArrayManager(out DebugArrayManager debugArrayManager)
        {
            debugArrayManager = null;
            var debugArrayObject = m_debugArrayManager.Value;
            if(!debugArrayObject) return;

            debugArrayManager = debugArrayObject.GetComponent<DebugArrayManager>();
        }

        #endregion


        #region Private and Protected

        private static event Action OnDisplayChanged;
        private static bool s_display;

        #endregion
    }
}