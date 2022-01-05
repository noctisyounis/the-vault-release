namespace Universe
{
    [System.Serializable]
    public sealed class IntReference : ReferenceBase<int, IntFact>
    {
        public IntReference() : base() { }
        public IntReference(int value) : base(value) { }
    }
}