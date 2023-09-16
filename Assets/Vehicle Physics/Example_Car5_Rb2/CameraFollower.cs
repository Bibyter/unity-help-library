using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Example_Car5_Rb2
{
    public class CameraFollower : MonoBehaviour
    {
        [SerializeField] Transform _target;

        float _offsetz;

        private void OnEnable()
        {
            _offsetz = transform.position.z;
        }

        void LateUpdate()
        {
            var position = transform.position;
            position = Vector3.Lerp(position, _target.position, Time.deltaTime * 5f);
            position.z = _offsetz;
            transform.position = position;
        }
    }
}