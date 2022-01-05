namespace Universe
{
    [System.Serializable]
    public sealed class FloatReference : ReferenceBase<float, FloatFact>
    {
        public FloatReference() : base() { }
        public FloatReference(float value) : base(value) { }
    }
}