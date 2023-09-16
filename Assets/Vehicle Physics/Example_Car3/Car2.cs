using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Example_Car3
{
    public class Car2 : MonoBehaviour
    {
        [SerializeField] EngineSound _engineSound;
        [SerializeField] WheelSlipSound _wheelSlipSound;

        [SerializeField] UnityEngine.Rigidbody _rigidbody;
        [Space]
        [SerializeField] AnimationCurve _torqueCurve;
        //[SerializeField] float _engineMinRmp = 1250f;
        [SerializeField] float _steerAngle = 0f;
        [SerializeField] float _steerAngleMax = 30f;
        [SerializeField] float _steerSmooth = 120f;

        public enum AckermannAngle { Positive, Negative }
        [SerializeField] AckermannAngle _ackermannAngle;

        [SerializeField] Gear _gear;

        [SerializeField] WheelCollider _frontLeftWheelCollider;
        [SerializeField] WheelCollider _frontRightWheelCollider;
        [SerializeField] WheelCollider _rearLeftWheelCollider;
        [SerializeField] WheelCollider _rearRightWheelCollider;

        public WheelCollider frontLeftWheelCollider => _frontLeftWheelCollider;
        public WheelCollider frontRightWheelCollider => _frontRightWheelCollider;
        public WheelCollider rearLeftWheelCollider => _rearLeftWheelCollider;
        public WheelCollider rearRightWheelCollider => _rearRightWheelCollider;

        [SerializeField] Transform _frontLeftViewTransform;
        [SerializeField] Transform _frontRightViewTransform;
        [SerializeField] Transform _rearLeftViewTransform;
        [SerializeField] Transform _rearRightViewTransform;

        [Space]
        [SerializeField] bool _debugDrawSlip;
        [SerializeField] bool _debugDrawSteer;

        float _cacheEngineRmp;

        public float infoTractionForce;
        public float infoEngineTorque;
        public float infoRearAxleTorque;
        public float infoEngnineRmp;
        public float infoDifferentialValue;


        float _finalDriveRatio = 4.1f; // задний редуктор, передаточное значение

        private void Awake()
        {
            _engineSound.Awake(gameObject);
            _wheelSlipSound.Awake(this);
        }

        // Start is called before the first frame update
        void Start()
        {
            Time.fixedDeltaTime = 1f / 120f;
        }

        // Update is called once per frame
        void Update()
        {

        }

        float _guiVar;
        int _guiCounter;

        private void OnGUI()
        {
            var e = Event.current;

            if (e.type == EventType.Repaint)
            {
                _guiCounter = (_guiCounter + 1) % 60;
            }

            GUI.Box(new Rect(45 - 5, 45 - 5, 350, 150), GUIContent.none);
            GUI.BeginGroup(new Rect(45, 45, 350, 150));


            GUILayout.Label("Сила тяги: " + infoTractionForce);

            GUILayout.Label("Крутящий момент двигателя: " + infoEngineTorque);

            GUILayout.Label("Крутящий момент задней оси: " + infoRearAxleTorque);

            GUILayout.Label("Кол-во оборотов двигателя: " + infoEngnineRmp);

            GUILayout.Label("Скорость км: " + _rigidbody.velocity.magnitude * 3.6f);


            if (_guiCounter % 5 == 0)
            {
                _guiVar = _rearLeftWheelCollider.rpm - _rearRightWheelCollider.rpm;
            }

            GUILayout.Label("Разница rpm задних колес: " + _guiVar.ToString("0"));

            GUI.EndGroup();
        }

        private void OnDrawGizmos()
        {
            UnityEditor.Handles.Label(transform.TransformPoint(_rearLeftWheelCollider.center), _rearLeftWheelCollider.rpm.ToString("0"), UnityEditor.EditorStyles.whiteLabel);
            UnityEditor.Handles.Label(transform.TransformPoint(_rearRightWheelCollider.center), _rearRightWheelCollider.rpm.ToString("0"), UnityEditor.EditorStyles.whiteLabel);

            UnityEditor.Handles.Label(transform.TransformPoint(_rearLeftWheelCollider.center + new Vector3(0f, 0.5f, 0f)), _rearLeftWheelCollider.motorTorque.ToString("0"), UnityEditor.EditorStyles.whiteLabel);
            UnityEditor.Handles.Label(transform.TransformPoint(_rearRightWheelCollider.center + new Vector3(0f, 0.5f, 0f)), _rearRightWheelCollider.motorTorque.ToString("0"), UnityEditor.EditorStyles.whiteLabel);
        }

        private void FixedUpdate()
        {
            float newEngineRmp = GetEngineRmpWithWheel();// SpeedToEngineRmp(_rigidbody.velocity.magnitude, Transmission_GetGearRatio(_gear));

            _cacheEngineRmp = Mathf.Lerp(_cacheEngineRmp, newEngineRmp, Time.fixedDeltaTime * 6f);

            float engineTorque = Engine_GetTorque(_cacheEngineRmp);
            float rearAxleTorque = engineTorque * Transmission_GetGearRatio(_gear) * _finalDriveRatio;

            infoRearAxleTorque = rearAxleTorque;
            infoEngineTorque = engineTorque;
            infoEngnineRmp = _cacheEngineRmp;
            infoTractionForce = CalculateEngineForce(_cacheEngineRmp, Transmission_GetGearRatio(_gear));

            if (Input.GetAxisRaw("Vertical") != 0f)
            {
                LockingDifferential_ApplyTorque(rearAxleTorque * Input.GetAxis("Vertical"));

                //_rearLeftWheelCollider.motorTorque = rearAxleTorque / 2f;
                //_rearRightWheelCollider.motorTorque = rearAxleTorque / 2f;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                LockingDifferential_ApplyTorque(-rearAxleTorque);

                //_rearLeftWheelCollider.motorTorque = rearAxleTorque / -2f;
                //_rearRightWheelCollider.motorTorque = rearAxleTorque / -2f;
            }
            else
            {
                _rearLeftWheelCollider.motorTorque = _rearRightWheelCollider.motorTorque = 0f;
            }


            if (Input.GetAxisRaw("Horizontal") != 0f)
            {

                _steerAngle += Input.GetAxisRaw("Horizontal") * _steerSmooth * Time.deltaTime;
                _steerAngle = Mathf.Clamp(_steerAngle, -_steerAngleMax, _steerAngleMax);
            }
            else
            {
                _steerAngle = Mathf.MoveTowards(_steerAngle, 0f, Time.deltaTime * 120f);
            }


            float localSteerAngle = _steerAngle;

            //_ackermannAngle = Input.GetKey(KeyCode.Space) ? AckermannAngle.Negative : AckermannAngle.Positive;
            //localSteerAngle = Input.GetKey(KeyCode.Space) ? -_steerAngle : _steerAngle;

            if (Mathf.Abs(localSteerAngle) > 0.05f)
            {
                float steerAngleRad = (localSteerAngle * Mathf.Deg2Rad) + (Mathf.PI * 0.5f);

                float distanceBeetwenAxles = Mathf.Abs(_frontLeftWheelCollider.center.z - _rearLeftWheelCollider.center.z); // от переднего моста до заднего
                float carSteerRadius = Mathf.Tan(steerAngleRad) * distanceBeetwenAxles;

                switch (_ackermannAngle)
                {
                    case AckermannAngle.Positive:
                        if (_debugDrawSteer)
                        {
                            Debug.DrawRay(transform.TransformPoint(new Vector3(0f, _rearLeftWheelCollider.center.y, _rearLeftWheelCollider.center.z)), transform.TransformDirection(Vector3.left * carSteerRadius));
                        }

                        WheelApplySteer_AckermannPositive(_frontRightWheelCollider, carSteerRadius, distanceBeetwenAxles);
                        WheelApplySteer_AckermannPositive(_frontLeftWheelCollider, carSteerRadius, distanceBeetwenAxles);
                        break;
                    case AckermannAngle.Negative:
                        if (_debugDrawSteer)
                        {
                            Debug.DrawRay(
                                transform.TransformPoint(new Vector3(0f, _rearLeftWheelCollider.center.y, _rearLeftWheelCollider.center.z + (distanceBeetwenAxles * 2f))), 
                                transform.TransformDirection(Vector3.right * carSteerRadius)
                            );
                        }

                        WheelApplySteer_AckermannNegative(_frontRightWheelCollider, carSteerRadius, distanceBeetwenAxles);
                        WheelApplySteer_AckermannNegative(_frontLeftWheelCollider, carSteerRadius, distanceBeetwenAxles);
                        break;
                }
            }

            WhellViewUpdate(_frontLeftWheelCollider, _frontLeftViewTransform);
            WhellViewUpdate(_frontRightWheelCollider, _frontRightViewTransform);
            WhellViewUpdate(_rearLeftWheelCollider, _rearLeftViewTransform);
            WhellViewUpdate(_rearRightWheelCollider, _rearRightViewTransform);

            _engineSound.Update(_cacheEngineRmp, 1250, 5600);
            _wheelSlipSound.Update();

            Debug.DrawRay(_rigidbody.position, _rigidbody.velocity);

            if (_debugDrawSlip)
            {
                DebugDrawSlip();
            }

            FunctionGraph_Update(rearAxleTorque);
        }

        #region
        [SerializeField] Bibyter.UiElements.FunctionGraph.UiFunctionGraph _wheelRpmFunctionGraph;
        [SerializeField] Bibyter.UiElements.FunctionGraph.UiFunctionGraph _engineFunctionGraph;

        void FunctionGraph_Update(float torque)
        {
            if (_wheelRpmFunctionGraph == null)
            {
                _wheelRpmFunctionGraph = FindObjectOfType<Bibyter.UiElements.FunctionGraph.UiFunctionGraph>();
            }

            _wheelRpmFunctionGraph.AddValue(0, _rearLeftWheelCollider.rpm);
            _wheelRpmFunctionGraph.AddValue(1, _rearRightWheelCollider.rpm);

            _engineFunctionGraph.AddValue(0, torque);
        }
        #endregion

        public float LockingDifferential_pow = 2f;

        void LockingDifferential_ApplyTorque(float torque)
        {
            float leftWheelAngularVelocity = Mathf.Pow(Mathf.Max(0.01f, _rearLeftWheelCollider.rpm), LockingDifferential_pow);
            float rightWheelAngularVelocity = Mathf.Pow(Mathf.Max(0.01f, _rearRightWheelCollider.rpm), LockingDifferential_pow);

            float totalAngularVelocity = leftWheelAngularVelocity + rightWheelAngularVelocity;

            float differentialLeftMultiplier = rightWheelAngularVelocity / totalAngularVelocity;
            float differentialRightMultiplier = leftWheelAngularVelocity / totalAngularVelocity;


            _rearLeftWheelCollider.motorTorque = torque * differentialLeftMultiplier;
            _rearRightWheelCollider.motorTorque = torque * differentialRightMultiplier;

            infoDifferentialValue = differentialLeftMultiplier;
            print(differentialLeftMultiplier);
        }

        float GetInertia(WheelCollider wheelCollider)
        {
            return 0.5f * wheelCollider.mass * (wheelCollider.radius * wheelCollider.radius);
        }

        void LockingDifferential_ApplyTorque2(float torque)
        {
            float velocityLeft = _rearLeftWheelCollider.rpm / 60f * (Mathf.PI * 2f);
            float velocityRight = _rearRightWheelCollider.rpm / 60f * (Mathf.PI * 2f);

            float halfDifference = (velocityLeft - velocityRight) * 0.5f;
            float differentialLockingTorque = GetInertia(_rearLeftWheelCollider) * halfDifference / Time.fixedDeltaTime;

            _rearLeftWheelCollider.motorTorque = torque - differentialLockingTorque;
            _rearRightWheelCollider.motorTorque = torque + differentialLockingTorque;
        }

        public float LockingDifferential_ApplyTorque3_coef = 1f;

        public void LockingDifferential_ApplyTorque3_coef_set(float v)
        {
            LockingDifferential_pow = v;
            LockingDifferential_ApplyTorque3_coef = v;
        }

        void LockingDifferential_ApplyTorque3(float torque)
        {
            float velocityLeft = _rearLeftWheelCollider.rpm / 60f * (Mathf.PI * 2f);
            float velocityRight = _rearRightWheelCollider.rpm / 60f * (Mathf.PI * 2f);

            float halfDifference = (velocityLeft - velocityRight) * 0.5f;

            _rearLeftWheelCollider.motorTorque = torque - (halfDifference * LockingDifferential_ApplyTorque3_coef);
            _rearRightWheelCollider.motorTorque = torque + (halfDifference * LockingDifferential_ApplyTorque3_coef);
        }

        void WheelApplySteer_AckermannPositive(WheelCollider wheelCollider, float carSteerRadius, float distanceBeetwenAxles)
        {
            float wheelSteerRadius = carSteerRadius + wheelCollider.center.x;

            float steerAngle = (Mathf.Atan(wheelSteerRadius / distanceBeetwenAxles) - (Mathf.PI * 0.5f) * Mathf.Sign(wheelSteerRadius)) * Mathf.Rad2Deg;

            if (_debugDrawSteer)
            {
                Debug.DrawRay(transform.TransformPoint(wheelCollider.center), Quaternion.Euler(0f, steerAngle, 0f) * transform.forward * 1f);
                Debug.DrawRay(transform.TransformPoint(wheelCollider.center), Quaternion.Euler(0f, steerAngle, 0f) * transform.right * 10f * Mathf.Sign(-carSteerRadius));
            }

            wheelCollider.steerAngle = steerAngle;
        }

        void WheelApplySteer_AckermannNegative(WheelCollider wheelCollider, float carSteerRadius, float distanceBeetwenAxles)
        {
            float wheelSteerRadius = carSteerRadius - wheelCollider.center.x;

            float steerAngle = (Mathf.Atan(wheelSteerRadius / distanceBeetwenAxles) - (Mathf.PI * 0.5f) * Mathf.Sign(wheelSteerRadius)) * Mathf.Rad2Deg;

            if (_debugDrawSteer)
            {
                Debug.DrawRay(transform.TransformPoint(wheelCollider.center), Quaternion.Euler(0f, steerAngle, 0f) * transform.forward * 1f);
                Debug.DrawRay(transform.TransformPoint(wheelCollider.center), Quaternion.Euler(0f, steerAngle, 0f) * transform.right * 10f * Mathf.Sign(carSteerRadius));
            }

            wheelCollider.steerAngle = steerAngle;
        }

        void WhellViewUpdate(WheelCollider wheelCollider, Transform viewTransform)
        {
            wheelCollider.GetWorldPose(out var position, out var quaternion);

            viewTransform.SetPositionAndRotation(position, quaternion);
        }

        enum Gear { g1, g2, g3, g4 }

        float Transmission_GetGearRatio(Gear gear)
        {
            switch (gear)
            {
                case Gear.g1: return 3.67f;
                case Gear.g2: return 2.1f;
                case Gear.g3: return 1.36f;
                case Gear.g4: return 1f;
                default: return 0f;
            }
        }

        float Engine_GetTorque(float rpm)
        {
            return _torqueCurve.Evaluate(rpm);
        }

        float SpeedToEngineRmp(float speed, float transmissionGearRatio)
        {
            float wheelLenght = 2f * Mathf.PI * _frontLeftWheelCollider.radius;

            float wheelRmp = speed / wheelLenght;

            float engineRmpSec = wheelRmp * _finalDriveRatio * transmissionGearRatio;

            float engineRmpMin = engineRmpSec * 60f;

            return Mathf.Max(engineRmpMin, 1250f);
        }

        float CalculateEngineForce(float engineRmp, float transmissionGearRatio)
        {
            float engineTorque = Engine_GetTorque(engineRmp);

            // расчет мощности от движка
            float engineForce = (engineTorque * transmissionGearRatio * _finalDriveRatio) / _frontLeftWheelCollider.radius; // в ньютонах

            return engineForce;
        }

        private float GetEngineRmpWithWheel()
        {
            float middleWheelRmp = Mathf.Lerp(_rearLeftWheelCollider.rpm, _rearRightWheelCollider.rpm, 0.5f);
            float engoneRmp = middleWheelRmp * Transmission_GetGearRatio(_gear) * _finalDriveRatio;
            return engoneRmp;
        }

        private void DebugDrawSlip()
        {
            if (rearLeftWheelCollider.GetGroundHit(out var hit))
            {
                Debug.DrawRay(
                    transform.TransformPoint(rearLeftWheelCollider.center),
                    transform.TransformDirection(Vector3.left * hit.sidewaysSlip), Color.yellow
                );

                Debug.DrawRay(
                    transform.TransformPoint(rearLeftWheelCollider.center),
                    transform.TransformDirection(Vector3.forward * hit.forwardSlip), Color.red
                );
            }

            if (rearRightWheelCollider.GetGroundHit(out hit))
            {
                Debug.DrawRay(
                    transform.TransformPoint(rearRightWheelCollider.center),
                    transform.TransformDirection(Vector3.left * hit.sidewaysSlip), Color.yellow
                );

                Debug.DrawRay(
                    transform.TransformPoint(rearRightWheelCollider.center),
                    transform.TransformDirection(Vector3.forward * hit.forwardSlip), Color.red
                );
            }
        }
    }

    [System.Serializable]
    public sealed class EngineSound
    {
        public AudioClip audioClip;

        [Tooltip("Volume added to the base engine volume depending on engine state.")]
        [Range(0, 1)]
        public float volumeRange = 0.5f;

        [Tooltip("Pitch added to the base engine pitch depending on engine RPM.")]
        [Range(0, 4)]
        public float pitchRange = 1.5f;

        [Tooltip("Smoothing of engine sound.")]
        [Range(0, 1)]
        public float smoothing = 0.1f;

        float pitch = 0.9f;

        AudioSource _audioSource;

        public void Awake(GameObject gameObject)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.clip = audioClip;
            _audioSource.loop = true;
            _audioSource.Play();
            _audioSource.volume = 0.2f;
        }

        public void Update(float engineRmp, float engineRmpMin, float engineRmpMax)
        {
            float rpmModifier = Mathf.Clamp01((engineRmp - engineRmpMin) / engineRmpMax);
            float newPitch = pitch + rpmModifier * pitchRange;
            _audioSource.pitch = Mathf.Lerp(_audioSource.pitch, newPitch, 1f - smoothing);
        }
    }

    [System.Serializable]
    public sealed class WheelSlipSound
    {
        [SerializeField] AudioSource _audioSource;

        Car2 _car;

        UnityEngine.Rigidbody _rigidbody;
        Transform _transform;

        public void Awake(Car2 car)
        {
            _car = car;
            _rigidbody = car.GetComponent<UnityEngine.Rigidbody>();
            _transform = car.transform;

            _audioSource.loop = true;
            _audioSource.Play();
        }

        public void Update()
        {
            _audioSource.volume = 0f;

            float totalSidewaysSlip = 0f;
            float totalForwardSlip = 0f;
            int handledWheelCount = 0;

            if (_car.rearLeftWheelCollider.GetGroundHit(out var hit))
            {
                totalSidewaysSlip += Mathf.Abs(hit.sidewaysSlip);
                totalForwardSlip += Mathf.Abs(hit.forwardSlip);
                handledWheelCount++;
            }

            if (_car.rearRightWheelCollider.GetGroundHit(out hit))
            {
                totalSidewaysSlip += Mathf.Abs(hit.sidewaysSlip);
                totalForwardSlip += Mathf.Abs(hit.forwardSlip);
                handledWheelCount++;
            }


            totalSidewaysSlip /= handledWheelCount;
            totalForwardSlip /= handledWheelCount;

            float normalizedForwardSlip = Mathf.InverseLerp(0.05f, 1f, totalForwardSlip);
            float normalizedSidewaySlip = Mathf.InverseLerp(0.1f, 0.3f, totalSidewaysSlip);

            _audioSource.volume = new Vector2(0f, normalizedSidewaySlip).magnitude;
        }
    }
}