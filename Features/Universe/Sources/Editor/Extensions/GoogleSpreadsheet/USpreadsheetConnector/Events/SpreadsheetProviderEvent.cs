using System.Collections.Generic;

namespace Universe
{
    public class SpreadsheetProviderEvent
    {
        #region Public

        public void Add( SpreadsheetProviderCallback listener )
        {
            if( _listeners.Contains( listener ) ) return;

            AddListener( listener );
        }

        public void Remove( SpreadsheetProviderCallback listener )
        {
            if( !_listeners.Contains( listener ) ) return;

            RemoveListener( listener );
        }

        public void Invoke( USpreadsheetLinkData data, string response )
        {
            _eventHandler( data, response );
        }

        public delegate string SpreadsheetProviderCallback( USpreadsheetLinkData data, string response );

        #endregion


        #region Private

        private void AddListener( SpreadsheetProviderCallback listener )
        {
            _eventHandler += listener;
            _listeners.Add( listener );
        }

        private void RemoveListener( SpreadsheetProviderCallback listener )
        {
            _eventHandler -= listener;
            _listeners.Remove( listener );
        }

        private SpreadsheetProviderCallback _eventHandler;
        private List<SpreadsheetProviderCallback> _listeners = new List<SpreadsheetProviderCallback>();

        #endregion
    }
}