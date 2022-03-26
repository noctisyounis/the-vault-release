using System.Collections.Generic;

namespace Universe
{
    public class LifeCycleManager : UBehaviour
    {
        #region Static API
        
        
        
        
        #endregion
        
        
        #region Public API

        //public static void RegisterToUpdate(UBehaviour target) => _registeredUpdates.Add(target);
        /*public static void UnregisterToUpdate(UBehaviour target) => _registeredUpdates.Remove(target);

        public static void RegisterToFixedUpdate(UBehaviour target) => _registeredFixedUpdate.Add(target);
        public static void UnregisterToFixedUpdate(UBehaviour target) => _registeredFixedUpdate.Remove(target);

        public static void RegisterToLateUpdate(UBehaviour target) => _registeredLateUpdate.Add(target);
        public static void UnregisterToLateUpdate(UBehaviour target) => _registeredLateUpdate.Remove(target);
        */
        #endregion
        
        
        #region Unity API

        private void Update()
        {
            foreach (var target in _registeredUpdates)
            {
                target.OnUpdate(UTime.DeltaTime);
            }
        }

        private void FixedUpdate()
        {
            foreach (var target in _registeredFixedUpdate)
            {
                target.OnFixedUpdate(UTime.FixedDeltaTime);
            }
        }

        private void LateUpdate()
        {
            foreach (var target in _registeredLateUpdate)
            {
                target.OnLateUpdate(UTime.DeltaTime);
            }
        }
        
        #endregion
        
        
        #region Private

        private List<UBehaviour> _registeredUpdates = new();
        private List<UBehaviour> _registeredFixedUpdate = new();
        private List<UBehaviour> _registeredLateUpdate = new();

        #endregion
    }    
}