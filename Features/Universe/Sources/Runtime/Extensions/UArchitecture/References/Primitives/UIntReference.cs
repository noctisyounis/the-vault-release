namespace Universe
{
    [System.Serializable]
    public sealed class UIntReference : ReferenceBase<uint, UIntFact>
    {
        public UIntReference() : base() { }
        public UIntReference(uint value) : base(value) { }
    }
}