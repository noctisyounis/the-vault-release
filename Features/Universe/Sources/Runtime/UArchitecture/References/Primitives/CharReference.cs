using UnityEngine;

namespace Universe
{
    [System.Serializable]
    public sealed class CharReference : ReferenceBase<char, CharFact>
    {
        public CharReference() : base() { }
        public CharReference(char value) : base(value) { }
    }
}