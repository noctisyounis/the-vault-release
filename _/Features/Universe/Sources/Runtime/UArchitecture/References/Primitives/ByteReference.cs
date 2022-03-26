using UnityEngine;

namespace Universe
{
    [System.Serializable]
    public sealed class ByteReference : ReferenceBase<byte, ByteFact>
    {
        public ByteReference() : base() { }
        public ByteReference(byte value) : base(value) { }
    }
}