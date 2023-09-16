using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Example_Car4
{
    public sealed class Car : MonoBehaviour
    {
        public float distanceFrontAxle = 1f;
        public float distanceRearAxle = 1.5f;
        [Range(-0.7f, 0.7f)] public float steerAngle;
        public float mass = 1300f;
        [Range(0f, 1f)] public float throttle = 0f;

        public AnimationCurve _engineTorque;

        [Space]
        public float frontSlipAngle;
        public float rearSlipAngle;

        Vector2 _position;
        public Vector2 worldVelocity;
        [SerializeField] float _angularVelocity;
        [SerializeField] float _rotation;

        [Header("Debug Properties")]
        public bool _debugApplyTransform;

        private void OnValidate()
        {
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(transform.position + GetForward() * distanceFrontAxle, Quaternion.Euler(0f, 0f, (_rotation + steerAngle) * Mathf.Rad2Deg + 90f) * Vector3.up * 5f);
            Gizmos.DrawRay(transform.position + GetForward() * distanceFrontAxle, Quaternion.Euler(0f, 0f, (_rotation + steerAngle) * Mathf.Rad2Deg - 90f) * Vector3.up * 5f);
            Gizmos.DrawRay(transform.position, worldVelocity);
        }

        private void OnEnable()
        {
            _angularVelocity = _rotation = 0f;
            worldVelocity = Vector2.zero;
        }

        private void Update()
        {
            MyUpdate();
        }

        void MyUpdate()
        {
            const float gearRatio = 3.31f;
            const float finalDriveRatio = 3.73f;
            const float wheelRadius = 0.3f;
            const float bodyWidth = 1.8f;
            const float bodyLength = 4f;
            const float engineMaxRpm = 6000f;

            // lateral - боковой
            // longitudinal - продольные

            Vector2 localVelocity = transform.InverseTransformDirection(worldVelocity);

            if (localVelocity.magnitude > float.Epsilon)
            {
                frontSlipAngle = Mathf.Atan((localVelocity.x + _angularVelocity * distanceFrontAxle) / localVelocity.y) - steerAngle * Mathf.Sign(localVelocity.y);
                rearSlipAngle = Mathf.Atan((localVelocity.x + _angularVelocity * distanceRearAxle) / localVelocity.y);

                //Debug.DrawRay(transform.position + GetForward() * distanceFrontAxle, Quaternion.Euler(0f, 0f, (_rotation + steerAngle) * Mathf.Rad2Deg) * Vector3.up * 1f, Color.green);
                //Debug.DrawRay(transform.position + GetForward() * distanceFrontAxle, worldVelocity, new Color(0.1f, 1f, 0.1f, 1f));
            }
            else
            {
                frontSlipAngle = rearSlipAngle = 0f;
            }

            Vector2 rearAxleForce = Vector2.zero;
            rearAxleForce.x = TireFriction(rearSlipAngle);
            rearAxleForce.x = Mathf.Min(1f, rearAxleForce.x);
            rearAxleForce.x *= KgToH(mass * 0.5f); // нагрузка на ось


            Vector2 frontAxleForce = Vector2.zero;
            float frontAxleTireFriction = TireFriction(frontSlipAngle);
            frontAxleTireFriction = Mathf.Min(1f, frontAxleTireFriction);
            frontAxleForce.x = frontAxleTireFriction * KgToH(mass * 0.5f);

            float engineRpm = localVelocity.y * 60f * gearRatio * finalDriveRatio / (2f * Mathf.PI * wheelRadius);
            engineRpm = Mathf.Clamp(engineRpm, 1000f, engineMaxRpm);

            float engineTorque = _engineTorque.Evaluate(engineRpm);

            float wheelTorque = engineTorque * gearRatio * finalDriveRatio;

            float tractionForce = wheelTorque * throttle / wheelRadius;

            Vector2 resistanceForce = Vector2.zero;

            float bodyTorque = Mathf.Cos(steerAngle) * frontAxleForce.x * distanceFrontAxle - rearAxleForce.x * distanceRearAxle;

            float steerAngleSin = Mathf.Sin(steerAngle);
            float steerAngleCos = Mathf.Cos(steerAngle);

            //Debug.DrawRay(transform.position, GetForward() * steerAngleSin);
            //Debug.DrawRay(transform.position, GetRight() * steerAngleCos);

            Vector2 bodyForce = Vector2.zero;
            bodyForce.y = tractionForce + frontAxleForce.x * steerAngleSin * resistanceForce.y;
            bodyForce.x = rearAxleForce.x + frontAxleForce.x * steerAngleCos * resistanceForce.x;

            Vector2 acceleration = bodyForce / mass;
            Vector2 worldAcceleration = transform.TransformDirection(acceleration);// TransformLocalToWorld(acceleration);
            worldVelocity += worldAcceleration * Time.deltaTime;

            float angularAcceleration = bodyTorque / GetRectInertia(mass, bodyWidth, bodyLength);
            _angularVelocity += angularAcceleration * Time.deltaTime;


            if (_debugApplyTransform)
            {
                _position += worldVelocity * Time.deltaTime;
                _rotation += _angularVelocity * Time.deltaTime;

                transform.position = _position;
                transform.rotation = Quaternion.Euler(0f, 0f, _rotation * Mathf.Rad2Deg);
            }
        }

        Vector2 TransformWorldToLocal(Vector2 v)
        {
            return Bibyter.Mathematics.Shape2dExt.RotatePoint(v, -_rotation);
        }

        Vector2 TransformLocalToWorld(Vector2 v)
        {
            return Bibyter.Mathematics.Shape2dExt.RotatePoint(v, _rotation);
        }

        Vector3 GetForward()
        {
            return transform.up;
        }
        
        Vector3 GetRight()
        {
            return transform.right;
        }

        float GetRectInertia(float mass, float width, float length)
        {
            return (1f / 12f) * mass * (width * width + length * length);
        }

        float KgToH(float v)
        {
            return v * 9.81f;
        }

        float TireFriction(float slipAngleRad)
        {
            slipAngleRad = Mathf.Abs(slipAngleRad);
            // при 3 градусах максимальная сила трения 1, производная получается 0.33
            // т.к. угол в радианах, производную тоже считаем в радианах
            float derivative = 1f / 3f * Mathf.Rad2Deg;
            return slipAngleRad * derivative;
        }
    }
}