using System.Collections.Generic;
using Shapes;
using UnityEngine;

using static Shapes.Draw;

namespace Universe.ShapesProvider.Runtime
{
    public class ShapesProvider : IGizmosProvider
    {
        #region Public Members

        public ShapesManager m_shapesManager;
        
        #endregion
        
        
        #region Public API
        
        public override void DrawCube(Vector3 position, float size, Color color) =>
            m_shapesManager.AddGizmos(() => Cube(position, size, color));
        public override void DrawCube(Vector3 position, Quaternion rotation, float size, Color color) =>
            m_shapesManager.AddGizmos(() => Cube(position, rotation, size, color));

        public override void DrawCuboid(Vector3 position, Quaternion rotation, Vector3 size, Color color) =>
            m_shapesManager.AddGizmos(() => Cuboid(position, rotation, size, color));

        public override void DrawLine(Vector3 start, Vector3 end, Color color) =>
            m_shapesManager.AddGizmos(() =>Line(start, end, color));
            
        public override void DrawSphere(Vector3 position, float radius, Color color) =>
            m_shapesManager.AddGizmos(() =>Sphere(position, radius, color));

        public override void DrawTorus(Vector3 position, Quaternion rotation, float radius, float thickness, Color color) =>
            m_shapesManager.AddGizmos(() =>Torus(position, rotation, radius, thickness, color));

        public override void DrawCone(Vector3 position, Quaternion rotation, float radius, float length, Color color) =>
            m_shapesManager.AddGizmos(() =>Cone(Position, Rotation, radius, length, color));

        public override void DrawPolyline(List<Vector3> points, bool closed, float thickness, Color color)
        {
            m_shapesManager.AddGizmos(() => 
            {
                using(var p = ConvertToPolylinePath(points))
                {
                    Polyline(p, closed, thickness, color);
                }
            });
        }

        public override void DrawPolygon(List<Vector3> points, Color color)
        {
            var path = ConvertToPolygonPath(points);
            m_shapesManager.AddGizmos(() =>Polygon(path, color));
        }

        public void DrawQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Color color) =>
            m_shapesManager.AddGizmos(() =>Quad(a,b,c,d, color));

        public override void DrawTriangle(Vector3 a, Vector3 b, Vector3 c, float roundness, Color color) =>
            m_shapesManager.AddGizmos(() =>Triangle(a,b,c,roundness, color));
        
        public override void DrawDisc(Vector3 position, Vector3 normal, float radius, Color color)
        {
            var colors = new DiscColors
            {
                innerEnd = color,
                innerStart = color,
                outerEnd = color,
                outerStart = color
            };

            m_shapesManager.AddGizmos(() =>Disc(position, normal, radius, colors));
        }

        public override void DrawRectangle(Vector3 position, Quaternion rotation, Vector2 size, Color color) =>
            m_shapesManager.AddGizmos(() =>Rectangle(position, rotation, size, color ));
        
        #endregion
        
        
        #region Utils

        private PolylinePath ConvertToPolylinePath(List<Vector3> points)
        {
            var path = new PolylinePath();
            
            foreach (var point in points)
            {
                path.AddPoint(point);
            }

            return path;
        }

        private PolygonPath ConvertToPolygonPath(List<Vector3> points)
        {
            var path = new PolygonPath();

            foreach (var point in points)
            {
                path.AddPoint(point);
            }

            return path;
        }

        #endregion
    }
}