using System;
using System.Collections;
using UnityEngine;

namespace Universe
{
    public class LoopRoutineData
    {
        #region Public

        public GameSignal OnRoutineComplete = ScriptableObject.CreateInstance<GameSignal>();
        public LoopCallback OnComplete;
        
        public bool IsWorking { get; private set; }
        public bool IsNotWorking => !IsWorking;

        public LoopRoutineData( uint numberOfCallByFrame, IList list, OnLoop OnLoop )
        {
            InitializeRoutine( numberOfCallByFrame, list, OnLoop );
            StartRoutineInternal( numberOfCallByFrame, list, OnLoop );
        }

        public delegate void OnLoop( object index );
        public delegate void LoopCallback();
        public delegate void LoopCallback<T>( T param );

        #endregion


        #region Unity API

        public void Update()
        {
            if( IsNotWorking ) return;
            FragmentedLoop();
        }

        #endregion


        #region Callbacks

        internal static bool OnLoopCallback( LoopCallback callback )
        {
            try
            {
                callback();
            }
            catch( Exception e )
            {
                LogCallbackException( e );
                return false; // Callback error
            }
            return true;
        }

        internal static bool OnLoopCallback<T>( LoopCallback<T> callback, T param )
        {
            try
            {
                callback( param );
            }
            catch( Exception e )
            {
                LogCallbackException( e );
                return false; // Callback error
            }
            return true;
        }

        #endregion


        #region Utils

        private void InitializeRoutine( uint numberOfCallByFrame, IList list, OnLoop OnLoop )
        {
            _numberOfCallByFrame = numberOfCallByFrame;
            _loopHandler = OnLoop;
            _list = list;
            _currentIndex = 0;
        }

        private void StartRoutineInternal( uint numberOfCallByFrame, IList list, OnLoop OnLoop )
        {
            if( IsWorking )
            {
                Debug.LogError( "ERROR routine already working" );
                return;
            }

            if( IsEmptyList( list ) ) return;

            IsWorking = true;
        }

        private void FragmentedLoop()
        {
            IsLastLoop = false;
            var endIndex = CalculateFragmentedEndIndex();

            for( var i = _currentIndex; i <= endIndex; i++ )
            {
                _loopHandler( _list[i] );
            }
            _currentIndex = endIndex + 1;
            
            if( IsLastLoop )
            {
                CompleteRoutine();
            }
        }

        private void CompleteRoutine()
        {
            IsWorking = false;
            OnLoopCallback( OnComplete );
            OnRoutineComplete.Emit();
        }

        private int CalculateFragmentedEndIndex()
        {
            var endIndex = (int)( _currentIndex + _numberOfCallByFrame - 1 );
            endIndex = Mathf.Clamp( endIndex, 0, _list.Count - 1 );
            if( IsLastElementOrOverflowed( endIndex ) )
            {
                IsLastLoop = true;
                endIndex = _list.Count - 1;
            }

            return endIndex;
        }

        private static void LogCallbackException( Exception e )
        {
            var errorMessage = "An error inside a routine callback was taken care of ({0}) ► {1}\n\n{2}\n\n";
            Debug.LogError( string.Format( errorMessage, e.TargetSite, e.Message, e.StackTrace ) );
        }

        #endregion


        #region Private

        private bool IsLastElementOrOverflowed( int endIndex ) => endIndex >= _list.Count - 1;
        private static bool IsEmptyList( IList list ) => list.Count == 0;

        private IList _list;
        private uint _numberOfCallByFrame;
        private OnLoop _loopHandler;

        private int _currentIndex = 0;
        private bool IsLastLoop { get; set; } = false;

        #endregion
    }
}