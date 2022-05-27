using UnityEditor;

namespace Universe.Editor
{
    [CustomEditor(typeof(Identity))]
    public class IdentityDrawer : UnityEditor.Editor
    {
        #region Unity API
        
        public override void OnInspectorGUI()
        {
            if (_guidComp == null)
            {
                _guidComp = (Identity)target;
            }
            
            EditorGUILayout.LabelField("Guid:", _guidComp.GetGuid().ToString());
        }
        
        #endregion
        
        
        #region Private
        
        private Identity _guidComp;
        
        #endregion
    }
}