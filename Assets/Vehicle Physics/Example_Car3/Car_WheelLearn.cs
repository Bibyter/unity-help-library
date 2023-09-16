using Bibyter.Mathematics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Example_Car3
{
    public struct WheelFrictionCurve
    { 

    }

    public class Car_WheelLearn : MonoBehaviour
    {
        public Vector3 velocity;

        // Угол бокового скольжения (бета)
        // это разница между ориентацией машины и направлением ее движения
        public float lateralSlipAngle;

        public float steerRadius = 10f;
        public float rearAxleDistance = 1f;

        public float angleRear;

        private void OnValidate()
        {
            lateralSlipAngle = -Mathf.Atan2(velocity.z, velocity.x) + (Mathf.PI * 0.5f);

            angleRear = Mathf.Atan((velocity.x - rearAxleDistance) / Mathf.Abs(velocity.z));
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawRay(transform.position, velocity);

            lateralSlipAngle = -Mathf.Atan2(velocity.z, velocity.x) + (Mathf.PI * 0.5f);

            var d = Shape2dExt.RadToDirection(lateralSlipAngle);
            Gizmos.DrawRay(transform.position + Vector3.up, new Vector3(d.x, 0f, d.y));

            Gizmos.DrawRay(transform.position + Vector3.back * rearAxleDistance + new Vector3(0f, 0.1f, 0f), Vector3.right * steerRadius);
            Gizmos.DrawLine(transform.position + Vector3.back * rearAxleDistance + Vector3.right, transform.position + Vector3.back * rearAxleDistance + Vector3.left);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            lateralSlipAngle = -(Mathf.Atan2(velocity.z, velocity.x) + (Mathf.PI * 0.5f));

            // Боковая сила четырех шин имеет два результата: результирующую угловую силу и вращающий момент вокруг оси, направленной вверх
        }
    }
}