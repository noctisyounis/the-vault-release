using System.Collections;

namespace Universe
{
    using static LoopRoutineData;

    public static class ULoopRoutine
    {
        #region Main

        public static LoopRoutineData StartRoutine(this UBehaviour source, uint numberOfCallByFrame, IList list, OnLoop OnLoop)
        {
            var updater = GetOrCreateRoutineUpdate( source );
            var routine = CreateNewRoutine( numberOfCallByFrame, list, OnLoop, updater );
            return routine;
        }

        #endregion


        #region Utils

        private static RoutineUpdater GetOrCreateRoutineUpdate( UBehaviour source )
        {
            if( !source.TryGetComponent( out RoutineUpdater updater ) )
            {
                updater = source.gameObject.AddComponent<RoutineUpdater>();
            }

            return updater;
        }

        private static LoopRoutineData CreateNewRoutine( uint numberOfCallByFrame, IList list, OnLoop OnLoop, RoutineUpdater updater )
        {
            var routine = new LoopRoutineData( numberOfCallByFrame, list, OnLoop );
            updater.AddRoutineToUpdate( routine );
            return routine;
        }

        #endregion
    }
}