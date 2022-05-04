using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universe.SceneTask.Runtime
{
    public class LevelSettings : USettings
    {
        #region Exposed

        public LevelData m_startingLevel;
        public Environment m_startingEnvironment;
        public int m_startingTask;

        #endregion
    }
}
