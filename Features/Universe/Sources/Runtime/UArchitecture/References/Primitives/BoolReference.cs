using UnityEngine;

namespace Universe
{
    [System.Serializable]
    public sealed class BoolReference : ReferenceBase<bool, BoolFact>
    {
        public BoolReference() : base() { }
        public BoolReference(Object value) : base(value) { }
    }
}