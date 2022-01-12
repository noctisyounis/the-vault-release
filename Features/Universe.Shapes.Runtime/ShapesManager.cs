using System;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

using static Shapes.Draw;

namespace Universe.ShapesProvider.Runtime
{
    public class ShapesManager : MonoBehaviour
    {
        #region Public

        public Camera m_camera;

        public Camera Camera
        {
            get
            {
                if (m_camera) return m_camera;

                m_camera = Camera.main;
                return m_camera;
            }
            
            private set => m_camera = value;
        }
        
        #endregion
        
        
        #region Public API

        public void AddGizmos(Action gizmos)
        {
            if (_gizmos.Contains(gizmos)) return;
            
            _gizmos.Add(gizmos);
        }
        
        #endregion
        
        
        #region Unity API
        
        private void Update()
        {
            using (Command(Camera))
            {
                for (var i = _gizmos.Count - 1; i >= 0; i--)
                {
                    var currentGizmos = _gizmos[i];
                    if (currentGizmos == null) continue;
                    
                    currentGizmos.Invoke();
                    _gizmos.Remove(currentGizmos);
                }
            }
        }

        private void OnGUI()
        {
            GUILayout.Button(_gizmos.Count.ToString());
        }
        
        #endregion
        
        
        #region Private Members

        private List<Action> _gizmos = new();

        #endregion
    }
}