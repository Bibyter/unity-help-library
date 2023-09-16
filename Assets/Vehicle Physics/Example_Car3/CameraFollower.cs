using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Example_Car3
{
    public class CameraFollower : MonoBehaviour
    {
        [SerializeField] Transform _target;

        Vector3 _offset;
        float _rotation;

        private void OnEnable()
        {
            _offset = transform.position - _target.position;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                _rotation += Input.GetAxis("Mouse X") * 3f;
            }
        }

        void LateUpdate()
        {
            transform.position = _target.position + Quaternion.Euler(0f, _rotation, 0f) * _offset;

            transform.LookAt(_target.position + Vector3.up);
        }
    }
}