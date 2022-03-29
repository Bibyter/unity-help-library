using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Animations
{

    public sealed class TwoPointsAnimation : MonoBehaviour
    {
        [SerializeField] Vector3 _pointA, _pointB;
        [SerializeField] float _speed = 1f;

        float _time;

        private void LateUpdate()
        {
            float deltaTime = Time.deltaTime * _speed;
            _time = (_time + deltaTime) % (Mathf.PI * 2f);
            float lerpTime = 0.5f + (Mathf.Sin(_time) * 0.5f);
            transform.localPosition = Vector3.Lerp(_pointA, _pointB, lerpTime);
        }
    }
}