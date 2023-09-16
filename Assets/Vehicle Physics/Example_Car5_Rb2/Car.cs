using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Example_Car5_Rb2
{
    public sealed class Car : MonoBehaviour
    {
        [SerializeField] Rigidbody2D _rigidbody;
        [SerializeField] Vector2 _velocity;
        [Space]
        [SerializeField, Range(-1.5f, 1.5f)] float _steerAngle;
        [SerializeField, Range(0f, 1f)] float _throttle;
        [SerializeField] float _motorTorqie = 1000f;
        [SerializeField] float _maxSteerAngle = 1f;
        [Space]
        [SerializeField] SteeringHelper _steeringHelper;
        [Space]
        [SerializeField] Wheel[] _wheels;


        public float maxSteerAngleEuler
        {
            set { _maxSteerAngle = value * Mathf.Deg2Rad; }
            get { return _maxSteerAngle * Mathf.Rad2Deg; }
        }

        [System.Serializable]
        public struct Wheel
        {
            [SerializeField] Transform _transform;
            public float sideFrictionMultiplier;

            public bool debugPrint;


            public bool isSterring;
            public bool isMotor;

            public void Update(Rigidbody2D rigidbody, Transform transform, int frictionPointCount, float steerAngle, float motorTorque)
            {
                // уравнение трения
                // нужно найти направление трения и величину (магнитуду)
                // friction = -1 * mu * N * norm(v)
                // mu - коеффициент трения, скаляр
                // N - сила реакции опоры, другими словами - сила перпендикулярная двум соприкасающимся объектам, скаляр
                // N = m*g
                // norm(v) - направление трения, нормализованная скорость, вектор

                if (isSterring)
                {
                    _transform.localEulerAngles = new Vector3(0f, 0f, steerAngle * Mathf.Rad2Deg);
                }

                

                var wheelWorldPosition = _transform.position;
                var wheelForward = (Vector2)_transform.up;
                float wheelRadius = 0.3f;

                Vector2 tractionForce = Vector2.zero;

                if (isMotor)
                {
                    tractionForce = wheelForward * (motorTorque / wheelRadius);
                }

                const float mu = 0.9f;
                float N = (rigidbody.mass * (1f / frictionPointCount)) * 10f;

                Vector2 worldVelocity = rigidbody.GetPointVelocity(wheelWorldPosition);

                Vector2 localVelocity = _transform.InverseTransformDirection(worldVelocity);
                // убираем продольное трение
                localVelocity.y = 0f;

                worldVelocity = _transform.TransformDirection(localVelocity);

                // делаем нормализацию только после превышения магнитуды единыци для корретного зануления скоростей
                if (worldVelocity.magnitude > 1f)
                    worldVelocity.Normalize();

                Vector2 sideFrictionForce = -1f * mu * N * worldVelocity;
                sideFrictionForce = ApplyFrictionCircle(sideFrictionForce, tractionForce);

                var totalForce = sideFrictionForce + tractionForce;
                rigidbody.AddForceAtPosition(totalForce, wheelWorldPosition);

                Debug.DrawRay(wheelWorldPosition, tractionForce * 0.003f, Color.green);
                Debug.DrawRay(wheelWorldPosition, sideFrictionForce * 0.003f, Color.blue);
            }

            private Vector2 ApplyFrictionCircle(Vector2 sideFrictionForce, Vector2 tractionForce)
            {
                var forces = new Vector2(sideFrictionForce.magnitude * sideFrictionMultiplier, tractionForce.magnitude);
                var normalizedForces = forces.normalized;

                if (debugPrint)
                    print($"{normalizedForces.x} {normalizedForces.y}");

                return sideFrictionForce * normalizedForces.x;
            }

            public void DrawGizmos(Transform transform)
            {
                Gizmos.DrawWireSphere(_transform.position, 0.2f);
            }
        }

        public float slipAngle;

        void Start()
        {

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _rigidbody.AddForce(Vector2.one * 500f, ForceMode2D.Impulse);
            }
        }

        private void OnDrawGizmos()
        {
            for (int i = 0; i < _wheels.Length; i++)
            {
                _wheels[i].DrawGizmos(transform);
            }
        }

        void FixedUpdate()
        {
            float horAxis = -Input.GetAxis("Horizontal");
            float verAxis = Input.GetAxis("Vertical");

            Vector2 forward = transform.up;
            Vector2 velocity = _rigidbody.velocity;

            float steeringAngle = Mathf.Clamp(GetManualSteeringAngle(horAxis, forward, velocity) + _steeringHelper.Apply(forward, velocity), -_maxSteerAngle, _maxSteerAngle);

            for (int i = 0; i < _wheels.Length; i++)
            {
                _wheels[i].Update(_rigidbody, transform, _wheels.Length, steeringAngle, verAxis * _motorTorqie);
            }

            Debug.DrawRay(_rigidbody.position, _rigidbody.velocity, Color.yellow);

            return;

            var worldVelocity = _rigidbody.velocity;
            Debug.DrawRay(transform.position, worldVelocity);
            var localVelocty = _rigidbody.GetRelativeVector(worldVelocity);

            slipAngle = Vector2.Angle(localVelocty, Vector2.up);

            float latForce = TireFriction(slipAngle);

            if (latForce > 1f)
                latForce = 1f;

            float load = KgToH(_rigidbody.mass);

            latForce = latForce * load;

            Vector2 force = Vector2.zero;
            force.x = -latForce;

            _rigidbody.AddRelativeForce(force, ForceMode2D.Force);


            return;

            var wheelPosition = new Vector2(0f, 1.8f);
            var wheelWorldPosition = transform.TransformPoint(wheelPosition);
            var wheelWorldVelocity = _rigidbody.GetRelativePointVelocity(wheelPosition);
            Debug.DrawRay(transform.TransformPoint(wheelPosition), wheelWorldVelocity);

            var wheelWorldForward = transform.TransformDirection(Quaternion.Euler(0f, 0f, -_steerAngle) * Vector2.up);
            var wheelWorldRight = transform.TransformDirection(Quaternion.Euler(0f, 0f, -_steerAngle) * Vector2.right);
            Debug.DrawRay(transform.TransformPoint(wheelPosition), wheelWorldForward);

            slipAngle = Vector2.Angle(wheelWorldVelocity, wheelWorldForward);

            float sideFriction = TireFriction(slipAngle);
            sideFriction = Mathf.Min(1f, sideFriction);
            sideFriction = sideFriction * KgToH(_rigidbody.mass); // нагрузка на ось

            float tractionForce = 1000f * _throttle;

            var wheelWorldForce = (wheelWorldForward * tractionForce) + (wheelWorldRight * sideFriction);

            // разобраться как применять силу трения
            //if (Input.GetKey(KeyCode.Space))
            {
                //_rigidbody.AddTorque(1000f);
                _rigidbody.AddForceAtPosition(wheelWorldForce, wheelWorldPosition);
            }
        }

        float GetManualSteeringAngle(float horInput, Vector2 forward, Vector2 velocity)
        {
            return horInput * 35f * Mathf.Deg2Rad;

            float angle = 0f;

            if (velocity.magnitude > 0.3f)
            {
                angle = Vector2.Angle(forward, velocity);
            }

            float maxSteeringAngle = Mathf.Lerp(30f, maxSteerAngleEuler, Mathf.InverseLerp(0f, 25f, angle));
            print($"angle {angle} time {Mathf.InverseLerp(0f, 25f, angle)} {maxSteeringAngle}");
            return maxSteeringAngle * horInput * Mathf.Deg2Rad;
        }

        float TireFriction(float slipAngleEuler)
        {
            slipAngleEuler = Mathf.Abs(slipAngleEuler);
            // при 3 градусах максимальная сила трения 1, производная получается 0.33
            // т.к. угол в радианах, производную тоже считаем в радианах
            float derivative = 1f / 3f;
            return slipAngleEuler * derivative;
        }

        float KgToH(float v)
        {
            return v * 9.81f;
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("SteeringHelper.intensity");

            var str = GUILayout.TextField(_steeringHelper.intensity.ToString(), GUILayout.Width(150f));

            if (float.TryParse(str, out float parseValuef))
            {
                _steeringHelper.intensity = parseValuef;
            }

            GUILayout.EndHorizontal();

            //////////////////////////////////
            GUILayout.BeginHorizontal();
            GUILayout.Label("MotorTorque");

            str = GUILayout.TextField(_motorTorqie.ToString(), GUILayout.Width(150f));

            if (float.TryParse(str, out parseValuef))
            {
                _motorTorqie = parseValuef;
            }
            GUILayout.EndHorizontal();
            //////////////////////////
            GUILayout.BeginHorizontal();
            GUILayout.Label("SideFrictionMultiplier");

            str = GUILayout.TextField(_wheels[0].sideFrictionMultiplier.ToString(), GUILayout.Width(150f));

            if (float.TryParse(str, out parseValuef))
            {
                for (int i = 0; i < _wheels.Length; i++)
                {
                    _wheels[i].sideFrictionMultiplier = parseValuef;
                }
            }
            GUILayout.EndHorizontal();
            //////////////////////////
            GUILayout.BeginHorizontal();
            GUILayout.Label("MaxSteerAngle");

            str = GUILayout.TextField(maxSteerAngleEuler.ToString(), GUILayout.Width(150f));

            if (float.TryParse(str, out parseValuef))
            {
                maxSteerAngleEuler = parseValuef;
            }
            GUILayout.EndHorizontal();
        }
    }

    [System.Serializable]
    public sealed class SteeringHelper
    {
        [SerializeField, Range(0, 1f)] float _intensity;

        public float intensity
        {
            set { _intensity = Mathf.Clamp(value, 0f, 1f); }
            get { return _intensity; }
        }

        public float Apply(Vector2 forward, Vector2 velocity)
        {
            if (velocity.magnitude < 0.3f)
                return 0f;

            var angle = Vector2.SignedAngle(velocity, forward);
            angle = Mathf.Clamp(angle, -60f, 60f) * Mathf.Deg2Rad * -1f;

            return angle * _intensity * 2f;
        }
    }
}