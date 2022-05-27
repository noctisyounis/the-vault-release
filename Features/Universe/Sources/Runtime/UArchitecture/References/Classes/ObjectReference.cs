using UnityEngine;

namespace Universe
{
    [System.Serializable]
    public sealed class ObjectReference : ReferenceBase<Object, ObjectFact>
    {
        public ObjectReference() : base() { }
        public ObjectReference(Object value) : base(value) { }
    }
}