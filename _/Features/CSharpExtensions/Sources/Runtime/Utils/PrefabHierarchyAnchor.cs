using UnityEngine;

namespace CSharpExtensions.Runtime
{
    public class PrefabHierarchyAnchor : MonoBehaviour
    {
        #region Public

        public string m_referenceName;
        
        #endregion
        
        
        #region Main

        public PrefabHierarchyHolder GetParentHolder()
        {
            return GetComponentInParent<PrefabHierarchyHolder>();
        }
            


        public bool ReferenceThisInParentHolder()
        {
            var parentHolder = GetParentHolder();

            if (!parentHolder) return false;
            if (parentHolder.m_references == null) return false;
            
            var reference = new PrefabHierarchyHolder.Reference
            {
                m_name = name,
                m_reference = gameObject
            };
            
            parentHolder.m_references.Add(reference);
            return true;
        }

        #endregion
    }
}