using System.Collections.Generic;
using UnityEngine;

using static Universe.DebugWatchTools.Runtime.Outliner;

namespace Universe.DebugWatchTools.Runtime
{
    public class CollidersOutline : UBehaviour
    {

        #region Public Members

        [Header("Collider Outline Settings")]
        public OutlineStyle m_colliderStyle;
        public OutlineStyle m_triggerStyle;

        #endregion


        #region Unity API

        public override void Awake()
        {
            base.Awake();

            s_colliderStyle = m_colliderStyle;
            s_triggerStyle = m_triggerStyle;

            s_boxColliders       = new List<BoxCollider>();
            s_sphereColliders    = new List<SphereCollider>();

            GetTypeOfColliders();
        }

        public override void OnUpdate(float deltaTime)
        {
            Verbose($"Collider displayed : {s_displayed}");
            
            if (!s_displayed) return;

            DisplayColliders();
        }
        
        #endregion


        #region Main

        public void DisplayColliders()
        {
            foreach (var collider in s_boxColliders)
            {
                var isTrigger = collider.isTrigger;
                var style = isTrigger ? s_triggerStyle : s_colliderStyle;

                DrawColliderOutline(collider, style);
            }

            foreach (var collider in s_sphereColliders)
            {
                var isTrigger = collider.isTrigger;
                var color = isTrigger ? s_triggerStyle.m_color : s_colliderStyle.m_color;
                var width = isTrigger ? s_triggerStyle.m_width : s_colliderStyle.m_width;

                DrawSphere(collider.bounds.center, collider.radius, color);
            }
        }

        public void DrawColliderOutline(BoxCollider box, OutlineStyle style)
        {
            var center = box.bounds.center;
            var size = box.size;
            var transform = box.transform;

            DrawCuboidOutline(center, size, transform, style);
        }

        #endregion


        #region Utils

        private static void RefreshLists()
        {
            s_boxColliders.Clear();
            s_sphereColliders.Clear();

            GetTypeOfColliders();
        }

        public static void ToggleDisplay()
        {
            s_displayed = !s_displayed;

            if(!s_displayed) return;

            RefreshLists();
        }

        private static void GetTypeOfColliders()
        {
            var colliders = GetCollidersInScene();

            foreach (var collider in colliders)
            {
                if (IsBoxCollider(collider))
                {
                    s_boxColliders.Add((BoxCollider)collider);
                    continue;
                }

                if (IsSphereCollider(collider))
                {
                    s_sphereColliders.Add((SphereCollider)collider);
                }
            }
        }

        private static bool IsSphereCollider(Collider element) => 
            element.GetType() == typeof(SphereCollider);

        private static bool IsBoxCollider(Collider element) => 
            element.GetType() == typeof(BoxCollider);

        private static Collider[] GetCollidersInScene() => 
            GameObject.FindObjectsOfType<Collider>();

        #endregion


        #region Private Properties

        private static Camera MainCamera => s_mainCamera ??= Camera.main;

        #endregion


        #region private Members

        private static bool s_displayed;
        private static OutlineStyle s_colliderStyle;
        private static OutlineStyle s_triggerStyle;
        private static List<BoxCollider> s_boxColliders;
        private static List<SphereCollider> s_sphereColliders;
        private static Camera s_mainCamera;

        #endregion
    }
}