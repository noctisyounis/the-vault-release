using UnityEditor;

using static UnityEditor.EditorWindow;

namespace Universe.Editor
{
    public class RefreshLanguageMenuItem
    {
        #region Main

        [MenuItem( "Vault/Localisation/RefreshLanguage" )]
        public static void Launch()
        {
            GetWindow<RefreshLanguageEditorWindow>();
        }

        #endregion
    }
}