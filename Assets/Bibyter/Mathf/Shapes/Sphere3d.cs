using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Mathematics
{
    public struct Sphere3d
    {
        static readonly Vector2 nonCollision = new Vector2(-1f, -1f);

        public Vector3 center;
        public float radius;

        public Sphere3d(Vector3 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }

        public bool Raycast(in Vector3 origin, in Vector3 direction, out Vector2 distance)
        {
            return sphIntersect(origin, direction, center, radius, out distance);
        }

        public static bool sphIntersect(in Vector3 ro, in Vector3 rd, in Vector3 ce, float ra, out Vector2 distance)
        {
            Vector3 oc = ro - ce;
            float b = Vector3.Dot(oc, rd);
            float c = Vector3.Dot(oc, oc) - ra * ra;
            float h = b * b - c;

            if (h < 0f || b > 0f)
            {
                distance = nonCollision;
                return false;
            }

            h = Mathf.Sqrt(h);
            distance = new Vector2(-b - h, -b + h);
            return true;
        }

        public void DismosDraw()
        {
            Gizmos.DrawWireSphere(center, radius);
        }
    }
}