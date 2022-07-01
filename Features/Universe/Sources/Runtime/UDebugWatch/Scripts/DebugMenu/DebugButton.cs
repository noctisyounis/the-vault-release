using UnityEngine;
using UnityEngine.UI;

using static Universe.DebugWatch.Runtime.DebugMenuRoot;

namespace Universe.DebugWatch.Runtime
{
    public class DebugButton : MonoBehaviour
    {
        #region Public Members

        public DebugPanel   m_owner;
        public string       m_path;
        public Image        m_arrowHover;

        #endregion


        #region Unity API

        private void Awake() => HideArrow();
        private void OnDisable() => HideArrow();

        #endregion


        #region Main

        public void OnClick() => s_instance.Execute(m_path);
        public void OnCancel() => m_owner.ReturnToParent();

        #endregion


        #region Utils

        public void DisplayArrow() => m_arrowHover.gameObject.SetActive(true);
        public void HideArrow() => m_arrowHover.gameObject.SetActive(false);
        public void DisplayTooltip() => s_instance.DisplayTooltip( m_path );

        #endregion
    }
}