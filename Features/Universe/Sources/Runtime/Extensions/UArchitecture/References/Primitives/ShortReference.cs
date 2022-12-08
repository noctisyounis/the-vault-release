namespace Universe
{
    [System.Serializable]
    public sealed class ShortReference : ReferenceBase<short, ShortFact>
    {
        public ShortReference() : base() { }
        public ShortReference(short value) : base(value) { }
    }
}