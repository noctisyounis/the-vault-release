//
// URP Debug Views for Unity
// (c) 2019 PH Graphics
// Source code may be used and modified for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 
// *** A NOTE ABOUT PIRACY ***
// 
// If you got this asset from a pirate site, please consider buying it from the Unity asset store. This asset is only legally available from the Unity Asset Store.
// 
// I'm a single indie dev supporting my family by spending hundreds and thousands of hours on this and other assets. It's very offensive, rude and just plain evil to steal when I (and many others) put so much hard work into the software.
// 
// Thank you.
//
// *** END NOTE ABOUT PIRACY ***
//

using UnityEditor;
using UnityEngine;

namespace URPDebugViews
{
    public class DebugViewMenu : MonoBehaviour
    {
        [MenuItem("Tools/Debug Views/None", false, 0)]
        private static void None()
        {
            DebugViewsManager.Instance.EnableViewWithMaterialName("None");
        }

        [MenuItem("Tools/Debug Views/Shaded Wireframe", false, 100)]
        private static void ShadedWireframe()
        {
            DebugViewsManager.Instance.EnableViewWithMaterialName("ShadedWireframe");
        }
        
        [MenuItem("Tools/Debug Views/Overdraw", false, 101)]
        private static void Overdraw()
        {
            DebugViewsManager.Instance.EnableViewWithMaterialName("Overdraw");
        }
        
        [MenuItem("Tools/Debug Views/UV0 Checker", false, 1000)]
        private static void UV0Checker()
        {
            DebugViewsManager.Instance.EnableViewWithMaterialName("UV0Checker");
        }
        
        [MenuItem("Tools/Debug Views/UV1 Checker", false, 1001)]
        private static void UV1Checker()
        {
            DebugViewsManager.Instance.EnableViewWithMaterialName("UV1Checker");
        }
        
        [MenuItem("Tools/Debug Views/Shadow Cascades", false, 10000)]
        private static void ShadowCascades()
        {
            DebugViewsManager.Instance.EnableViewWithMaterialName("ShadowCascades");
        }
        
        [MenuItem("Tools/Debug Views/Vertex color R", false, 100000)]
        private static void VertexColorR()
        {
            DebugViewsManager.Instance.EnableViewWithMaterialName("VertexColorR");
        }
        
        [MenuItem("Tools/Debug Views/Vertex color G", false, 100001)]
        private static void VertexColorG()
        {
            DebugViewsManager.Instance.EnableViewWithMaterialName("VertexColorG");
        }
        
        [MenuItem("Tools/Debug Views/Vertex color B", false, 100002)]
        private static void VertexColorB()
        {
            DebugViewsManager.Instance.EnableViewWithMaterialName("VertexColorB");
        }

        [MenuItem("Tools/Debug Views/Vertex color A", false, 100003)]
        private static void VertexColorA()
        {
            DebugViewsManager.Instance.EnableViewWithMaterialName("VertexColorA");
        }
        
        [MenuItem("Tools/Debug Views/Vertex color RGB", false, 100004)]
        private static void VertexColorRGB()
        {
            DebugViewsManager.Instance.EnableViewWithMaterialName("VertexColorRGB");
        }

        [MenuItem("Tools/Debug Views/World normals", false, 1000000)]
        private static void WorldNormals()
        {
            DebugViewsManager.Instance.EnableViewWithMaterialName("WorldNormals");
        }

        [MenuItem("Tools/Debug Views/World tangents", false, 1000001)]
        private static void WorldTangents()
        {
            DebugViewsManager.Instance.EnableViewWithMaterialName("WorldTangents");
        }
    }
}