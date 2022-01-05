using System;
using System.Collections.Generic;
using UnityEngine;

namespace Universe
{
    [Serializable]
    public abstract class ISaveProvider : UBehaviour
    {
        [Header("Runtime"), Space(10)]
        public FloatFact m_normalizedPercentageOfCompletion;

        [Header("Signals"), Space(10)]
        public GameSignal OnSaveFinish;
        public GameSignal OnLoadFinish;

        [Header("Save Data"), Space(15)]
        public List<FactBase> m_factList;

        public abstract void Save(USaveLevel saveLevel);

        public abstract void SaveAll();

        public abstract void Load(USaveLevel saveLevel);

        public abstract void LoadAll();
    }
}