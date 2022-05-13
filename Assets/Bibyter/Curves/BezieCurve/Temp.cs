using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.BezieCurve
{
    public class Temp : MonoBehaviour
    {
        public float angle;

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(angle, transform.forward) * Vector3.forward * 5f);
        }
    }
}