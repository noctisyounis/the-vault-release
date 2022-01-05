namespace Universe
{
    [System.Serializable]
    public sealed class DoubleReference : ReferenceBase<double, DoubleFact>
    {
        public DoubleReference() : base() { }
        public DoubleReference(double value) : base(value) { }
    }
}