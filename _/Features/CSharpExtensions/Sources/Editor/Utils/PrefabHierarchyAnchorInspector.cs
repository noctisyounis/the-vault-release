using System.Collections.Generic;
using CSharpExtensions.Runtime;
using UnityEngine;

namespace CSharpExtension.Editor
{
    using UnityEditor;
    
    [CustomEditor(typeof(PrefabHierarchyAnchor))]
    public class PrefabHierarchyAnchorInspector : Editor
    {
        #region Unity API

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            var active = (Selection.activeObject as GameObject).GetComponent<PrefabHierarchyAnchor>();
            if (!active) return;
            
            var parentHolder = active.GetParentHolder();
            GUILayout.Label(parentHolder ? "Parent Holder Found" : "Parent Holder Not Found Or Disabled");
            if (!parentHolder) return;

            GUILayout.Space(10);
            active.m_referenceName = EditorGUILayout.TextField("Reference Name: ", active.m_referenceName);

            int index = -1;
            for (var i = 0; i < parentHolder.m_references.Count; i++)
            {
                var currentRef = parentHolder.m_references[i].m_reference;
                if (currentRef && currentRef == active)
                {
                    index = i;
                }
            }
            
            GUILayout.BeginHorizontal();
            
            if(index != -1)
            {
                if (GUILayout.Button("Rename"))
                {
                    var tempRef = parentHolder.m_references[index];
                    tempRef.m_name = active.m_referenceName.Length <= 0 ? active.name : active.m_referenceName;

                    parentHolder.m_references[index] = tempRef;
                }
                
                if (GUILayout.Button("Delete"))
                {
                    parentHolder.m_references.RemoveAt(index);
                }
            }
            else
            {
                if (GUILayout.Button("Add"))
                {
                    Selection.activeObject = parentHolder;

                    var reference = new PrefabHierarchyHolder.Reference
                    {
                        m_name = string.IsNullOrEmpty(active.m_referenceName) ? active.name : active.m_referenceName,
                        m_reference = active
                    };
                    
                    if (parentHolder.m_references == null)
                        parentHolder.m_references = new List<PrefabHierarchyHolder.Reference>();
                    
                    parentHolder.m_references.Add(reference);
                    EditorUtility.SetDirty(parentHolder);
                }
            }
            
            GUILayout.EndHorizontal();
        }
        
        #endregion
    }
}