using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Можно взять любой участок кривой и создать по нему vertext path крутя нормали куда захочется
/// </summary>

namespace Bibyter.BezieCurve
{
    public class Script : MonoBehaviour
    {
        public int steps = 10;
        public float favalue = 0f;

        [Tooltip("Значение производной")]
        public float derivativeValue = 0f;

        public float startAngle, endAngle;

        public float spacing = 0.1f, accuracy = 10f;
        public float sphereRadius = 0.1f;

        public bool drawCurve;

        public float normalWithUpValue = 0f;

        private void OnValidate()
        {
        }

        private void OnDrawGizmos()
        {
            if (transform.childCount < 4) return;

            var a = transform.GetChild(0).position;
            var b = transform.GetChild(1).position;
            var c = transform.GetChild(2).position;
            var d = transform.GetChild(3).position;

            var segment = new CurveSegment() { a = a, b = b, c = c, d = d };
            var alingningSegment = aligning(a, b, c, d);

            if (drawCurve)
            {
                for (int i = 1; i <= steps; i++)
                {
                    float inora = i / (float)steps;
                    float inorb = (i - 1) / (float)steps;

                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(CubeBezieLerp(a, b, c, d, inora), CubeBezieLerp(a, b, c, d, inorb));
                }
            }

            for (int i = 0; i <= steps; i++)
            {
                float inora = i / (float)steps;

                //Gizmos.color = Color.green;
                //Gizmos.DrawRay(CubeBezieLerp(a, b, c, d, inora), Derivative(a, b, c, d, inora).normalized * 0.5f);
                //Gizmos.color = Color.yellow;
                //Gizmos.DrawRay(CubeBezieLerp(a, b, c, d, inora), PerpendicularTo(Derivative(a, b, c, d, inora)).normalized * 0.5f);
                //Gizmos.color = Color.blue;
                //Gizmos.DrawRay(CubeBezieLerp(a, b, c, d, inora), Vector3.Cross(Derivative(a, b, c, d, inora), PerpendicularTo(Derivative(a, b, c, d, inora))).normalized * 0.5f);
            }

            {
                Gizmos.DrawRay(segment.Lerp(normalWithUpValue), segment.GetNormal(normalWithUpValue, transform.GetChild(0).up));
            }

            //var frames = PerpendicularFrames(segment, 10);
            //for (int i = 0; i < frames.Length; i++)
            //{
            //    Gizmos.color = Color.green;
            //    Gizmos.DrawRay(frames[i].Origin, frames[i].XAxis);
            //    Gizmos.color = Color.yellow;
            //    Gizmos.DrawRay(frames[i].Origin, frames[i].YAxis);
            //}


            //for (int i = 0; i < frames2.Count; i++)
            //{
            //    Gizmos.color = Color.green;
            //    Gizmos.DrawRay(frames2[i].o, Vector3.Cross(frames2[i].tangent, frames2[i].normal).normalized);
            //    Gizmos.color = Color.yellow;
            //    Gizmos.DrawRay(frames2[i].o, frames2[i].normal.normalized);
            //    //Gizmos.color = Color.red;
            //    //Gizmos.DrawRay(frames2[i].o, frames2[i].tangent.normalized);
            //}

            //for (int i = 1; i <= steps; i++)
            //{
            //    float inora = i / (float)steps;
            //    float inorb = (i - 1) / (float)steps;

            //    Gizmos.DrawLine(alingningSegment.Lerp(inora), alingningSegment.Lerp(inorb));
            //}


            //for (int i = 0; i < Tvalues.Length; i++)
            {
                //float z = 0.5f;
                //float t = z * favalue + z;
                //Gizmos.DrawWireSphere(CubeBezieLerp(a, b, c, d, t), 0.05f);
            }

            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(CubeBezieLerp(a, b, c, d, derivativeValue), Derivative(a, b, c, d, derivativeValue).normalized * 0.5f);
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(CubeBezieLerp(a, b, c, d, derivativeValue), PerpendicularTo(Derivative(a, b, c, d, derivativeValue)).normalized * 0.5f);

                //Gizmos.DrawRay(CubeBezieLerp(a, b, c, d, derivativeValue), EvaluateCurveSecondDerivative(a, b, c, d, derivativeValue) * 0.5f);
            }

            UnityEditor.Handles.Label(transform.position, alingningSegment.ComputeLenth().ToString());
            UnityEditor.Handles.Label(transform.position + new Vector3(0f, -0.2f, 0f), $"True Length {ComputeLength(a, b, c, d)}");

            spacing = Mathf.Max(0.1f, spacing);

            var path = segment.CreateVertexPath(transform.GetChild(0).up, spacing, accuracy, startAngle, endAngle);

            for (int i = 0; i < path.Count; i++)
            {
                var p = path[i].point;
                //Gizmos.DrawWireSphere(p, sphereRadius);
                Gizmos.color = Color.green;
                Gizmos.DrawRay(p, path[i].tangent * sphereRadius);
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(p, path[i].normal * sphereRadius);
                Gizmos.color = Color.red;
                Gizmos.DrawRay(p, Vector3.Cross(path[i].normal, path[i].tangent) * sphereRadius);
            }
        }

        [ContextMenu("Test")]
        void test()
        {
            var a = transform.GetChild(0).position;
            var b = transform.GetChild(1).position;
            var c = transform.GetChild(2).position;
            var d = transform.GetChild(3).position;

            var stopwatch = new System.Diagnostics.Stopwatch();

            stopwatch.Start();

            for (int i = 1; i <= steps; i++)
            {
                float inora = i / (float)steps;
                float inorb = (i - 1) / (float)steps;

                var v0 = CubeBezieLerp(a, b, c, d, inora);
                var v1 = CubeBezieLerp(a, b, c, d, inorb);
            }

            stopwatch.Stop();
            print(stopwatch.ElapsedTicks);
            stopwatch.Reset();
            stopwatch.Start();

            for (int i = 1; i <= steps; i++)
            {
                float inora = i / (float)steps;
                float inorb = (i - 1) / (float)steps;

                var v0 = CubeLerp(a, b, c, d, inora);
                var v1 = CubeLerp(a, b, c, d, inorb);
            }

            stopwatch.Stop();
            print(stopwatch.ElapsedTicks);
        }

        public static Vector3 CubeLerp(in Vector3 a, in Vector3 b, in Vector3 c, in Vector3 d, float t)
        {
            Vector3 p0 = QuadLerp(a, b, c, t);
            Vector3 p1 = QuadLerp(b, c, d, t);
            return Lerp(p0, p1, t);
        }

        public static Vector2 QuadLerp(in Vector3 a, in Vector3 b, in Vector3 c, float t)
        {
            var p0 = Lerp(a, b, t);
            var p1 = Lerp(b, c, t);
            return Lerp(p0, p1, t);
        }

        public static Vector3 Lerp(in Vector3 a, in Vector3 b, float t)
        {
            return a + (b - a) * t;
        }


        /// <summary>
        /// (1-t)^3A + 3(1-t)^2tB + 3(1 - t)t^2C + t^3D Функция полинома 3-го порядка
        /// </summary>
        static Vector3 CubeBezieLerp(in Vector3 a, in Vector3 b, in Vector3 c, in Vector3 d, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;
            float mt = 1f - t;
            float mt2 = mt * mt;
            float mt3 = mt2 * mt;
            return (mt3 * a) + (3f * mt2 * t * b) + (3f * mt * t2 * c) + (t3 * d);
        }

        public static float ComputeLength(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            Vector3 lastPoint = a;
            float sum = 0f;

            const int steps = 10;
            for (int i = 1; i <= steps; i++)
            {
                float inor = i / (float)steps;
                Vector3 nextPoint = CubeBezieLerp(a, b, c, d, inor);

                sum += Vector3.Distance(lastPoint, nextPoint);
                lastPoint = nextPoint;
            }

            return sum;
        }

        /// <summary>
        /// Производная - это скорость изменения y в определенном положении x графика за единицу x.
        /// По итогу получается касательная (tangent) в векторном просранстве
        /// </summary>
        /// <returns>Производная Безье</returns>
        static Vector3 Derivative(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t) 
        {
            float mt = 1f - t;
            float aa = mt * mt;
            float bb = 2f * mt * t;
            float cc = t * t;

            Vector3 d0 = 3f * (b - a);
            Vector3 d1 = 3f * (c - b);
            Vector3 d2 = 3f * (d - c);

            return new Vector3(
                aa * d0.x + bb * d1.x + cc * d2.x,
                aa * d0.y + bb * d1.y + cc * d2.y,
                aa * d0.z + bb * d1.z + cc * d2.z);
        }

        public static Vector3 EvaluateCurveSecondDerivative(Vector3 a1, Vector3 c1, Vector3 c2, Vector3 a2, float t)
        {
            return 6 * (1 - t) * (c2 - 2 * c1 + a1) + 6 * t * (a2 - 2 * c2 + c1);
        }

        // Crude, but fast estimation of curve length.
        public static float EstimateCurveLength(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float controlNetLength = (p0 - p1).magnitude + (p1 - p2).magnitude + (p2 - p3).magnitude;
            float estimatedCurveLength = (p0 - p3).magnitude + controlNetLength / 2f;
            return estimatedCurveLength;
        }

        public struct SplitItem
        {
            public Vector3 point;
            public Vector3 tangent;
            public Vector3 normal;
        }

        /// <summary>
        /// Просто выбираем точки на кривой и линейно заполняем точками между ними с фикс шагом
        /// </summary>
        public static List<SplitItem> SplitBezierPathEvenly(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float spacing, float accuracy = 1f)
        {
            accuracy = Mathf.Max(0.02f, accuracy);
            
            var vertices = new List<SplitItem>();
            vertices.Add(new SplitItem() { point = a, tangent = Derivative(a, b, c, d, 0f).normalized });

            Vector3 prevPointOnPath = a;
            Vector3 lastAddedPoint = a;

            float currentPathLength = 0;
            float dstSinceLastVertex = 0;

            // Go through all segments and split up into vertices
            {
                float estimatedSegmentLength = EstimateCurveLength(a, b, c, d);
                int divisions = Mathf.CeilToInt(estimatedSegmentLength * accuracy);
                float increment = 1f / divisions;

                for (float t = increment; t <= 1; t += increment)
                {
                    bool isLastPointOnPath = false;
                    if (isLastPointOnPath)
                    {
                        t = 1;
                    }

                    Vector3 pointOnPath = CubeBezieLerp(a, b, c, d, t);
                    dstSinceLastVertex += (pointOnPath - prevPointOnPath).magnitude;

                    // If vertices are now too far apart, go back by amount we overshot by
                    if (dstSinceLastVertex > spacing)
                    {
                        float overshootDst = dstSinceLastVertex - spacing;
                        pointOnPath += (prevPointOnPath - pointOnPath).normalized * overshootDst;
                        t -= increment;
                    }

                    if (dstSinceLastVertex >= spacing || isLastPointOnPath)
                    {
                        currentPathLength += (lastAddedPoint - pointOnPath).magnitude;
                        vertices.Add(new SplitItem() { point = pointOnPath, tangent = Derivative(a, b, c, d, t).normalized } );
                        dstSinceLastVertex = 0;
                        lastAddedPoint = pointOnPath;
                    }

                    prevPointOnPath = pointOnPath;
                }
            }
            return vertices;
        }

        public struct CurveSegment
        {
            public Vector3 a, b, c, d;

            public Vector3 Lerp(float t)
            {
                return CubeBezieLerp(a, b, c, d, t);
            }

            public Vector3 Tangent(float t)
            {
                return Script.Derivative(a, b, c, d, t);
            }

            public float ComputeLenth()
            {
                return ComputeLength(a, b, c, d);
            }

            public List<SplitItem> CreateVertexPath(Vector3 startNormal, float spacing = 0.1f, float accuracy = 1f, float startAngle = 0f, float endAngle = 0f)
            {
                var path = SplitBezierPathEvenly(a, b, c, d, spacing, accuracy);
                generateRMFrames(path, startNormal);
                RotateNormals(path, startAngle, endAngle);
                return path;
            }

            public Vector3 GetNormal(float t, Vector3 up)
            {
                var tangent = Tangent(t);
                var binormal = Vector3.Cross(tangent, up);
                var normal = Vector3.Cross(tangent, binormal);
                return normal;
            }
        }


        /// <summary>
        /// Выровнить кривую в 0 координаты
        /// </summary>
        CurveSegment aligning(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            float angle = Mathf.Atan2(d.y, d.x);

            a = Rotate(a, -angle);
            b = Rotate(b, -angle);
            c = Rotate(c, -angle);
            d = Rotate(d, -angle);

            b -= a;
            c -= a;
            d -= a;
            a = new Vector3(0f, 0f, 0f);

            return new CurveSegment { a = a, b = b, c = c, d = d };
        }

        Vector2 Rotate(Vector2 v, float angle)
        {
            var r = v;
            r.x = v.x * Mathf.Cos(angle) - v.y * Mathf.Sin(angle);
            r.y = v.x * Mathf.Sin(angle) + v.y * Mathf.Cos(angle);
            return r;
        }
        public struct Plane
        {
            public Vector3 Origin, XAxis, YAxis;

            public Plane(Vector3 Origin, Vector3 XAxis, Vector3 YAxis)
            {
                this.Origin = Origin;
                this.XAxis = XAxis;
                this.YAxis = YAxis;
            }
        }

        public struct Frame2
        {
            public Vector3 o, tangent, normal;
        }

        /// <summary>
        /// Rotation Minimising Frame.
        /// Разбиение пути на фреймы и минимализация вращения нормалей на основании предыдущего кадра.
        /// https://pomax.github.io/bezierinfo/#pointvectors3d
        /// </summary>
        List<Frame2> generateRMFrames(CurveSegment curve, int steps, Vector3 r)
        {
            var frames = new List<Frame2>();

            var f = new Frame2();
            f.o = curve.Lerp(0f);
            f.tangent = curve.Tangent(0f);
            Vector3 lastRotationAxis = Vector3.Cross(f.tangent, r);
            f.normal = Vector3.Cross(lastRotationAxis, f.tangent);

            frames.Add(f);

            for (int i = 0; i < steps; i++)
            {
                var x0 = frames[frames.Count - 1];

                //float t0 = i / (float)steps;
                float t1 = (i + 1) / (float)steps;

                var x1 = new Frame2() { o = curve.Lerp(t1), tangent = curve.Tangent(t1) };

                var v1 = x1.o - x0.o;
                var c1 = Vector3.Dot(v1, v1);
                var riL = lastRotationAxis - (v1 * 2f / c1 * Vector3.Dot(v1, lastRotationAxis));
                var tiL = x0.tangent - (v1 * 2f / c1 * Vector3.Dot(v1, x0.tangent));

                var v2 = x1.tangent - tiL;
                var c2 = Vector3.Dot(v2, v2);

                Vector3 finalRotationAxis = riL - (v2 * 2f / c2 * Vector3.Dot(v2, riL));
                x1.normal = Vector3.Cross(finalRotationAxis, x1.tangent);
                frames.Add(x1);
                lastRotationAxis = finalRotationAxis;
            }

            return frames;
        }

        static void generateRMFrames(List<SplitItem> list, Vector3 r)
        {
            var firstItem = list[0];
            Vector3 lastRotationAxis = Vector3.Cross(firstItem.tangent, r);
            firstItem.normal = Vector3.Cross(lastRotationAxis, firstItem.tangent).normalized;
            list[0] = firstItem;

            for (int i = 1; i < list.Count; i++)
            {
                var x0 = list[i - 1];


                var x1 = list[i];

                var v1 = x1.point - x0.point;
                var c1 = Vector3.Dot(v1, v1);
                var riL = lastRotationAxis - (v1 * 2f / c1 * Vector3.Dot(v1, lastRotationAxis));
                var tiL = x0.tangent - (v1 * 2f / c1 * Vector3.Dot(v1, x0.tangent));

                var v2 = x1.tangent - tiL;
                var c2 = Vector3.Dot(v2, v2);

                Vector3 finalRotationAxis = riL - (v2 * 2f / c2 * Vector3.Dot(v2, riL));
                x1.normal = Vector3.Cross(finalRotationAxis, x1.tangent);
                x1.normal.Normalize();

                list[i] = x1;
                lastRotationAxis = finalRotationAxis;
            }
        }

        public static void RotateNormals(List<SplitItem> list, float startAngle, float endAngle)
        {
            for (int i = 0; i < list.Count; i++)
            {
                float inora = i / (float)(list.Count - 1);
                float angle = Mathf.Lerp(startAngle, endAngle, inora);

                var f = list[i];

                var r = Quaternion.AngleAxis(angle, f.tangent);
                f.normal = r * f.normal;

                list[i] = f;
            }
        }

        public static Vector3 PerpendicularTo(Vector3 vector)
        {
            Vector3 vectorComponents = vector;
            Vector3 tempVector = vector;

            int i, j, k;
            float a, b;

            if (Mathf.Abs(vectorComponents[1]) > Mathf.Abs(vectorComponents[0]))
            {
                if (Mathf.Abs(vectorComponents[2]) > Mathf.Abs(vectorComponents[1]))
                {
                    // |v.z| > |v.y| > |v.x|
                    i = 2;
                    j = 1;
                    k = 0;
                    a = vectorComponents[2];
                    b = -vectorComponents[1];
                }
                else if (Mathf.Abs(vectorComponents[2]) > Mathf.Abs(vectorComponents[0]))
                {
                    // |v.y| >= |v.z| >= |v.x|
                    i = 1;
                    j = 2;
                    k = 0;
                    a = vectorComponents[1];
                    b = -vectorComponents[2];
                }
                else
                {
                    // |v.y| > |v.x| > |v.z|
                    i = 1;
                    j = 0;
                    k = 2;
                    a = vectorComponents[1];
                    b = -vectorComponents[0];
                }
            }
            else if (Mathf.Abs(vectorComponents[2]) > Mathf.Abs(vectorComponents[0]))
            {
                // |v.z| > |v.x| >= |v.y|
                i = 2;
                j = 0;
                k = 1;
                a = vectorComponents[2];
                b = -vectorComponents[0];
            }
            else if (Mathf.Abs(vectorComponents[2]) > Mathf.Abs(vectorComponents[1]))
            {
                // |v.x| >= |v.z| > |v.y|
                i = 0;
                j = 2;
                k = 1;
                a = vectorComponents[0];
                b = -vectorComponents[2];
            }
            else
            {
                // |v.x| >= |v.y| >= |v.z|
                i = 0;
                j = 1;
                k = 2;
                a = vectorComponents[0];
                b = -vectorComponents[1];
            }

            tempVector[i] = b;
            tempVector[j] = a;
            tempVector[k] = 0.0f;

            return tempVector;
        }
    }

    public sealed class BezieCurve
    {
        Vector3[] _points;
        float[] _angles;

        public int segmentCount => _points.Length / 4;

        public Script.CurveSegment GetSegment(int i)
        {
            return new Script.CurveSegment()
            {
                a = _points[i * 4 + 0],
                b = _points[i * 4 + 1],
                c = _points[i * 4 + 2],
                d = _points[i * 4 + 3],
            };
        }
    }

    public sealed class VertextPath
    {
        public struct Vertex
        {
            public Vector3 normal;
            public Vector3 point;
            public Vector3 tarnget;
            public float distance;
        }

        Vertex[] _vertices;
        int _verticesCount;

        public VertextPath()
        {
            _vertices = new Vertex[32];
            _verticesCount = 0;
        }

        public float length => 0f;

        public int vertexCount => 0;

        public Vertex GetVertex(int i)
        {
            return new Vertex();
        }

        public Vertex GetVertexAtDistance(float distance)
        {
            var id = GetVertextRangeWithDistance(distance);

            var v0 = new Vertex();
            var v1 = new Vertex();

            var r = new Vertex();
            r.point = Vector3.Lerp(v0.point, v1.point, 0.5f);
            r.normal = Vector3.Slerp(v0.normal, v1.normal, 0.5f);
            r.tarnget = Vector3.Slerp(v0.tarnget, v1.tarnget, 0.5f);

            return r;
        }

        struct Range
        {
            public int leftIndex, rightIndex;
            public float percent;
        }

        private Range GetVertextRangeWithDistance(float distance)
        {
            int leftIndex = 0;
            int rightIndex = _verticesCount - 1;
            int midIndex = _verticesCount / 2;

            while (true)
            {
                if (distance <= _vertices[midIndex].distance)
                {
                    leftIndex = midIndex;
                }
                else
                {
                    rightIndex = midIndex;
                }

                midIndex = (leftIndex + rightIndex) / 2;

                if (leftIndex - rightIndex <= 1)
                {
                    break;
                }
            }

            return new Range() { 
                percent = Mathf.InverseLerp(_vertices[leftIndex].distance, _vertices[rightIndex].distance, distance),
                rightIndex = rightIndex, 
                leftIndex = leftIndex
            };
        }
    }
}