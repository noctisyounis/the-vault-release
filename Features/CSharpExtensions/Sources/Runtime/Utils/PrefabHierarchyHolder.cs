using System.Collections.Generic;
using UnityEngine;

namespace CSharpExtensions.Runtime
{
   public class PrefabHierarchyHolder : MonoBehaviour
   {
      #region Public
      
      [System.Serializable]
      public struct Reference
      {
         public string m_name;
         public Object m_reference;

      }

      public List<Reference> m_references;
     
      #endregion
      
      #region Main

      public bool Have(string referenceSignature)
      {
         for (var i = 0; i < m_references.Count; i++)
         {
            if (m_references[i].m_name == referenceSignature)
            {
               return true;
            }
         }

         return false;
      }

      public Object Get(string referenceSignature)
      {
         for (var i = 0; i < m_references.Count; i++)
         {
            if (m_references[i].m_name == referenceSignature)
            {
               return m_references[i].m_reference;
            }
         }

         return null;
      }
      
      public GameObject GetGameObject(string referenceSignature) =>
         (GameObject) Get(referenceSignature);
      
      #endregion
   }
}