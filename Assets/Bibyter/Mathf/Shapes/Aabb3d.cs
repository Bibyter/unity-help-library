using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Mathematics
{
    // Axis-aligned Bounding Boxes
    public struct Aabb3d
    {
        public Vector3 min, max;

        public Vector3 center => min + ((max - min) * 0.5f);

        public Aabb3d(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
        }

        public void DrawGizmos()
        {
            Gizmos.DrawWireCube(center, max - min);
        }

        public bool Raycast(in Vector3 rayOrigin, in Vector3 rayDirection, out float distance)
        {
            return Raycast(rayOrigin, rayDirection, this, out distance);
        }

        public static bool Raycast(in Vector3 rayOrigin, in Vector3 rayDirection, in Aabb3d aabb, out float distance)
        {
            float t1 = (aabb.min.x - rayOrigin.x) / rayDirection.x;
            float t2 = (aabb.max.x - rayOrigin.x) / rayDirection.x;
            float t3 = (aabb.min.y - rayOrigin.y) / rayDirection.y;
            float t4 = (aabb.max.y - rayOrigin.y) / rayDirection.y;
            float t5 = (aabb.min.z - rayOrigin.z) / rayDirection.z;
            float t6 = (aabb.max.z - rayOrigin.z) / rayDirection.z;

            float tmin = Mathf.Max(Mathf.Max(Mathf.Min(t1, t2), Mathf.Min(t3, t4)), Mathf.Min(t5, t6));
            float tmax = Mathf.Min(Mathf.Min(Mathf.Max(t1, t2), Mathf.Max(t3, t4)), Mathf.Max(t5, t6));

            // if tmax < 0, ray (line) is intersecting AABB, but whole AABB is behing us
            if (tmax < 0)
            {
                distance = -1f;
                return false;
            }

            // if tmin > tmax, ray doesn't intersect AABB
            if (tmin > tmax)
            {
                distance = -1;
                return false;
            }

            if (tmin < 0f)
            {
                distance = tmax;
                return true;
            }

            distance = tmin;
            return true;
        }
    }
}