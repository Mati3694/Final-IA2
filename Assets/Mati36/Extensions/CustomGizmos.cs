

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    static public class CustomGizmos
    {
        //
        //GIZMOS
        //
        static public void DrawX(Vector3 position, float size)
        {
            Gizmos.DrawLine(position + new Vector3(-1, -1, 0) * size, position + new Vector3(1, 1, 0) * size);
            Gizmos.DrawLine(position + new Vector3(-1, 1, 0) * size, position + new Vector3(1, -1, 0) * size);
        }

        static public void DrawArrow(Vector3 from, Vector3 to)
        {
            DrawArrow(from, to, Vector3.up);
        }

        static public void DrawArrow(Vector3 from, Vector3 to, Vector3 arrowPlane)
        {
            var dir = to - from;
            Gizmos.DrawLine(from, to);
            Gizmos.DrawLine(to, to - Quaternion.AngleAxis(45, arrowPlane) * dir.normalized);
            Gizmos.DrawLine(to, to - Quaternion.AngleAxis(-45, arrowPlane) * dir.normalized);
        }

        static public void DrawCone(Vector3 position, Vector3 dir, float angle, float length)
        {
            DrawCone(position, dir, angle, length, Vector3.forward);
        }

        static public void DrawCone(Vector3 position, Vector3 dir, float angle, float length, Vector3 conePlane)
        {
            var first = Quaternion.AngleAxis(angle / 2f, conePlane) * dir;
            var second = Quaternion.AngleAxis(-angle / 2f, conePlane) * dir;
            Gizmos.DrawRay(position, first * length);
            Gizmos.DrawRay(position, second * length);
            DrawArc(position, position + first * length, position + second * length, conePlane);
        }

        static public void DrawArc(Vector3 center, Vector3 start, Vector3 end, float resolution = 10f)
        {
            DrawArc(center, start, end, Vector3.forward, resolution);
        }

        static public void DrawArc(Vector3 center, Vector3 start, Vector3 end, Vector3 arcPlane, float resolution = 0.5f)
        {
            Vector3 lastPoint = start;
            float angle = Vector3.SignedAngle(start - center, end - center, arcPlane);

            float segments = Mathf.Round(resolution * Mathf.Abs(angle));
            for (float i = 0; i <= segments; i++)
            {
                var next = center + (Quaternion.AngleAxis(angle / segments * i, arcPlane) * (start - center));
                Gizmos.DrawLine(lastPoint, next);
                lastPoint = next;
            }
        }

        static public void DebugDrawX(Vector3 position, float size, Color color)
        {
            Debug.DrawLine(position + new Vector3(-1, -1, 0) * size, position + new Vector3(1, 1, 0) * size, color);
            Debug.DrawLine(position + new Vector3(-1, 1, 0) * size, position + new Vector3(1, -1, 0) * size, color);
        }

    }
