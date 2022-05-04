using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universe.SceneTask.Runtime
{
    public class LevelManager : UBehaviour
    {
        #region Exposed

        public LevelSettings m_settings;
        private static LevelSettings s_settings;
        public static LevelSettings Settings => s_settings;

        #endregion


        #region Unity API

        public override void Awake()
        {
            base.Awake();
            s_settings = m_settings;

            Level.CurrentEnvironment = Settings.m_startingEnvironment;
        }

        #endregion
    }
}