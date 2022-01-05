namespace Universe
{
    [System.Serializable]
    public sealed class SByteReference : ReferenceBase<sbyte, SByteFact>
    {
        public SByteReference() : base() { }
        public SByteReference(sbyte value) : base(value) { }
    }
}