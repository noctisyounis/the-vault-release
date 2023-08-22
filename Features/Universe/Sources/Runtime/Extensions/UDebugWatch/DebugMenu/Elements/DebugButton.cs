using TMPro;
using UnityEngine;

using static Universe.DebugWatch.Runtime.DebugMenuRoot;

namespace Universe.DebugWatch.Runtime
{
    public class DebugButton : DebugElement, IClickable, ICancelable
    {
        #region Public Members

        [Header("References")]
        public GameObject   m_arrowHover;
        public TMP_Text m_label;
        
        [Header("Parameters")] 
        public Color m_disabled = Color.gray;
        public Color m_selected = Color.white;
        public Color m_active = Color.white;
        public Color m_inactive = Color.white;
        
        #endregion


        #region Unity API

        public override void Awake()
        {
            OnDeselected();
            FetchStatus(false);
        }
        public override void OnDisable() => OnDeselected();

        #endregion


        #region Main

        public void OnClick()
        {
            var result = s_instance.Execute<object>(m_path);
            if (result is not bool active)
            {
                SetLabelColor(m_selected);
                return;
            }
            
            var color = active ? m_active : m_inactive;
            
            SetLabelColor(color);
        }
        
        public void OnCancel() => m_owner.ReturnToParent();

        #endregion


        #region Utils

        public override void OnSelected()
        {
            FetchStatus(true);
            DisplayArrow();
            DisplayTooltip();
        }

        public override void OnDeselected()
        {
            FetchStatus(false);
            HideArrow();
        }

        private void FetchStatus(bool selected)
        {
            var endColor = selected ? m_selected : m_disabled;
            var result = s_instance.GetLastResult(m_path);
            
            if (result is not bool active)
            {
                SetLabelColor(endColor);
                return;
            }
            
            var color = active ? m_active : m_inactive;
            endColor *= color;
            
            SetLabelColor(endColor);
        }

        private void SetLabelColor(Color next)
        {
            if (m_label == null) return;

            m_label.color = next;
        }
        
        
        private void DisplayArrow() => m_arrowHover.gameObject.SetActive(true);
        private void HideArrow() => m_arrowHover.gameObject.SetActive(false);
        private void DisplayTooltip() => s_instance.DisplayTooltip( m_path );

        #endregion
    }
}