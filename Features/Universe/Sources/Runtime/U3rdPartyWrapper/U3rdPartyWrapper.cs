using UnityEngine;

namespace Universe
{
    public class U3rdPartyWrapper : UBehaviour
    {

        public Input test;
        
        #region Save

        [Header("Wrapper"), Space(15)]
        public ISaveProvider m_saveProvider;
        private static ISaveProvider s_saveProvider;

        public IInputProvider m_inputProvider;
        private static IInputProvider s_inputProvider;

        public IGizmosProvider m_gizmosProvider;
        private static IGizmosProvider s_gizmosProvider;
        
        

        public static ISaveProvider GetSaveProvider()
        {
            return s_saveProvider;
        }

        public static IInputProvider GetInputProvider()
        {
            return s_inputProvider;
        }

        public static IGizmosProvider GetGizmosProvider()
        {
            return s_gizmosProvider;
        }

        #endregion

        
        #region Unity API

        public override void Awake()
        {
            base.Awake();
            s_saveProvider = m_saveProvider;
            s_inputProvider = m_inputProvider;
            s_gizmosProvider = m_gizmosProvider;
        }

        #endregion
    }
}