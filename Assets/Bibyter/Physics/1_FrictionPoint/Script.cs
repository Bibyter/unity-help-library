using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.PhysicsNs.FrictionPoint
{
    public sealed class Script : MonoBehaviour
    {
        [SerializeField] Rigidbody2D _rigidbody;

        [SerializeField] FrictionPoint[] _frictionPoints;

        [System.Serializable]
        public struct FrictionPoint
        {
            [SerializeField] Vector2 _pivot;

            public void Update(Rigidbody2D rigidbody, Transform transform)
            {
                // уравнение трени€
                // нужно найти направление трени€ и величину (магнитуду)
                // friction = -1 * mu * N * norm(v)
                // mu - коеффициент трени€, скал€р
                // N - сила реакции опоры, другими словами - сила перпендикул€рна€ двум соприкасающимс€ объектам, скал€р
                // N = m*g
                // norm(v) - направление трени€, нормализованна€ скорость, вектор

                const float mu = 0.5f;
                float N = rigidbody.mass/* * 9.81f*/; // почему-то в unity умножать на гравитацию не надо

                Vector2 dir = rigidbody.GetRelativePointVelocity(_pivot);
                dir.Normalize();

                Vector2 frictionForce = -1f * mu * N * dir;

                Vector2 worldFrictionPoint = transform.TransformPoint(_pivot);
                rigidbody.AddForceAtPosition(frictionForce, worldFrictionPoint);
            }

            public void DrawGizmos(Transform transform)
            {
                Gizmos.DrawWireSphere(transform.TransformPoint(_pivot), 0.2f);
            }
        }

        private void OnDrawGizmos()
        {
            if (_frictionPoints != null)
            {
                for (int i = 0; i < _frictionPoints.Length; i++)
                {
                    _frictionPoints[i].DrawGizmos(transform);
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _rigidbody.AddForce(Vector2.up * 100f, ForceMode2D.Impulse);
            }
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < _frictionPoints.Length; i++)
            {
                _frictionPoints[i].Update(_rigidbody, transform);
            }
        }
    }
}