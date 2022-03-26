using UnityEngine;

namespace Universe
{
    [System.Serializable]
    public sealed class Vector2Reference : ReferenceBase<Vector2, Vector2Fact>
    {
        public Vector2Reference() : base() { }
        public Vector2Reference(Vector2 value) : base(value) { }
    }
}