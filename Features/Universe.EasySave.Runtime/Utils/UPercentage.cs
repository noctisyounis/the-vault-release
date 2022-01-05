namespace Universe
{
    public class UPercentage
    {
        #region Main

        public UPercentage(int max = 0)
        {
            InitializeCompletionPercentage( max );
        }

        public void InitializeCompletionPercentage( int count )
        {
            _totalCompletionCount = count;
            _count = 0;
        }

        public float GetNormalizedPercentage()
        {
            return (float)_count / _totalCompletionCount;
        }

        public float GetPercentage()
        {
            return GetNormalizedPercentage() * 100;
        }

        public void IncreasePercentage()
        {
            _count++;
        }

        #endregion


        #region Private

        private int _count;
        private int _totalCompletionCount;

        #endregion
    }
}