using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Mathematics
{
    public struct Line2d
    {
        // навравление
        public float a, b;
        // удаленность от центра в квадрате через перпендикуляр
        public float c;

        public Line2d(Vector2 p1, Vector2 p2)
        {
            a = p2.y - p1.y;
            b = p1.x - p2.x;
            c = a * p1.x + b * p1.y;
        }

        public Vector2 normal => new Vector2(b, -a);
        public Vector2 origin => new Vector2(a, b).normalized * distanceToCenter;
        public float distanceToCenter => c / Mathf.Sqrt(a * a + b * b);


        public bool Intersection(in Line2d line, out Vector2 point)
        {
            return Shape2dExt.Intersection_LineVsLine(this, line, out point);
        }
    }
}