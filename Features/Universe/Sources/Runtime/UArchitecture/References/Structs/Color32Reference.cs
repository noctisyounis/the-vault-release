using UnityEngine;

namespace Universe
{
    [System.Serializable]
    public sealed class Color32Reference : ReferenceBase<Color32, Color32Fact>
    {
        public Color32Reference() : base() { }
        public Color32Reference(Color32 value) : base(value) { }
    }
}