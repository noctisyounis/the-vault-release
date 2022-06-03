using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universe.SceneTask.Runtime
{
    public class CheckpointData : UniverseScriptableObject
    {
        #region Exposed

        public LevelData m_level;
        public TaskData m_task;

        #endregion
    }
}