using UnityEngine;

namespace Universe
{
    public class UTime : UBehaviour
    {
        #region Public

        public static float DeltaTime => Time.deltaTime;
        public static float FixedDeltaTime => Time.fixedDeltaTime;

        #endregion
    }
}