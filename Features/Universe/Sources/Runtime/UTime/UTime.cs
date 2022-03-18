namespace Universe
{
    public class UTime : UBehaviour
    {
        #region Public

        public static float DeltaTime => UnityEngine.Time.deltaTime;
        public static float UnscaledDeltaTime => UnityEngine.Time.unscaledDeltaTime;
        public static float FixedDeltaTime => UnityEngine.Time.fixedDeltaTime;
        public static float UnscaledTime => UnityEngine.Time.unscaledTime;
        public static float Time => UnityEngine.Time.time;

        #endregion
    }
}