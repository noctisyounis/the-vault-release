using UnityEditor;

using static UnityEditor.EditorWindow;

namespace Universe.Editor
{
    public class LanguageGeneratorMenuItem
    {
        #region Main

        [MenuItem( "Vault/Localisation/Generate LanguageData" )]
        public static void Launch()
        {
            GetWindow<LanguageGeneratorEditorWindow>();
        }

        #endregion
    }
}