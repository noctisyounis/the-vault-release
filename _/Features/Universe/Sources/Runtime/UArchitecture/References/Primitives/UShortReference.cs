namespace Universe
{
    [System.Serializable]
    public sealed class UShortReference : ReferenceBase<ushort, UShortFact>
    {
        public UShortReference() : base() { }
        public UShortReference(ushort value) : base(value) { }
    }
}