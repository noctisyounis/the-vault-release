namespace Universe
{
    [System.Serializable]
    public sealed class ULongReference : ReferenceBase<ulong, ULongFact>
    {
        public ULongReference() : base() { }
        public ULongReference(ulong value) : base(value) { }
    }
}