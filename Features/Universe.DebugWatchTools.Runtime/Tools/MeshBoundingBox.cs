using System.Collections.Generic;
using UnityEngine;
using Shapes;

using static Universe.DebugWatchTools.Runtime.Outliner;

namespace Universe.DebugWatchTools.Runtime
{
    public class MeshBoundingBox : UBehaviour
    {
        #region Public Members

        public OutlineStyle m_style;

        #endregion


        #region Unity API

        public override void Awake()
        {
            base.Awake();

            s_meshes = new List<MeshRenderer>();
            s_style = m_style;
        }

        private void Start() => GetMeshesInScene();

        #endregion


        #region Universe API

        public override void OnUpdate(float deltaTime)
        {
            if (!s_isDisplay) return;

            DrawMeshesBoundingBoxes();
        }

        #endregion


        #region Main

        public static void ToggleDisplay()
        {
            s_isDisplay = !s_isDisplay;

            if(!s_isDisplay) return;

            RefreshMeshes();
        }

        private static void RefreshMeshes()
        {
            s_meshes.Clear();
            GetMeshesInScene();
        }

        private static void GetMeshesInScene()
        {
            var meshRenderers = FindObjectsOfType(typeof(MeshRenderer)) as MeshRenderer[];
            
            foreach (var mesh in meshRenderers) 
                s_meshes.Add(mesh);
        }

        #endregion


        #region Drawing Shape

        private void DrawMeshesBoundingBoxes()
        {
            foreach (var mesh in s_meshes)
                DrawBoundingBox(mesh);
        }

        private void DrawBoundingBox(MeshRenderer mesh)
        {
            var bounds = mesh.bounds;
            var center = bounds.center;

            DrawCuboidOutline(center, bounds.size, null, s_style);
        }

        #endregion


        #region Private Members

        private static bool s_isDisplay;
        private static List<MeshRenderer> s_meshes;
        private static OutlineStyle s_style;

        #endregion
    }
}