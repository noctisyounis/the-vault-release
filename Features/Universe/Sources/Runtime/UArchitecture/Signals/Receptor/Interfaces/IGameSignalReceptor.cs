namespace Universe
{
    public partial interface IGameSignalReceptor<T>
    {
        #region Methods
        
        void OnSignalEmitted(T value);
        
        #endregion
    }

    public interface IGameSignalReceptor
    {
        #region Methods
        
        void OnSignalEmitted();

        #endregion
    }
}