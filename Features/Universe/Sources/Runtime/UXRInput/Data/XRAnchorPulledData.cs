using UnityEngine;

namespace Universe
{
    public class XRAnchorPulledData
    {
        #region Public

        public Transform m_sceneObject;
        
        public Vector3 m_lastPosition;

        public Vector3 GetVelocity(float deltaTime) => (m_sceneObject.position - m_lastPosition) / deltaTime;
        
        #endregion
        
        
        #region Constructor

        public XRAnchorPulledData(Transform tr)
        {
            m_sceneObject = tr;
            m_lastPosition = new Vector3();
        }
        
        #endregion
    }
}