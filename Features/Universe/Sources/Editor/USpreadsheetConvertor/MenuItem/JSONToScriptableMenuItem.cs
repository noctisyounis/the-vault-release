using UnityEditor;

namespace Universe.Editor
{
    using static EditorWindow;

    public class JSONToScriptableMenuItem : UBehaviour
    {
        #region Main

        [MenuItem( "Vault/Content/JSON To Scriptable" )]
        public static void Launch()
        {
            GetWindow<JSONToScriptableEditorWindow>();
        }

        #endregion
    }
}