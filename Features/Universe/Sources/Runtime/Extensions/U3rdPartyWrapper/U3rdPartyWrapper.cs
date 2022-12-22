using UnityEngine;
using Universe.Stores.Runtime;

namespace Universe
{
    public class U3rdPartyWrapper : UBehaviour
    {
        #region Exposed
        
        [Header("Wrapper"), Space(15)]
        public ISaveProvider m_saveProvider;
        private static ISaveProvider s_saveProvider;

        public IInputProvider m_inputProvider;
        private static IInputProvider s_inputProvider;

        public IGizmosProvider m_gizmosProvider;
        private static IGizmosProvider s_gizmosProvider;

        public IStoreProvider m_storeProvider;
        private static IStoreProvider s_storeProvider;
        
        #endregion
        
        
        #region Save

        public static ISaveProvider GetSaveProvider()
            => s_saveProvider;
        

        public static IInputProvider GetInputProvider()
            => s_inputProvider;
        

        public static IGizmosProvider GetGizmosProvider()
            => s_gizmosProvider;

        public static IStoreProvider GetStoreProvider()
            => s_storeProvider;

        #endregion

        
        #region Unity API

        public override void Awake()
        {
            base.Awake();
            s_saveProvider = m_saveProvider;
            s_inputProvider = m_inputProvider;
            s_gizmosProvider = m_gizmosProvider;
            s_storeProvider = m_storeProvider;
        }

        #endregion
    }
}