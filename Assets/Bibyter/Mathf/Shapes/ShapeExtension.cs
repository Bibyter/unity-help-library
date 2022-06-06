using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Mathematics
{
    public static class ShapeExtension
    {

        // Computes the square distance between a point p and an AABB b
        public static float SqDistPointAABB(in Vector3 p, in Aabb3d b)
        {
            float sqDist = 0.0f;
            for (int i = 0; i < 3; i++)
            {
                // For each axis count any excess distance outside box extents
                float v = p[i];
                if (v < b.min[i]) sqDist += (b.min[i] - v) * (b.min[i] - v);
                if (v > b.max[i]) sqDist += (v - b.max[i]) * (v - b.max[i]);
            }
            return sqDist;
        }

        // Returns true if sphere s intersects AABB b, false otherwise
        public static bool HasCollision(in Sphere3d s, in Aabb3d b)
        {
            // Compute squared distance between sphere center and AABB
            float sqDist = SqDistPointAABB(s.center, b);
            // Sphere and AABB intersect if the (squared) distance
            // between them is less than the (squared) sphere radius
            return sqDist <= s.radius * s.radius;
        }

        public static bool HasCollision(in Capsule3d c, in Aabb3d b)
        {
            Vector3 capsuleClosestPoint = Capsule3d.GetClosestPointOnLine(c.from, c.to, b.center);

            float sqDist = SqDistPointAABB(capsuleClosestPoint, b);

            return sqDist <= c.radius * c.radius;
        }
    }
}