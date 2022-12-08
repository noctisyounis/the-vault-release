using System.Collections.Generic;

using static Universe.UTime;

namespace Universe
{
    public class LifeCycleManager : UBehaviour
    {
        #region Unity API

        private void Update()
        {
            foreach (var target in _registeredUpdates)
                target.OnUpdate(DeltaTime);
        }

        private void FixedUpdate()
        {
            foreach (var target in _registeredFixedUpdate)
                target.OnFixedUpdate(FixedDeltaTime);
        }

        private void LateUpdate()
        {
            foreach (var target in _registeredLateUpdate)
                target.OnLateUpdate(DeltaTime);
        }
        
        #endregion
        
        
        #region Private

        private List<UBehaviour> _registeredUpdates = new();
        private List<UBehaviour> _registeredFixedUpdate = new();
        private List<UBehaviour> _registeredLateUpdate = new();

        #endregion
    }    
}