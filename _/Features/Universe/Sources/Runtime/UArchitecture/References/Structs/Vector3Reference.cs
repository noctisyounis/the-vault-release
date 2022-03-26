using UnityEngine;

namespace Universe
{
    [System.Serializable]
    public sealed class Vector3Reference : ReferenceBase<Vector3, Vector3Fact>
    {
        public Vector3Reference() : base() { }
        public Vector3Reference(Vector3 value) : base(value) { }
    }
}