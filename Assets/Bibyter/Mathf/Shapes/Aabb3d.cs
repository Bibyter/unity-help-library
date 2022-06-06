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
    }
}