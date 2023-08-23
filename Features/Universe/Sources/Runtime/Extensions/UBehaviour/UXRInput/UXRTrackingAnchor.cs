using UnityEngine;
using System.Linq;
using UnityEngine.AddressableAssets;
using Universe.SceneTask.Runtime;
using static Universe.SceneTask.Runtime.Task;

namespace Universe
{
    public class UXRTrackingAnchor : UBehaviour
    {
        public AssetReference m_XRRig;
        public XRProvider m_xrProvider;
        
        #region Unity API
        
        public override void Awake()
        {
            base.Awake();
            Spawn( m_XRRig, transform, InitializeTrackedElements );
        }

        public override void OnUpdate(float deltatime)
        {
            if (!_xrRigInitialized) return;
            
            activeTask = GetFocusedTaskManager();
            activeTask.SetHeadset(m_xrProvider.GetHeadsetData());
            activeTask.SetPlayArea(m_xrProvider.GetPlayAreaData());
            activeTask.SetLeftController(m_xrProvider.GetLeftControllerData());
            activeTask.SetRightController(m_xrProvider.GetRightControllerData());
        }

        #endregion
        
        
        #region Utils

        private void InitializeTrackedElements(GameObject spawnedObject)
        {
            m_xrProvider.SetXRRig(spawnedObject);
            _xrRigInitialized = true;
        }
        
        #endregion
        

        #region Private

        private TaskManager activeTask;
        private bool _xrRigInitialized = false;
        
        #endregion
    }
}