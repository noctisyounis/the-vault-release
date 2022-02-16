using Sirenix.OdinInspector;
using UnityEngine;
using Universe;

using static UnityEngine.GUILayout;

namespace Universe.SceneTask.Runtime
{
    public class USceneTask : UBehaviour
    {
        #region Exposed

        public TaskData m_taskData;

        [Header( "Debug" ), EnableIf( "IsDebug" ), Space( 15 )]
        public TaskData m_additive01;
        [EnableIf( "IsDebug" )]
        public TaskData m_additive02;
        [EnableIf( "IsDebug" )]
        public TaskData m_cameraScene;

        #endregion


        #region Main

        public void GoToScene( TaskData sceneTask )
        {
            this.LoadTask( sceneTask );
        }

        public void UnloadPreviousAndGoToScene( TaskData sceneTask )
        {
            this.UUnloadLastTaskAndLoad( sceneTask );
        }

        public void UnloadScene( TaskData sceneTask )
        {
            this.UnloadTask( sceneTask );
        }

        #endregion


        #region Unity API

        private void OnGUI()
        {
            if( !IsDebug ) return;

            if( Button( "Load Camera Scene " ) ) GoToScene( m_cameraScene );
            if( Button( "Load Additive 01 " ) ) GoToScene( m_additive01 );
            if( Button( "Load Additive 02 " ) ) GoToScene( m_additive02 );
            if( Button( "Unload Additive 01 " ) ) UnloadScene( m_additive01 );
            if( Button( "Unload Additive 02 " ) ) UnloadScene( m_additive02 );
            if( Button( "UnloadPreviousAndGoTo Additive 01 " ) ) UnloadPreviousAndGoToScene( m_additive01 );
            if( Button( "UnloadPreviousAndGoTo Additive 02 " ) ) UnloadPreviousAndGoToScene( m_additive02 );
            if( Button("Debug Log ordered Scene ")) Task.LogDisplaySceneOrder();
        }

        #endregion
    }
}