using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Projectiles.Examples
{
    public sealed class Example : MonoBehaviour
    {
        [SerializeField] Projectile _projectilePrefab;
        [SerializeField] float _force = 1f;
        [SerializeField] float _shotPeriod = 1f;

        // Start is called before the first frame update
        void Start()
        {
            InvokeRepeating(nameof(Shot), 1f, _shotPeriod);
        }

        void Shot()
        {
            var instance = Instantiate(_projectilePrefab, transform.position, Quaternion.identity, null);
            instance.velocity = transform.forward * _force;
        }
    }
}