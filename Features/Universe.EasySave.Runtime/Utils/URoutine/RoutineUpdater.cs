using System.Collections.Generic;

namespace Universe
{
    public class RoutineUpdater : UBehaviour
    {
        #region Main

        public void AddRoutineToUpdate( LoopRoutineData routine )
        {
            routine.OnComplete += OnRoutineComplete;
            _routines.Add( routine );
        }

        #endregion


        #region Unity API

        void Update()
        {
            _i = _routines.Count;

            while( --_i >= 0 )
            {
                _routines[_i].Update();
            }
        }

        #endregion


        #region Private

        private void OnRoutineComplete()
        {
            Verbose( "RoutineUpdate.OnRoutineComplete" );
            _i = _routines.Count;

            while( --_i >= 0 )
            {
                RemoveRoutineIfNotWorking();
            }

            if( _routines.Count <= 0 ) Destroy( this );
        }

        private void RemoveRoutineIfNotWorking()
        {
            if( _routines[_i].IsNotWorking )
            {
                _routines.Remove( _routines[_i] );
            }
        }

        private List<LoopRoutineData> _routines = new List<LoopRoutineData>();
        private int _i;

        #endregion
    }
}