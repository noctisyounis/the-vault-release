using UnityEngine;
using System.Collections.Generic;

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

        public static void Save( this UBehaviour source, FactBase fact ) => GetSaveProvider().Save( fact );

        public static void Save( this UBehaviour source, List<FactBase> facts ) => GetSaveProvider().Save( facts );

        public static void SaveAll(this UBehaviour source) => GetSaveProvider().SaveAll();

        public static void Load(this UBehaviour source, USaveLevel saveLevel) => GetSaveProvider().Load(saveLevel);

        public static void Load( this UBehaviour source, FactBase fact ) => GetSaveProvider().Load( fact );

        public static void Load( this UBehaviour source, List<FactBase> facts ) => GetSaveProvider().Load( facts );

        public static void LoadAll(this UBehaviour source) => GetSaveProvider().LoadAll();

        #endregion
    }
}