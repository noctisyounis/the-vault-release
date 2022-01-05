namespace Universe
{
    [System.Serializable]
    public sealed class StringReference : ReferenceBase<string, StringFact>
    {
        public StringReference() : base() { }
        public StringReference(string value) : base(value) { }
    }
}