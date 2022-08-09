using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Ai
{
    public sealed class AiTargetFinder : MonoBehaviour
    {
        [SerializeField, Range(0f, 360f)] float _visionAngle = 30f;
        [SerializeField] float _visionRange = 3f;

        // target Retention Time in outside vision range
        [SerializeField] float _targetRetentionTime = 3f;

        List<IAiTarget> _targets;
        float _targetNotVisionTime;

        IAiTarget _currentTarget;
        public bool hasTarget => _currentTarget != null;
        public IAiTarget target => _currentTarget;

        private void Awake()
        {
            _targets = new List<IAiTarget>(4);
            _currentTarget = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IAiTarget target))
            {
                _targets.Add(target);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IAiTarget target))
            {
                _targets.Remove(target);
            }
        }

        private void Update()
        {
            if (_currentTarget != null)
            {
                if (IsVision(_currentTarget.position))
                {
                    _targetNotVisionTime = 0f;
                }
                else
                {
                    _targetNotVisionTime += Time.deltaTime;
                }

                if (!_currentTarget.isAlive || _targetNotVisionTime > _targetRetentionTime)
                {
                    _currentTarget = null;
                }
            }

            if (_currentTarget == null)
            {
                _currentTarget = FindMeleeTarget();
            }
        }

        IAiTarget FindMeleeTarget()
        {
            float minDistance = float.MaxValue;
            IAiTarget target = null;
            var selfPosition = transform.position;
            var selfForward = transform.forward;

            for (int i = 0; i < _targets.Count; i++)
            {
                if (_targets[i] == null)
                {
                    _targets.RemoveAt(i);
                    i--;
                    continue;
                }

                if (!_targets[i].isAlive) continue;

                var targetPosition = _targets[i].position;

                if (!IsVision(selfPosition, selfForward, targetPosition)) continue;

                var distnace = Vector3.Distance(selfPosition, targetPosition);

                if (distnace < minDistance)
                {
                    minDistance = distnace;
                    target = _targets[i];
                }
            }

            return target;
        }

        public bool IsVision(in Vector3 position)
        {
            return IsVision(transform.position, transform.forward, position);
        }

        bool IsVision(in Vector3 selfPosition, in Vector3 selfForward, in Vector3 targetPosition)
        {
            var dir = targetPosition - selfPosition;
            dir.y = 0f;

            float dirMagnitude = dir.magnitude;

            if (dirMagnitude > _visionRange || dirMagnitude <= 0.001f)
                return false;

            float angle = Vector3.Angle(dir, selfForward);
            return angle < _visionAngle * 0.5f;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var forward = transform.TransformDirection(Quaternion.Euler(0f, _visionAngle * -0.5f, 0f) * Vector3.forward);
            UnityEditor.Handles.color = new Color(0.8f, 0.5f, 0.5f, 0.5f);
            UnityEditor.Handles.DrawSolidArc(transform.position, transform.up, forward, _visionAngle, _visionRange);

            if (_targets != null)
            {
                for (int i = 0; i < _targets.Count; i++)
                {
                    if (_targets[i] == null) return;
                    Gizmos.color = IsVision(_targets[i].position) ? Color.red : Color.gray;
                    Gizmos.DrawLine(_targets[i].position, transform.position);
                }
            }
        }

        private void OnValidate()
        {
            _visionRange = Mathf.Max(_visionRange, 0f);

            if (TryGetComponent(out SphereCollider sphereCollider))
            {
                sphereCollider.radius = _visionRange;
            }
        }
#endif
    }
}
