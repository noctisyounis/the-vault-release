using UnityEngine;

namespace Universe
{
    [System.Serializable]
    public sealed class ColorReference : ReferenceBase<Color, ColorFact>
    {
        public ColorReference() : base() { }
        public ColorReference(Color value) : base(value) { }
    }
}