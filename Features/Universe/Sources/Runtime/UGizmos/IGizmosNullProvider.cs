using System.Collections.Generic;
using UnityEngine;

namespace Universe
{
    public class IGizmosNullProvider : IGizmosProvider
    {
        public override void DrawCube(Vector3 position, float size, Color color ){}

        public override void DrawCube(Vector3 position, Quaternion rotation, float size, Color color){}

        public override void DrawCuboid(Vector3 position, Quaternion rotation, Vector3 dimension, Color color){}

        public override void DrawLine(Vector3 start, Vector3 end, Color color){}

        public override void DrawSphere(Vector3 position, float radius, Color color){}
        
        public override void DrawTorus(Vector3 position, Quaternion rotation, float radius, float thickness, Color color){}

        public override void DrawCone(Vector3 position, Quaternion rotation, float radius, float length, Color color){}
        
        public override void DrawPolyline(List<Vector3> points, bool closed, float thickness, Color color){}
        
        public override void DrawPolygon(List<Vector3> points, Color color){}
        
        public override void DrawTriangle(Vector3 a, Vector3 b, Vector3 c, float roundness, Color color){}
        
        public override void DrawDisc(Vector3 position, Vector3 normal, float radius, Color color){}
        
        public override void DrawRectangle(Vector3 position, Quaternion rotation, Vector2 size, Color color){}
    }
}