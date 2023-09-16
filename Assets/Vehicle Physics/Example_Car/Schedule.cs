using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Example_Car
{
    public sealed class Schedule : MonoBehaviour
    {
        [SerializeField] AnimationCurve _powerCurve;
        [SerializeField] AnimationCurve _torqoeCurve;

        public float engineRPM;
        public float BackTorque = 0.1f;

        [Range(0f, 1f)]
        public float Throttle;
        [Range(0f, 1f)]
        public float Clutch;

        Engine2 _engine;

        private void Awake()
        {
            _engine = new Engine2(_torqoeCurve, _powerCurve);
        }

        private void Update()
        {
            _engine.BackTorque = BackTorque;
            _engine.Progress(Throttle, Clutch, Time.deltaTime);
            engineRPM = _engine.rpm;
        }
    }

    sealed class Engine2
    {
        /// <summary>
        /// �������� ������ (������ �� ����)
        /// </summary>
        AnimationCurve Torque;
        /// <summary>
        /// �������� (��������� ����)
        /// </summary>
        AnimationCurve Power;

        /// <summary>
        /// ���������� ����������� ���������
        /// </summary>
        float Inertia = 1f;

        float Mass;

        /// <summary>
        /// ���������, ������������ �������� ��������� �������.
        /// </summary>
        public float BackTorque = 0.1f;
        char Direction;
        /// <summary>
        /// // ������� ��������� ����
        /// </summary>
        float IdleRPM = 800f; 

        // ������������ ��������
        float RPM;
        public float rpm => RPM;

        public Engine2(AnimationCurve torque, AnimationCurve power)
        {
            Torque = torque;
            Power = power;
            RPM = IdleRPM;
        }

        /// <summary>
        /// ������� ���������� 
        /// </summary>
        /// <param name="Throttle">�������� �� 0 �� 1 � ��������</param>
        /// <param name="Clutch">�������� �� 0 �� 1 � ������ ���������</param>
        public void Progress(float Throttle, float Clutch, float deltaTime)
        {
            //if (RPM < IdleRPM && Throttle < 0.3f)
            //    Throttle += 0.1f;

            float torq = Torque.Evaluate(RPM) * Throttle;

            float additionRPM = torq - (Mathf.Pow(1.0f - Throttle, 2f) * BackTorque);
            RPM = Mathf.Max(RPM + additionRPM, IdleRPM);// additionRPM;
        }
    };
}