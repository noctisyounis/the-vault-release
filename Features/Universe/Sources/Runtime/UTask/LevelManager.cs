namespace Universe.SceneTask.Runtime
{
    public class LevelManager : UBehaviour
    {
        #region Exposed

        public LevelSettings m_settings;
        public static LevelSettings Settings => s_settings;

        #endregion


        #region Unity API

        public override void Awake()
        {
            base.Awake();
            s_settings = m_settings;

            Situation.CurrentEnvironment = Settings.m_startingEnvironment;
        }

        #endregion


        #region Private

        private static LevelSettings s_settings;

        #endregion
    }
}