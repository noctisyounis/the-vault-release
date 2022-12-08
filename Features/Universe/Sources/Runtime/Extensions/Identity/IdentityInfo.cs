using System;
using UnityEngine;


namespace Universe
{
    public struct IdentityInfo
    {
        #region Public
        
        public GameObject m_go;
        public event Action<GameObject> OnAdd;
        public event Action OnRemove;

        #endregion
        
        
        #region Constructor
        
        public IdentityInfo(Identity comp)
        {
            m_go = comp.gameObject;
            OnRemove = null;
            OnAdd = null;
        }
        
        #endregion
        
        
        #region Main API 

        public void HandleAddCallback()
        {
            if (OnAdd != null) OnAdd(m_go);
        }

        public void HandleRemoveCallback()
        {
            if (OnRemove != null) OnRemove();
        }
        
        #endregion
    }
}