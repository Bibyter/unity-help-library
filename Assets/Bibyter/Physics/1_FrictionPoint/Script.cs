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

            public void Update(Rigidbody2D rigidbody, Transform transform, int frictionPointCount)
            {
                // ��������� ������
                // ����� ����� ����������� ������ � �������� (���������)
                // friction = -1 * mu * N * norm(v)
                // mu - ����������� ������, ������
                // N - ���� ������� �����, ������� ������� - ���� ���������������� ���� ��������������� ��������, ������
                // N = m*g
                // norm(v) - ����������� ������, ��������������� ��������, ������

                const float mu = 0.9f;
                float N = (rigidbody.mass * (1f / frictionPointCount)) * 10f; // ������-�� � unity �������� �� ���������� �� ����

                Vector2 worldFrictionPoint = transform.TransformPoint(_pivot);

                Vector2 dir = rigidbody.GetPointVelocity(worldFrictionPoint);

                if (dir.magnitude > 1f)
                    dir.Normalize();

                Vector2 frictionForce = -1f * mu * N * dir;

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
                _frictionPoints[i].Update(_rigidbody, transform, _frictionPoints.Length);
            }
        }
    }
}