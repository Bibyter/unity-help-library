using Bibyter.Mathematics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Example_Car2
{
    public class WheelView : MonoBehaviour
    {
        [SerializeField] float _radius = 1f;
        [SerializeField] Transform _rotationZPivot;

        float _rotationZ;

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, _radius);
        }

        public void UpdateRotationZ(float linearSpeedDelta)
        {
            _rotationZ += Circle.ConvertLinearToAngularSpeed(linearSpeedDelta, _radius);

            _rotationZPivot.localEulerAngles = new Vector3(_rotationZ * Mathf.Rad2Deg, 0f, 0f);
        }
    }
}