using Bibyter.Mathematics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Example_Car3
{
    // что-бы найти инфу о машине, надо искать техпаспорт
    // жугили https://ru.wikipedia.org/wiki/%D0%92%D0%90%D0%97-2106
    // главная пара (редуктор заднего моста) передаточное знавение = 4.1
    // передача пз -  3.242, 1.989, 1.289, 1
    // радиус колеса 0.28
    // воздушное сопротивление 0.52
    // график крутящего момента http://www.vaz.ee/articles/images/up-gbz/1.jpg
    // масса колеса = 12 кг

    // формулы
    // power = rpm * torque / 9.549f;           power в ватах
    // ват = ньтон на метр (кто бы мог подумать)

    // должна быть torqueCurve и по ней получать torque через rpm

    // вычисление максимально скорости жигули
    // макс мощность - 58,000 ватт
    // площадь лобового сопротивления - 1.8 метра
    // коэффициент лобового сопротивления (обтекаемости) - 0.52
    // плотность воздуха - 1.25 кг/м^3
    //
    // формула сопративления - F = 0.5 · c · D · A · v2
    // с - коэффициент лобового сопротивления  (обтекаемости)
    // D - плотность воздуха в кг/м^3
    // A - проекционную площадь авто в м^2
    // P - мощность двигателя в ваттах
    //
    // уровнение максимальной скорости м. сек.: v = (2 · P / (c · D · A) )^(1/3)
    // v = (2 * 58000 / (0.52 * 1.25 * 1.8))^1/3 = (2 * 58000 / 1.17)^1/3 = 99145^1/3 = 46мс = 165кмч

    // общая теория
    // имеем кривую крутящего момента
    // по ней с помощью rpm высчитывает текущий крутящий момент
    // с помощью него высчитываем силу тяги H

    public sealed class Car : MonoBehaviour
    {
        const float airDensity = 1.25f; // плотность воздуха

        [SerializeField] AnimationCurve _torqueCurve;
        [SerializeField] bool _engineOn = false;
        [SerializeField] float _engineMinRmp = 1250f;
        [SerializeField] float _engineMaxRmp = 6500f;

        //[SerializeField] float _engineTorque = 121f;
        [SerializeField] float _environmentDragCoef = 0.52f; // коэффициент лобового сопротивления (обтекаемости)
        [SerializeField] float _environmentDragArea = 1.8f; // площадь лобового сопротивления
        [SerializeField] float _groundFriction = 30f;
        [SerializeField] float _mass = 100f;
        [Space]
        [SerializeField] float _wheelMass = 12f;
        [SerializeField] float _wheelRadius = 0.4f;
        [SerializeField] Transform _readWheelAxleTransform;

        // угловая скорость задней оси
        float _rearAxleWheelAngularVelocity;
        float _readAxleWheelRotation;

        [Space]
        [SerializeField, Range(-0.8f, 0.8f)] float _steerAngle = 0f;
        

        float _finalDriveRatio = 4.1f; // задний редуктор, передаточное значение
        [Space]
        [SerializeField] bool _drawTractionBalanceGraphing;
        [SerializeField] float _graphingForceScale = 1f;
        [SerializeField] float _graphingSpeedScale = 10f;

        enum Gear { g1, g2, g3, g4 }
        [SerializeField] Gear _gear;

        [Header("Info")]
        [SerializeField] float _maxSpeedKm;
        [SerializeField] float _maxSpeedM;
        [SerializeField] float _currentSpeedMs;
        [SerializeField] float _currentSpeedKmh;

        float _longitudinalVelocity;
        //Vector3 _velocity;
        Vector3 _position;
        [SerializeField] float _rotation;

        public float GetGearRatio()
        {
            return Transmission_GetGearRatio(_gear);
        }


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

        float GetSpeed()
        {
            return _longitudinalVelocity;
        }

        private void OnValidate()
        {
            //_maxSpeedKm = ToKmSec(CalculateMaxSpeed(_engineCurrentRpm, _engineTorque, _environmentDragCoef, _environmentDragArea));
            //_maxSpeedM = CalculateMaxSpeed(_engineCurrentRpm, _engineTorque, _environmentDragCoef, _environmentDragArea);
        }

        private void OnDrawGizmosSelected()
        {
            if (_drawTractionBalanceGraphing)
            {
                DrawTractionBalanceGraphing();
            }
        }

        private void DrawTractionBalanceGraphing()
        {
            const int steps = 40;
            const float stepsf = (float)steps;

            for (int i = 0; i < steps; i++)
            {
                float x_a = i / stepsf;
                float x_b = (i + 1) / stepsf;

                float speedMSec_A = (i / stepsf) * _graphingSpeedScale;
                float speedMSec_B = ((i + 1) / stepsf) * _graphingSpeedScale;

                float groundFrictionForceA = (_groundFriction * speedMSec_A);
                float groundFrictionForceB = (_groundFriction * speedMSec_B);

                Gizmos.color = Color.white;

                Gizmos.DrawLine(
                    new Vector3(x_a, groundFrictionForceA / _graphingForceScale),
                    new Vector3(x_b, groundFrictionForceB / _graphingForceScale)
                );


                // F = 0,5 · c · D · A · v2
                float airDragForceA = 0.5f * _environmentDragCoef * airDensity * _environmentDragArea * (speedMSec_A * speedMSec_A);
                float airDragForceB = 0.5f * _environmentDragCoef * airDensity * _environmentDragArea * (speedMSec_B * speedMSec_B);

                Gizmos.color = Color.yellow;

                Gizmos.DrawLine(
                    new Vector3(x_a, airDragForceA / _graphingForceScale),
                    new Vector3(x_b, airDragForceB / _graphingForceScale)
                );


                Gizmos.color = Color.cyan;

                Gizmos.DrawLine(
                    new Vector3(x_a, (airDragForceA + groundFrictionForceA) / _graphingForceScale),
                    new Vector3(x_b, (airDragForceB + groundFrictionForceB) / _graphingForceScale)
                );

                DrawEngineForce(Transmission_GetGearRatio(Gear.g1), x_a, x_b, speedMSec_A, speedMSec_B, Color.blue);
                DrawEngineForce(Transmission_GetGearRatio(Gear.g2), x_a, x_b, speedMSec_A, speedMSec_B, Color.blue);
                DrawEngineForce(Transmission_GetGearRatio(Gear.g3), x_a, x_b, speedMSec_A, speedMSec_B, Color.blue);
                DrawEngineForce(Transmission_GetGearRatio(Gear.g4), x_a, x_b, speedMSec_A, speedMSec_B, Color.blue);

                if (i % 5 == 0 || i == steps - 1)
                {
                    UnityEditor.Handles.Label(new Vector3(x_a, -0.02f), speedMSec_A.ToString("0.00 ms"));
                    UnityEditor.Handles.Label(new Vector3(x_a, -0.05f), (speedMSec_A * 3.6f).ToString("0 km"));

                    UnityEditor.Handles.Label(new Vector3(-0.12f, x_a + 0.02f), (x_a * _graphingForceScale).ToString("0 hm"));
                }
            }
        }

        private void DrawEngineForce(float transmissionGearRatio, float x_a, float x_b, float speedA, float speedB, Color color)
        {
            Gizmos.color = color;

            float engineRpmA = SpeedToEngineRmp(speedA, transmissionGearRatio);
            float engineRpmB = SpeedToEngineRmp(speedB, transmissionGearRatio);

            if ( engineRpmA < _engineMaxRmp)
            {
                float engineForceA = CalculateEngineForce(engineRpmA, transmissionGearRatio);
                float engineForceB = CalculateEngineForce(engineRpmB, transmissionGearRatio);

                Gizmos.DrawLine(
                    new Vector3(x_a, engineForceA / _graphingForceScale),
                    new Vector3(x_b, engineForceB / _graphingForceScale)
                );
            }
        }

        private void Awake()
        {
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        private void Update()
        {
            //Vector3 worldSteerRight;
            //float steerRadius = float.PositiveInfinity;

            //if (Mathf.Abs(_steerAngle) > 0.05f)
            //{
            //    var localSteerRad = _steerAngle + (Mathf.PI * 0.5f);
            //    var worldSteerRad = localSteerRad + _rotation;

            //    var worldSteerRight2d = Shape2dExt.RadToDirection(worldSteerRad);
            //    worldSteerRight = new Vector3(worldSteerRight2d.x, 0f, worldSteerRight2d.y);

            //    float katVertical = 0.85f; // от переднего моста до заднего
            //    float katHorizontal = Mathf.Tan(localSteerRad) * -katVertical; // радиус поворота
            //    steerRadius = katHorizontal;

            //    var circle = new Circle(katHorizontal);

            //    _rotation += circle.ConvertLinearToAngularSpeed(GetSpeed() * Time.deltaTime);

                
            //}
            //else
            //{
            //    var worldSteerRight2d = Shape2dExt.RadToDirection(_rotation + (Mathf.PI * 0.5f));
            //    worldSteerRight = new Vector3(worldSteerRight2d.x, 0f, worldSteerRight2d.y);
            //}

            //var worldForward2d = Shape2dExt.RadToDirection(_rotation);
            //var forward = new Vector3(worldForward2d.x, 0f, worldForward2d.y);
            //var right = new Vector3(worldForward2d.y, 0f, -worldForward2d.x);

            //_currentSpeedMs = GetSpeed();
            //_currentSpeedKmh = _currentSpeedMs * 3.6f;

            //float engineRpm = SpeedToEngineRmp(GetSpeed(), GetGearRatio());
            //float engineForce = CalculateEngineForce(engineRpm, GetGearRatio());

            //// сила тяги
            //var forceTraction = _engineOn ? engineForce : 0f;

            //// сила вохдушного сопротивления
            //// F = k * S * V^2
            //// F = 0,5 · c · D · A · v2
            //var environmentDragForce = -(_environmentDragCoef * _environmentDragArea * GetSpeed() * GetSpeed());

            //var forcegGroundDrag = -(_groundFriction * GetSpeed());

            //var totalForce = forceTraction + forcegGroundDrag + environmentDragForce;

            //var acceleration = totalForce / _mass;

            //_longitudinalVelocity += Time.deltaTime * acceleration;

            ////_velocity = Vector3Drag(_velocity, _environmentDragCoef, Time.deltaTime);

            //_position += forward * _longitudinalVelocity * Time.deltaTime;

            //transform.position = _position;
            //transform.eulerAngles = new Vector3(0f, _rotation * Mathf.Rad2Deg, 0f);

            //DrawSteerLines(forward, right, worldSteerRight, steerRadius);


            RearWheelAxleUpdate();
        }

        void RearWheelAxleUpdate()
        {
            float engineRmp = SpeedToEngineRmp(GetSpeed(), GetGearRatio());

            float driveTorque = Engine_GetTorque(engineRmp) * Transmission_GetGearRatio(Gear.g1) * _finalDriveRatio;

            float slipRatio = _longitudinalVelocity == 0f ? 0f : (_rearAxleWheelAngularVelocity * _wheelRadius - _longitudinalVelocity) / (Mathf.Abs(_longitudinalVelocity));
            float slipRatio_LongForceMultiplier = 1f - SlipRatioCurve(slipRatio);

            float tractionForce = (driveTorque / _wheelRadius) * slipRatio_LongForceMultiplier;

            float tractionTorque = tractionForce * _wheelRadius;
            float tractionToqueTwoWheel = tractionTorque + tractionTorque;
            float brakeToque = 0f;

            float totalTorque = driveTorque + tractionToqueTwoWheel + brakeToque;

            // inertia of a cylinder = Mass * radius2 / 2
            float wheelInertia = _wheelMass * _wheelRadius * _wheelRadius / 2f;

            // т.к. на оси два колеса, берем сумму
            float twoWheelInertia = wheelInertia + wheelInertia;

            float rearWheelAxleAcceleration = totalTorque / twoWheelInertia;

            _rearAxleWheelAngularVelocity += rearWheelAxleAcceleration * Time.deltaTime;

            _readAxleWheelRotation += _rearAxleWheelAngularVelocity * Time.deltaTime;

            _readWheelAxleTransform.localEulerAngles = new Vector3(_readAxleWheelRotation * Mathf.Rad2Deg, 0f, 0f);

            var forward = Vector3.forward;
            _longitudinalVelocity += tractionForce * Time.deltaTime;
            _position += forward * _longitudinalVelocity * Time.deltaTime;
        }


        void DrawSteerLines(Vector3 forward, Vector3 right, Vector3 worldSteerRight, float steerRadius)
        {
            Debug.DrawRay(transform.position + forward * 0.85f, worldSteerRight * 2f);
            Debug.DrawRay(transform.position + forward * 0.85f, worldSteerRight * -2f);

            if (steerRadius < 100000f)
            {
                Debug.DrawRay(transform.position, right * steerRadius, Color.yellow);
            }

            Debug.DrawRay(transform.position, _longitudinalVelocity * forward, Color.green);
        }

        public static Vector3 Vector3Drag(Vector3 source, float drag, float deltaTime)
        {
            return Vector3.MoveTowards(source, Vector3.zero, source.magnitude * deltaTime * drag);
        }

        public static float Vector1Drag(float source, float drag, float deltaTime)
        {
            return Mathf.MoveTowards(source, 0f, Mathf.Abs(source) * deltaTime * drag);
        }

        float SpeedToEngineRmp(float speed, float transmissionGearRatio)
        {
            float wheelLenght = 2f * Mathf.PI * _wheelRadius;

            float wheelRmp = speed / wheelLenght;

            float engineRmpSec = wheelRmp * _finalDriveRatio * transmissionGearRatio;

            float engineRmpMin = engineRmpSec * 60f;

            return Mathf.Max(engineRmpMin, _engineMinRmp);
        }

        float GetTransmissionTorque(float engineRmp, float transmissionGearRatio)
        {
            float engineForce = (Engine_GetTorque(engineRmp) * transmissionGearRatio * _finalDriveRatio) / _wheelRadius; // в ньютонах

            return engineForce;
        }

        // сила в ньютонах
        float CalculateEngineForce(float engineRmp, float transmissionGearRatio)
        {
            // расчет мощности от движка
            float engineForce = (Engine_GetTorque(engineRmp) * transmissionGearRatio * _finalDriveRatio) / _wheelRadius; // в ньютонах

            return engineForce;
        }

        float Engine_GetTorque(float rpm)
        {
            return _torqueCurve.Evaluate(rpm);
        }

        // https://metinmediamath.wordpress.com/2013/12/14/how-to-calculate-maximum-car-speed-examples-mercedes-c-180-bugatti-veyron/
        static float CalculateMaxSpeed(float rpm, float torque, float environmentDragCoef, float environmentDragArea)
        {
            // v = (2 · P / (c · D · A) )^(1 / 3)

            float power = rpm * torque / 9.549f; // в ватах
            return Mathf.Pow(2f * power / (environmentDragCoef * airDensity * environmentDragArea), 1f / 3f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slipRadio"></param>
        /// <returns>Множитель продольной силы</returns>
        static float SlipRatioCurve(float slipRadio)
        {
            const float maxSlipRatio = 6f;

            slipRadio = Mathf.Clamp(slipRadio, 0f, maxSlipRatio);

            return slipRadio / maxSlipRatio;
        }

        static float ToKmSec(float v)
        {
            return v * 3.6f;
        }
    }
}