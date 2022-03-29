using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Animations
{
    public sealed class RotateAnimation : MonoBehaviour
    {
        [SerializeField] Vector3 _speed;

        private void Update()
        {
            transform.Rotate(_speed * Time.deltaTime, Space.Self);
        }
    }
}
