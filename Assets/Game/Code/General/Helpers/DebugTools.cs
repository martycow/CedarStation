using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Game.General
{
    public static class DebugTools
    {
        public static void DrawArrow(Vector3 from, Vector3 to, Color color, float duration = 0f, bool depthTest = true)
        {
#if UNITY_EDITOR
            Debug.DrawLine(from, to, color, duration, depthTest);
            
            var direction = (to - from).normalized;
            var right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 150, 0) * Vector3.forward;
            var left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -150, 0) * Vector3.forward;
            
            Debug.DrawLine(to, to + right * 0.25f, color, duration, depthTest);
            Debug.DrawLine(to, to + left * 0.25f, color, duration, depthTest);
#endif
        }
        
        public static void DrawCircle(Vector3 center, float radius, Color color, int segments = 32, float duration = 0f, bool depthTest = true)
        {
#if UNITY_EDITOR
            var prev = center + Vector3.forward * radius;
            for (var i = 1; i <= segments; i++)
            {
                var angle = i * Mathf.PI * 2f / segments;
                var next = center + new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)) * radius;
                Debug.DrawLine(prev, next, color, duration, depthTest);
                prev = next;
            }
#endif
        }
        
        public static void DrawBounds(Bounds bounds, Color color,  float duration = 0f, bool depthTest = true)
        {
#if UNITY_EDITOR
            var center = bounds.center; 
            var e = bounds.extents;
            
            Vector3[] vertices = {
                center + new Vector3(-e.x,-e.y,-e.z), // 0
                center + new Vector3( e.x,-e.y,-e.z), // 1
                center + new Vector3( e.x,-e.y, e.z), // 2
                center + new Vector3(-e.x,-e.y, e.z), // 3
                center + new Vector3(-e.x, e.y,-e.z), // 4
                center + new Vector3( e.x, e.y,-e.z), // 5
                center + new Vector3( e.x, e.y, e.z), // 6
                center + new Vector3(-e.x, e.y, e.z), // 7
            };
            
            int[,] edges =
            {
                {0,1},
                {1,2},
                {2,3},
                {3,0},
                {4,5},
                {5,6},
                {6,7},
                {7,4},
                {0,4},
                {1,5},
                {2,6},
                {3,7}
            };
            
            for (var i = 0; i < edges.GetLength(0); i++)
            {
                var edgeStart = edges[i, 0];
                var edgeEnd = edges[i, 1];

                var p1 = vertices[edgeStart];
                var p2 = vertices[edgeEnd];
                
                Debug.DrawLine(p1, p2, color, duration, depthTest);
            }
#endif
        }

        public static void DrawThickLine(Vector3 from, Vector3 to, int thickness, Color color, float density = 0.01f, float duration = 0f, bool depthTest = true)
        {
#if UNITY_EDITOR
            var direction = (to - from).normalized;
            var perp1 = Vector3.Cross(direction, Vector3.up);
            if (perp1.sqrMagnitude < 0.001f)
                perp1 = Vector3.Cross(direction, Vector3.forward);
            perp1.Normalize();
            var perp2 = Vector3.Cross(direction, perp1).normalized;

            Debug.DrawLine(from, to, color, duration, depthTest);

            for (var i = 1; i < thickness; i++)
            {
                var o1 = perp1 * (i * density);
                var o2 = perp2 * (i * density);
                Debug.DrawLine(from + o1, to + o1, color, duration, depthTest);
                Debug.DrawLine(from - o1, to - o1, color, duration, depthTest);
                Debug.DrawLine(from + o2, to + o2, color, duration, depthTest);
                Debug.DrawLine(from - o2, to - o2, color, duration, depthTest);
            }
#endif
        }
        public static readonly (Vector3, Vector3)[] SegmentDisplayLines = 
        {
            // Upper Half
            (new Vector3(0, 0.5f, 0), new Vector3(0, 1f, 0)),       // 0  Left |
            (new Vector3(0, 1f, 0), new Vector3(1f, 1f, 0)),        // 1  Upper _
            (new Vector3(1f, 1f, 0), new Vector3(1f, 0.5f, 0)),     // 2  Right |
            (new Vector3(1f, 0.5f, 0), new Vector3(0f, 0.5f, 0)),   // 3  Lower _
            
            // Lower Half
            (new Vector3(0f, 0.5f, 0f), new Vector3(0f, 0f, 0f)),   // 4  Left |
            (new Vector3(0f, 0f, 0f), new Vector3(1f, 0f, 0f)),     // 5  Lower _
            (new Vector3(1f, 0f, 0f), new Vector3(1f, 0.5f, 0f)),   // 6  Right |
        };
        
        public static void DrawSymbol(char symbol, Color color, int thickness = 2, float density = 0.01f, float duration = 0f, bool depthTest = true)
        {
#if UNITY_EDITOR
            int[] indexes;
            switch (symbol)
            {
                case '0':
                    indexes = new[] { 0, 1, 2, 4, 5, 6 };
                    break;
                case '1':
                    indexes = new[] { 2, 6 };
                    break;
                case '2':
                    indexes = new[] { 1, 2, 3, 4, 5};
                    break;
                case '3':
                    indexes = new[] { 1, 2, 3, 5, 6 };
                    break;
                case '4':
                    indexes = new[] { 0, 2, 3, 6 };
                    break;
                case '5':
                    indexes = new[] { 0, 1, 3, 5, 6 };
                    break;
                case '6':
                    indexes = new[] { 0, 1, 3, 4, 5, 6 };
                    break;
                case '7':
                    indexes = new[] { 1, 2, 6 };
                    break;
                case '8':
                    indexes = new[] { 0, 1, 2, 3, 4, 5, 6 };
                    break;
                case '9':
                    indexes = new[] { 0, 1, 2, 3, 5, 6 };
                    break;
                default:
                    indexes = Array.Empty<int>();
                    break;
            }
            
            foreach (var index in indexes)
            {
                if (index < 0 || index > SegmentDisplayLines.Length - 1)
                    continue;

                var (start, end) = SegmentDisplayLines[index];
                
                DrawThickLine(start, end, thickness, color, density, duration, depthTest);
            }
#endif
        }
    }
}