using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Mathematics
{
    public struct Line2dSegment
    {
        readonly static Vector2 zero = new Vector2(0f, 0f);

        public static bool Intersection(in Vector2 start1, in Vector2 end1, in Vector2 start2, in Vector2 end2, out Vector2 out_intersection)
        {
            var dir1 = end1 - start1;
            var dir2 = end2 - start2;

            //считаем уравнения прямых проходящих через отрезки
            float a1 = -dir1.y;
            float b1 = +dir1.x;
            float d1 = -(a1 * start1.x + b1 * start1.y);

            float a2 = -dir2.y;
            float b2 = +dir2.x;
            float d2 = -(a2 * start2.x + b2 * start2.y);

            //подставляем концы отрезков, для выяснения в каких полуплоскотях они
            float seg1_line2_start = a2 * start1.x + b2 * start1.y + d2;
            float seg1_line2_end = a2 * end1.x + b2 * end1.y + d2;

            float seg2_line1_start = a1 * start2.x + b1 * start2.y + d1;
            float seg2_line1_end = a1 * end2.x + b1 * end2.y + d1;

            //если концы одного отрезка имеют один знак, значит он в одной полуплоскости и пересечения нет.

            bool result = !(seg1_line2_start * seg1_line2_end >= 0f || seg2_line1_start * seg2_line1_end >= 0f);

            out_intersection = result ? start1 + (seg1_line2_start / (seg1_line2_start - seg1_line2_end)) * dir1 : zero;

            return result;
        }
    }
}