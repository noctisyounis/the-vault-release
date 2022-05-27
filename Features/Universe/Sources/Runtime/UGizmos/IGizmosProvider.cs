using System.Collections.Generic;
using UnityEngine;

namespace Universe
{
    public abstract class IGizmosProvider : UBehaviour
    {
        #region Main

        public new abstract void DrawCube(Vector3 position, float size, Color color);

        public new abstract void DrawCube(Vector3 position, Quaternion rotation, float size, Color color);

        public new abstract void DrawCuboid(Vector3 position, Quaternion rotation, Vector3 dimension, Color color);
        
        public new abstract void DrawLine(Vector3 start, Vector3 end, Color color);

        public new abstract void DrawSphere(Vector3 position, float radius, Color color);
        
        public new abstract void DrawTorus(Vector3 position, Quaternion rotation, float radius, float thickness, Color color);
        
        public new abstract void DrawCone(Vector3 position, Quaternion rotation, float radius, float length, Color color);
        
        public new abstract void DrawPolyline(List<Vector3> points, bool closed, float thickness, Color color);
        
        public new abstract void DrawPolygon(List<Vector3> points, Color color);
        
        public new abstract void DrawTriangle(Vector3 a, Vector3 b, Vector3 c, float roundness, Color color);
        
        public new abstract void DrawDisc(Vector3 position, Vector3 normal, float radius, Color color);
        
        public new abstract void DrawRectangle(Vector3 position, Quaternion rotation, Vector2 size, Color color);
        
        #endregion
    }
}