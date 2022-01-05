namespace Universe
{
    public partial interface IGameSignalReceptor<T>
    {
        #region Methods
        
        void OnEventRaised(T value);
        
        #endregion
    }
}