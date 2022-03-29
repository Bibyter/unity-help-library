using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Animations
{
    public sealed class BounceAnimation : MonoBehaviour
    {
        [SerializeField] Vector3 _minScale = Vector3.one, _maxScale = Vector3.one;
        [SerializeField] float _speed = 1f;

        float _time;
        const float _timeRound = Mathf.PI * 2f;

        private void LateUpdate()
        {
            float deltaTime = Time.deltaTime * _speed;
            _time = (_time + deltaTime) % _timeRound;
            float lerpTime = 0.5f + (Mathf.Sin(_time) * 0.5f);
            transform.localScale = Vector3.Lerp(_minScale, _maxScale, lerpTime);
        }
    }
}
