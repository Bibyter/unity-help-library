using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WheelInertia
{
    public class Temp : MonoBehaviour
    {
        [SerializeField] WheelCollider _wheelCollider;

        private void OnDrawGizmos()
        {
            UnityEditor.Handles.Label(_wheelCollider.transform.position, _wheelCollider.rpm.ToString("0"));
        }

        void FixedUpdate()
        {
            float torque = 1f;

            _wheelCollider.motorTorque = torque;

            print($"{_wheelCollider.rpm}={GetCustomRpm()}");

            CustomRb(torque);
        }

        float customAngularVelocity;

        void CustomRb(float torque)
        {
            float acceleration = torque / GetInertia();
            customAngularVelocity += acceleration * Time.fixedDeltaTime;
        }

        float GetCustomRpm()
        {
            float obsec = customAngularVelocity / (Mathf.PI * 2f);
            float obmin = obsec * 60f;
            return obmin;
        }

        float GetInertia()
        {
            return 0.5f * _wheelCollider.mass * (_wheelCollider.radius * _wheelCollider.radius);
        }
    }
}