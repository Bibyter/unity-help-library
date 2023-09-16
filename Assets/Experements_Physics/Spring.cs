using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Experements_Physics
{
    // F = k (L - r)
    // L - текущая длина пружины
    // k - упругость, жесткость в кг/с^2
    // r - длина в покое

    // так-как пружина бесконечно колебается, нужен амортизатор(damper) который будет эти колебания гасить
    public class Spring : MonoBehaviour
    {
        [SerializeField] Rigidbody _rigidbody;
        [SerializeField] float _restLength;
        [SerializeField] float _stiffness; //  кг/с^2
        [SerializeField] float _damper;  //  кг/с^2

        float _prevLength;

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, _restLength);
        }

        private void FixedUpdate()
        {
            float L = Vector3.Distance(transform.position, _rigidbody.position);
            float r = _restLength;
            float k = _stiffness;

            float springForce = k * (L - r);
            float damperForce = _damper * ((_prevLength - L) / Time.fixedDeltaTime);
            float totalForce = springForce - damperForce;

            _prevLength = L;

            //_rigidbody.AddForce((transform.position - _rigidbody.position).normalized * totalForce);
            _rigidbody.AddForce((transform.position - _rigidbody.position).normalized * totalForce);
        }
    }
}