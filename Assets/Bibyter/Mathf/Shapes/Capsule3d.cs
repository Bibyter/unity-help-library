using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Mathematics
{
    public struct Capsule3d
    {
        public Vector3 from, to;
        public float radius;

        public Vector3 upPoint => to + (to - from).normalized * radius;
        public Vector3 downPoint => from + (from - to).normalized * radius;

        public Vector3 aabbMax => Max(from, to) + new Vector3(radius, radius, radius);
        public Vector3 aabbMin => Min(from, to) - new Vector3(radius, radius, radius);

        public Capsule3d(Vector3 from, Vector3 to, float radius)
        {
            this.from = from; this.to = to; this.radius = radius;
        }

        public bool ContainsPoint(in Vector3 point)
        {
            return ContainsPoint(from, to, point, radius);
        }

        public Vector3 ClosestPoint(in Vector3 point)
        {
            return ClosestPoint(from, to, point, radius);
        }

        public bool Raycast(in Vector3 origin, in Vector3 direction, out float distance)
        {
            distance = capIntersect(origin, direction, from, to, radius);
            return distance != nonCollision;
        }

        public static Vector3 GetClosestPointOnLine(in Vector3 a, in Vector3 b, in Vector3 point)
        {
            var ab = b - a;
            // Project c onto ab, computing parameterized position d(t)=a+ t*(b – a)
            var t = Vector3.Dot(point - a, ab) / Vector3.Dot(ab, ab);
            // If outside segment, clamp t (and therefore d) to the closest endpoint
            if (t < 0.0f) t = 0.0f;
            if (t > 1.0f) t = 1.0f;
            // Compute projected position from the clamped t
            return a + t * ab;
        }

        public static bool ContainsPoint(in Vector3 from, in Vector3 to, in Vector3 point, float radius)
        {
            return (GetClosestPointOnLine(from, to, point) - point).sqrMagnitude < radius * radius;
        }

        public static Vector3 ClosestPoint(in Vector3 from, in Vector3 to, in Vector3 point, float radius)
        {
            var closestPoint = GetClosestPointOnLine(from, to, point);
            var dir = point - closestPoint;

            return dir.sqrMagnitude > radius * radius ? closestPoint + dir.normalized * radius : point;
        }



        const float nonCollision = -1.0f;
        // capsule defined by extremes pa and pb, and radious ra
        // Note that only ONE of the two spherical caps is checked for intersections,
        // which is a nice optimization
        static float capIntersect(in Vector3 ro, in Vector3 rd, in Vector3 pa, in Vector3 pb, in float ra)
        {
            var ba = pb - pa;
            var oa = ro - pa;
            float baba = Vector3.Dot(ba, ba);
            float bard = Vector3.Dot(ba, rd);
            float baoa = Vector3.Dot(ba, oa);
            float rdoa = Vector3.Dot(rd, oa);
            float oaoa = Vector3.Dot(oa, oa);
            float a = baba - bard * bard;
            float b = baba * rdoa - baoa * bard;
            float c = baba * oaoa - baoa * baoa - ra * ra * baba;
            float h = b * b - a * c;

            if (h >= 0.0f && b < 0f)
            {
                float t = (-b - UnityEngine.Mathf.Sqrt(h)) / a;
                float y = baoa + t * bard;
                // body
                if (y > 0.0f && y < baba) return t;
                // caps
                var oc = y <= 0.0f ? oa : ro - pb;
                b = Vector3.Dot(rd, oc);
                c = Vector3.Dot(oc, oc) - ra * ra;
                h = b * b - c;
                if (h > 0.0f) return -b - UnityEngine.Mathf.Sqrt(h);
            }
            return nonCollision;
        }

        public static Vector3 Max(in Vector3 a, in Vector3 b)
        {
            return new Vector3(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y), Mathf.Max(a.z, b.z));
        }

        public static Vector3 Min(in Vector3 a, in Vector3 b)
        {
            return new Vector3(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Min(a.z, b.z));
        }

        public void GizmosDraw()
        {
            DrawWireCapsule(from, to, radius);
        }

        public static void DrawWireCapsule(Vector3 p1, Vector3 p2, float radius)
        {
#if UNITY_EDITOR
            // Special case when both points are in the same position
            if (p1 == p2)
            {
                // DrawWireSphere works only in gizmo methods
                Gizmos.DrawWireSphere(p1, radius);
                return;
            }
            using (new UnityEditor.Handles.DrawingScope(Gizmos.color, Gizmos.matrix))
            {
                Quaternion p1Rotation = Quaternion.LookRotation(p1 - p2);
                Quaternion p2Rotation = Quaternion.LookRotation(p2 - p1);
                // Check if capsule direction is collinear to Vector.up
                float c = Vector3.Dot((p1 - p2).normalized, Vector3.up);
                if (c == 1f || c == -1f)
                {
                    // Fix rotation
                    p2Rotation = Quaternion.Euler(p2Rotation.eulerAngles.x, p2Rotation.eulerAngles.y + 180f, p2Rotation.eulerAngles.z);
                }
                // First side
                UnityEditor.Handles.DrawWireArc(p1, p1Rotation * Vector3.left, p1Rotation * Vector3.down, 180f, radius);
                UnityEditor.Handles.DrawWireArc(p1, p1Rotation * Vector3.up, p1Rotation * Vector3.left, 180f, radius);
                UnityEditor.Handles.DrawWireDisc(p1, (p2 - p1).normalized, radius);
                // Second side
                UnityEditor.Handles.DrawWireArc(p2, p2Rotation * Vector3.left, p2Rotation * Vector3.down, 180f, radius);
                UnityEditor.Handles.DrawWireArc(p2, p2Rotation * Vector3.up, p2Rotation * Vector3.left, 180f, radius);
                UnityEditor.Handles.DrawWireDisc(p2, (p1 - p2).normalized, radius);
                // Lines
                UnityEditor.Handles.DrawLine(p1 + p1Rotation * Vector3.down * radius, p2 + p2Rotation * Vector3.down * radius);
                UnityEditor.Handles.DrawLine(p1 + p1Rotation * Vector3.left * radius, p2 + p2Rotation * Vector3.right * radius);
                UnityEditor.Handles.DrawLine(p1 + p1Rotation * Vector3.up * radius, p2 + p2Rotation * Vector3.up * radius);
                UnityEditor.Handles.DrawLine(p1 + p1Rotation * Vector3.right * radius, p2 + p2Rotation * Vector3.left * radius);
            }
#endif
        }
    }
}