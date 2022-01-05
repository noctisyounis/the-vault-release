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
        

        public static ISaveProvider GetSaveProvider()
        {
            return s_saveProvider;
        }

        public static IInputProvider GetInputProvider()
        {
            return s_inputProvider;
        }

        #endregion

        
        #region Unity API

        public override void Awake()
        {
            base.Awake();
            s_saveProvider = m_saveProvider;
            s_inputProvider = m_inputProvider;
        }

        #endregion
    }
}