using System.Collections.Generic;
using UnityEngine;

using static Universe.U3rdPartyWrapper;

namespace Universe.DebugWatchTools.Runtime
{
    public static class Outliner
    {
        #region Main

        public static void DrawCubeOutline(Vector3 center, float size, Transform targetTransform, OutlineStyle style)
        {
            var cuboidSize = Vector3.one * size;

            DrawCuboidOutline(center, cuboidSize, targetTransform, style);
        }

        public static void DrawCuboidOutline(Vector3 center, Vector3 size, Transform targetTransform, OutlineStyle style)
        {
            var path        = GetCuboidOutline(center, size, targetTransform);
            var provider    = GetGizmosProvider();

            provider.DrawPolyline(path, false, style.m_width, style.m_color);
        }

        #endregion


        #region Utils

        public static List<Vector3> GetCuboidOutline(Vector3 center, Vector3 size, Transform targetTransform)
        {
            var vertices    = new List<Vector3>();
            var path        = new List<Vector3>();

            var halfWidth   = size.x * .5f;
            var halfHeigth  = size.y * .5f;
            var halfDepth   = size.z * .5f;

            var right   = targetTransform ? targetTransform.right : Vector3.right;
            var up      = targetTransform ? targetTransform.up : Vector3.up;
            var forward = targetTransform ? targetTransform.forward : Vector3.forward;

            var cuboidOutline = new CuboidOutlineData();

            cuboidOutline.m_center      = center;
            cuboidOutline.m_halfWidth   = halfWidth * right;
            cuboidOutline.m_halfHeight  = halfHeigth * up;
            cuboidOutline.m_halfDepth   = halfDepth * forward;
            
            ComputeVertexesPositions(cuboidOutline, vertices);

            path.Merge(vertices, s_cubeVertexOrder);

            return path;
        }

        private static void ComputeVertexesPositions(CuboidOutlineData datas, List<Vector3> vertices)
        {
            for (var x = -1; x <= 1; x += 2)
            {
                for (var y = -1; y <= 1; y += 2)
                {
                    for (var z = -1; z <= 1; z += 2)
                    {
                        var vertice = datas.m_center;

                        vertice += (datas.m_halfWidth * x);
                        vertice += (datas.m_halfHeight * y);
                        vertice += (datas.m_halfDepth * z);

                        vertices.Add(vertice);
                    }
                }
            }
        }

        #endregion


        #region Private Members

        private static int[] s_cubeVertexOrder = new int[] { 0, 1, 3, 2, 6, 7, 5, 4, 0, 1, 5, 4, 6, 7, 3, 2, 0 };

        #endregion
    }
}