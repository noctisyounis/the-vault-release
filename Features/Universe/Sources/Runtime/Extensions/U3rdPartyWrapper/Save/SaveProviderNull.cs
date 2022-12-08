using System.Collections.Generic;

namespace Universe
{
    public class SaveProviderNull : ISaveProvider
    {
        public override void Load(USaveLevel saveLevel)
        {
            throw new System.NotImplementedException();
        }

        public override void Load( List<FactBase> facts )
        {
            throw new System.NotImplementedException();
        }

        public override void Load( FactBase fact )
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

        public override void Save( List<FactBase> facts )
        {
            throw new System.NotImplementedException();
        }

        public override void Save( FactBase fact )
        {
            throw new System.NotImplementedException();
        }

        public override void SaveAll()
        {
            throw new System.NotImplementedException();
        }
    }
}