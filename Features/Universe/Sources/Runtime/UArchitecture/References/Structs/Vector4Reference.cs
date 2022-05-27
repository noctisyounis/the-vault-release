using UnityEngine;

namespace Universe
{
    [System.Serializable]
    public sealed class Vector4Reference : ReferenceBase<Vector4, Vector4Fact>
    {
        public Vector4Reference() : base() { }
        public Vector4Reference(Vector4 value) : base(value) { }
    }
}