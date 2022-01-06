using System.Collections.Generic;
using UnityEngine;
using static Universe.U3rdPartyWrapper;

namespace Universe
{
    public static class UGizmos
    {
        #region Public API

        public static void UDrawCube(this UBehaviour target, Vector3 position, float size, Color color) =>
            GetGizmosProvider().DrawCube(position, size, color);
        
        public static void UDrawLine(this UBehaviour target, Vector3 start, Vector3 end, Color color) =>
            GetGizmosProvider().DrawLine(start, end, color);
        
        public static void UDrawSphere(this UBehaviour target, Vector3 position, float radius, Color color) =>
            GetGizmosProvider().DrawSphere(position, radius, color);
        
        public static void UDrawTorus(this UBehaviour target, Vector3 position, Quaternion rotation, float radius, float thickness, Color color) =>
            GetGizmosProvider().DrawTorus(position, rotation, radius, thickness, color);
        
        public static void UDrawCone(this UBehaviour target, Vector3 position, Quaternion rotation, float radius, float length, Color color) =>
            GetGizmosProvider().DrawCone(position, rotation, radius, length, color);
        
        public static void UDrawPolyline(this UBehaviour target, List<Vector3> points, bool closed, float thickness, Color color) =>
            GetGizmosProvider().DrawPolyline(points, closed, thickness, color);
        
        public static void UDrawPolygon(this UBehaviour target, List<Vector3> points, Color color) =>
            GetGizmosProvider().DrawPolygon(points, color);

        public static void UDrawTriangle(this UBehaviour target, Vector3 a, Vector3 b, Vector3 c, float roundness, Color color) =>
            GetGizmosProvider().DrawTriangle(a, b, c, roundness, color);

        public static void UDrawDisc(this UBehaviour target, Vector3 position, Vector3 normal, float radius, Color color) =>
            GetGizmosProvider().DrawDisc(position, normal, radius, color);

        public static void UDrawRectangle(this UBehaviour target, Vector3 position, Quaternion rotation, Vector2 size, Color color) =>
            GetGizmosProvider().DrawRectangle(position, rotation, size, color);
        
        #endregion
    }
}