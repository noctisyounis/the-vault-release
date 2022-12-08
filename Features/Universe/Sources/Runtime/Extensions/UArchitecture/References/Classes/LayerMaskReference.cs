using UnityEngine;

namespace Universe
{
    [System.Serializable]
    public sealed class LayerMaskReference : ReferenceBase<LayerMask, LayerMaskFact>
    {
        public LayerMaskReference() : base() { }
        public LayerMaskReference(LayerMask value) : base(value) { }
    }
}