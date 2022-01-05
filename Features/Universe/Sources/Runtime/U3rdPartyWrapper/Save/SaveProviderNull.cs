namespace Universe
{
    public class SaveProviderNull : ISaveProvider
    {
        public override void Load(USaveLevel saveLevel)
        {
            throw new System.NotImplementedException();
        }

        public override void LoadAll()
        {
            throw new System.NotImplementedException();
        }

        public override void Save(USaveLevel saveLevel)
        {
            throw new System.NotImplementedException();
        }

        public override void SaveAll()
        {
            throw new System.NotImplementedException();
        }
    }
}