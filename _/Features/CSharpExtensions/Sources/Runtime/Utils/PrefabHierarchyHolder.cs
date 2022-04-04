using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

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

      [System.Serializable]
      public struct Addresse
      {
         public string m_name;
         public AssetReference m_reference;
      }

      public List<Reference> m_references;
      public List<Addresse> m_addresses;
      
      #endregion
      
      #region Main

      public bool ReferencesHave(string referenceSignature)
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

      public Object ReferencesGet(string referenceSignature)
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
      
      public GameObject ReferencesGetGameObject(string referenceSignature) =>
         (GameObject) ReferencesGet(referenceSignature);
      
      
      
      public bool AddressesHave(string referenceSignature)
      {
         for (var i = 0; i < m_addresses.Count; i++)
         {
            if (m_addresses[i].m_name == referenceSignature)
            {
               return true;
            }
         }

         return false;
      }
      
      public AssetReference AddressesGet(string referenceSignature)
      {
         for (var i = 0; i < m_addresses.Count; i++)
         {
            if (m_addresses[i].m_name == referenceSignature)
            {
               return m_addresses[i].m_reference;
            }
         }

         return null;
      }
      
      #endregion
   }
}