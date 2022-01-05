using UnityEngine;

namespace Universe
{
    using static U3rdPartyWrapper;

    public static class USaver
    {
        #region Public

        [Header("Runtime"), Space(10)]
        public static FloatFact m_normalizedPercentageOfCompletion;

        [Header("Signals"), Space(10)]
        public static GameSignal OnSaveFinish;
        public static GameSignal OnLoadFinish;

        
        public static void Save(this UBehaviour source, USaveLevel saveLevel) => GetSaveProvider().Save(saveLevel);
   
        public static void SaveAll(this UBehaviour source) => GetSaveProvider().SaveAll();

        public static void Load(this UBehaviour source, USaveLevel saveLevel) => GetSaveProvider().Load(saveLevel);

        public static void LoadAll(this UBehaviour source) => GetSaveProvider().LoadAll();

        #endregion
    }
}