namespace Universe
{
    [System.Serializable]
    public sealed class LongReference : ReferenceBase<long, LongFact>
    {
        public LongReference() : base() { }
        public LongReference(long value) : base(value) { }
    }
}