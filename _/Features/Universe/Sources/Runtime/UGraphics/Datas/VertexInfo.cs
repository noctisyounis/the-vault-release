using System;
using UnityEngine;

using static UnityEngine.Vector3;

namespace Universe
{
    [Serializable]
    public class VertexInfo
    {
        #region Public

        public Vector3 m_position = zero;
        public Vector3 m_rotation = zero;
        public Vector3 m_scale = one;

        #endregion
    }
}