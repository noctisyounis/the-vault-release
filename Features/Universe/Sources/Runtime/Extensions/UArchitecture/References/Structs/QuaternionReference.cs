using UnityEngine;

namespace Universe
{
    [System.Serializable]
    public sealed class QuaternionReference : ReferenceBase<Quaternion, QuaternionFact>
    {
        public QuaternionReference() : base() { }
        public QuaternionReference(Quaternion value) : base(value) { }
    }
}