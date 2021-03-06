using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Bibyter.Curves.WheelFrictionCurve
{
    public sealed class Script : MonoBehaviour
    {
        [Range(-10f, 10f)] public float extremumSlip = 3f; //Extremum point slip (default 3).
        public float extremumValue = 6000f; //Force at the extremum slip (default 6000).
        [Range(-10f, 10f)] public float asymptoteSlip = 4f; //Asymptote point slip (default 4).
        public float asymptoteValue = 5500f; //Force at the asymptote slip (default 5500).
        public float stiffness = 1f; //Multiplier for the extremumValue and asymptoteValue values (default 1).

        public float slipStart = 0f;
        public float slipsEnd = 10f;

        public Vector2 curveScale = new Vector2(1f, 0.001f);

        WheelFrictionCurveSource _wheelFrictionCurveSource = new WheelFrictionCurveSource();

        private void OnDrawGizmos()
        {
            if (slipStart >= slipsEnd)
            {
                slipStart = slipsEnd - 1f;
            }

            _wheelFrictionCurveSource.ExtremumSlip = extremumSlip;
            _wheelFrictionCurveSource.ExtremumValue = extremumValue;
            _wheelFrictionCurveSource.AsymptoteSlip = asymptoteSlip;
            _wheelFrictionCurveSource.AsymptoteValue = asymptoteValue;
            _wheelFrictionCurveSource.Stiffness = stiffness;

            var slipStep = Mathf.Abs(slipStart - slipsEnd) * 0.01f;

            for (float slip = slipStart; slip < slipsEnd; slip += slipStep)
            {
                var slipa = slip;
                var slipb = slip + slipStep;

                var forcea = _wheelFrictionCurveSource.Evaluate(slipa);
                var forceb = _wheelFrictionCurveSource.Evaluate(slipb);

                Gizmos.DrawLine(new Vector3(slipa * curveScale.x, forcea * curveScale.y, 0f), new Vector3(slipb * curveScale.x, forceb * curveScale.y));
            }
        }
    }

    public class WheelFrictionCurveSource
    {
        private struct WheelFrictionCurvePoint
        {
            public float TValue;
            public Vector2 SlipForcePoint;
        }
        private float m_extremumSlip; //Extremum point slip (default 3).
        private float m_extremumValue; //Force at the extremum slip (default 6000).
        private float m_asymptoteSlip; //Asymptote point slip (default 4).
        private float m_asymptoteValue; //Force at the asymptote slip (default 5500).
        private float m_stiffness; //Multiplier for the extremumValue and asymptoteValue values (default 1).

        private int m_arraySize;
        private WheelFrictionCurvePoint[] m_extremePoints;
        private WheelFrictionCurvePoint[] m_asymptotePoints;

        public float ExtremumSlip
        {
            get
            {
                return m_extremumSlip;
            }
            set
            {
                m_extremumSlip = value;
                UpdateArrays();
            }
        }
        public float ExtremumValue
        {
            get
            {
                return m_extremumValue;
            }
            set
            {
                m_extremumValue = value;
                UpdateArrays();
            }
        }
        public float AsymptoteSlip
        {
            get
            {
                return m_asymptoteSlip;
            }
            set
            {
                m_asymptoteSlip = value;
                UpdateArrays();
            }
        }
        public float AsymptoteValue
        {
            get
            {
                return m_asymptoteValue;
            }
            set
            {
                m_asymptoteValue = value;
                UpdateArrays();
            }
        }
        public float Stiffness
        {
            get
            {
                return m_stiffness;
            }
            set
            {
                m_stiffness = value;
            }
        }

        public WheelFrictionCurveSource()
        {
            m_extremumSlip = 3;
            m_extremumValue = 6000;
            m_asymptoteSlip = 4;
            m_asymptoteValue = 5500;
            m_stiffness = 4;

            m_arraySize = 50;
            m_extremePoints = new WheelFrictionCurvePoint[m_arraySize];
            m_asymptotePoints = new WheelFrictionCurvePoint[m_arraySize];

            UpdateArrays();
        }

        private void UpdateArrays()
        {
            //Generate the arrays
            for (int i = 0; i < m_arraySize; ++i)
            {
                m_extremePoints[i].TValue = (float)i / (float)m_arraySize;
                m_extremePoints[i].SlipForcePoint = Hermite(
                        (float)i / (float)m_arraySize,
                        Vector2.zero,
                        new Vector2(m_extremumSlip, m_extremumValue),
                        Vector2.zero,
                        new Vector2(m_extremumSlip * 0.5f + 1, 0)
                    );

                m_asymptotePoints[i].TValue = (float)i / (float)m_arraySize;
                m_asymptotePoints[i].SlipForcePoint = Hermite(
                    (float)i / (float)m_arraySize,
                        new Vector2(m_extremumSlip, m_extremumValue),
                        new Vector2(m_asymptoteSlip, m_asymptoteValue),
                        new Vector2((m_asymptoteSlip - m_extremumSlip) * 0.5f + 1, 0),
                        new Vector2((m_asymptoteSlip - m_extremumSlip) * 0.5f + 1, 0)
                    );
            }
        }

        public float Evaluate(float slip)
        {
            //The slip value must be positive.
            slip = Mathf.Abs(slip);

            if (slip < m_extremumSlip)
            {
                return Evaluate(slip, m_extremePoints) * m_stiffness;
            }
            else if (slip < m_asymptoteSlip)
            {
                return Evaluate(slip, m_asymptotePoints) * m_stiffness;
            }
            else
            {
                return m_asymptoteValue * m_stiffness;
            }
        }

        private float Evaluate(float slip, WheelFrictionCurvePoint[] curvePoints)
        {
            int top = m_arraySize - 1;
            int bottom = 0;
            int index = (int)((top + bottom) * 0.5f);

            WheelFrictionCurvePoint result = curvePoints[index];

            //Binary search the curve to find the corresponding t value for the given slip
            while ((top != bottom && top - bottom > 1))
            {
                if (result.SlipForcePoint.x <= slip)
                {
                    bottom = index;
                }
                else if (result.SlipForcePoint.x >= slip)
                {
                    top = index;
                }

                index = (int)((top + bottom) * 0.5f);
                result = curvePoints[index];
            }

            float slip1 = curvePoints[bottom].SlipForcePoint.x;
            float slip2 = curvePoints[top].SlipForcePoint.x;
            float force1 = curvePoints[bottom].SlipForcePoint.y;
            float force2 = curvePoints[top].SlipForcePoint.y;

            float slipFraction = (slip - slip1) / (slip2 - slip1);

            return force1 * (1 - slipFraction) + force2 * (slipFraction); ;
        }

        private Vector2 Hermite(float t, Vector2 p0, Vector2 p1, Vector2 m0, Vector2 m1)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            return
                (2 * t3 - 3 * t2 + 1) * p0 +
                (t3 - 2 * t2 + t) * m0 +
                (-2 * t3 + 3 * t2) * p1 +
                (t3 - t2) * m1;
        }
    }
}