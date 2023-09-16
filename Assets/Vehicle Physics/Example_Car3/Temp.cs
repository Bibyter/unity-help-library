using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Example_Car3
{
    public class Temp : MonoBehaviour
    {
        [SerializeField] float _leftRpm;
        [SerializeField] float _rightRpm;

        [SerializeField] float _leftTorque;
        [SerializeField] float _rightTorque;

        private void OnValidate()
        {
            float leftWheelAngularVelocity = Mathf.Pow(_leftRpm, 3f);
            float rightWheelAngularVelocity = Mathf.Pow(_rightRpm, 3f);

            float totalAngularVelocity = leftWheelAngularVelocity + rightWheelAngularVelocity;

            float differentialLeftMultiplier = rightWheelAngularVelocity / totalAngularVelocity;
            float differentialRightMultiplier = leftWheelAngularVelocity / totalAngularVelocity;


            _leftTorque = 1f * differentialLeftMultiplier;
            _rightTorque = 1f * differentialRightMultiplier;
        }
    }
}