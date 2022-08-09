using UnityEngine;

namespace Bibyter.Mathematics
{
    public static class Shape2dExt
    {
        readonly static Vector2 zero = new Vector2(0f, 0f);

        public static bool Raycast_Circle(Vector2 rayOrigin, Vector2 rayDirection, Vector2 circleCenter, float circleRadius, out Vector2 pointA)
        {
            pointA = zero;

            var d = rayDirection;
            var f = rayOrigin - circleCenter;

            float a = Vector2.Dot(d, d);
            float b = 2f * Vector2.Dot(f, d);
            float c = Vector2.Dot(f, f) - circleRadius * circleRadius;

            float discriminant = b * b - 4f * a * c;

            if (discriminant < 0f)
            {
                // no intersection
                return false;
            }
            else
            {
                // ray didn't totally miss sphere,
                // so there is a solution to
                // the equation.
                discriminant = Mathf.Sqrt(discriminant);

                // either solution may be on or off the ray so need to test both
                // t1 is always the smaller value, because BOTH discriminant and
                // a are nonnegative.
                float t1 = (-b - discriminant) / (2 * a);
                float t2 = (-b + discriminant) / (2 * a);

                // 3x HIT cases:
                //          -o->             --|-->  |            |  --|->
                // Impale(t1 hit,t2 hit), Poke(t1 hit,t2>1), ExitWound(t1<0, t2 hit), 

                // 3x MISS cases:
                //       ->  o                     o ->              | -> |
                // FallShort (t1>1,t2>1), Past (t1<0,t2<0), CompletelyInside(t1<0, t2>1)

                if (t1 >= 0f && t1 <= 1f)
                {
                    // t1 is the intersection, and it's closer than t2
                    // (since t1 uses -b - discriminant)
                    // Impale, Poke
                    pointA = rayOrigin + t1 * d;
                    return true;
                }

                // here t1 didn't intersect so we are either started
                // inside the sphere or completely past it
                if (t2 >= 0f && t2 <= 1f)
                {
                    // ExitWound
                    pointA = rayOrigin + t2 * d;
                    return true;
                }

                // no intn: FallShort, Past, CompletelyInside
                return false;
            }
        }

        public static bool Intersection_LineSegmentVsCircle(Vector2 lineSegmentStart, Vector2 lineSegmentEnd, Vector2 circleCenter, float circleRadius, out Vector2 pointA, out bool hasPointA, out Vector2 pointB, out bool hasPointB)
        {
            pointA = pointB = zero;
            hasPointA = hasPointB = false;

            var d = lineSegmentEnd - lineSegmentStart;
            var f = lineSegmentStart - circleCenter;

            float a = Vector2.Dot(d, d);
            float b = 2f * Vector2.Dot(f, d);
            float c = Vector2.Dot(f, f) - circleRadius * circleRadius;

            float discriminant = b * b - 4f * a * c;

            if (discriminant < 0f)
            {
                // no intersection

                return false;
            }
            else
            {
                // ray didn't totally miss sphere,
                // so there is a solution to
                // the equation.

                discriminant = Mathf.Sqrt(discriminant);

                // either solution may be on or off the ray so need to test both
                // t1 is always the smaller value, because BOTH discriminant and
                // a are nonnegative.
                float t1 = (-b - discriminant) / (2 * a);
                float t2 = (-b + discriminant) / (2 * a);

                // 3x HIT cases:
                //          -o->             --|-->  |            |  --|->
                // Impale(t1 hit,t2 hit), Poke(t1 hit,t2>1), ExitWound(t1<0, t2 hit), 

                // 3x MISS cases:
                //       ->  o                     o ->              | -> |
                // FallShort (t1>1,t2>1), Past (t1<0,t2<0), CompletelyInside(t1<0, t2>1)

                if (t1 >= 0f && t1 <= 1f)
                {
                    // t1 is the intersection, and it's closer than t2
                    // (since t1 uses -b - discriminant)
                    // Impale, Poke
                    pointA = lineSegmentStart + t1 * d;
                    hasPointA = true;
                }

                // here t1 didn't intersect so we are either started
                // inside the sphere or completely past it
                if (t2 >= 0f && t2 <= 1f)
                {
                    // ExitWound
                    pointB = lineSegmentStart + t2 * d;
                    hasPointB = true;
                }

                // no intn: FallShort, Past, CompletelyInside
                return hasPointA || hasPointB;
            }
        }

        public static bool Intersection_LineVsCircle(Vector2 linePointA, Vector2 linePointB, Vector2 circleCenter, float circleRadius, out Vector2 pointA, out Vector2 pointB)
        {
            var d = linePointB - linePointA;
            var f = linePointA - circleCenter;

            float a = Vector2.Dot(d, d);
            float b = 2f * Vector2.Dot(f, d);
            float c = Vector2.Dot(f, f) - circleRadius * circleRadius;

            float discriminant = b * b - 4f * a * c;

            if (discriminant < 0f)
            {
                // no intersection
                pointA = pointB = zero;
                return false;
            }
            else
            {
                // ray didn't totally miss sphere,
                // so there is a solution to
                // the equation.

                discriminant = Mathf.Sqrt(discriminant);

                // either solution may be on or off the ray so need to test both
                // t1 is always the smaller value, because BOTH discriminant and
                // a are nonnegative.
                float t1 = (-b - discriminant) / (2f * a);
                float t2 = (-b + discriminant) / (2f * a);

                // 3x HIT cases:
                //          -o->             --|-->  |            |  --|->
                // Impale(t1 hit,t2 hit), Poke(t1 hit,t2>1), ExitWound(t1<0, t2 hit), 

                // 3x MISS cases:
                //       ->  o                     o ->              | -> |
                // FallShort (t1>1,t2>1), Past (t1<0,t2<0), CompletelyInside(t1<0, t2>1)

                // t1 is the intersection, and it's closer than t2
                // (since t1 uses -b - discriminant)
                // Impale, Poke
                pointA = linePointA + t1 * d;

                // here t1 didn't intersect so we are either started
                // inside the sphere or completely past it
                // ExitWound
                pointB = linePointA + t2 * d;

                // no intn: FallShort, Past, CompletelyInside
                return true;
            }
        }

        public static bool Intersection_LineVsLine(in Line2d l1, in Line2d l2, out Vector2 point)
        {
            float det = l1.a * l2.b - l2.a * l1.b;

            //If lines are parallel, the result will be (NaN, NaN).
            point = det == 0f ? zero :
                new Vector2((l2.b * l1.c - l1.b * l2.c) / det, (l1.a * l2.c - l2.a * l1.c) / det);

            return det != 0f;
        }

        public static Vector2 RadToDirection(float rad)
        {
            return new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
        }

        public static Vector2 RotatePoint(Vector2 point, float angle)
        {
            Vector2 rotated_point;
            rotated_point.x = point.x * Mathf.Cos(angle) - point.y * Mathf.Sin(angle);
            rotated_point.y = point.x * Mathf.Sin(angle) + point.y * Mathf.Cos(angle);
            return rotated_point;
        }
    }
}