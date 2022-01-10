using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Projectiles
{

    public interface IProjectileListener
    {
        void OnCollision(Collider collider, Vector3 point);
    }

    public sealed class Projectile : MonoBehaviour
    {
        [SerializeField] Vector3 _gravity = new Vector3(0f, -5f, 0f);
        [SerializeField] bool _needRotationUpdate;
        [SerializeField] LayerMask _staticMask;
        [SerializeField] LayerMask _dynamicMask;

        //FxService _fxService;
        Vector3 _velocity;
        Vector3 _position;

        static Collider[] _cacheColliders = new Collider[16];

        private void Awake()
        {
            var injector = GetComponent<IInjector>();
            //_fxService = injector.GetExternalLink<FxService>();
        }

        private void OnEnable()
        {
            _position = transform.position;
            CollisionEnter();

            Destroy(gameObject, 10f);
        }

        private void Update()
        {
            Move(Time.deltaTime);
            CollisionUpdate(Time.deltaTime);
        }

        private void Move(float deltaTime)
        {
            _velocity += _gravity * deltaTime;
            _position += _velocity * deltaTime;


            if (_position.magnitude > 6000f)
            {
                Destroy();
                return;
            }

            if (_needRotationUpdate)
            {
                transform.SetPositionAndRotation(_position, Quaternion.LookRotation(_velocity, Vector3.up));
            }
            else
            {
                transform.position = _position;
            }
        }

        #region Collision
        float _collisionTime;
        Vector3 _collisionLastPoint;

        private void CollisionEnter()
        {
            _collisionLastPoint = _position;
        }

        private void CollisionUpdate(float deltaTime)
        {
            _collisionTime += deltaTime;

            if (_collisionTime > 0.05f)
            {
                _collisionTime %= 0.05f;
                DetectCollision();
                _collisionLastPoint = _position;
            }
        }

        void DetectCollision()
        {
#if UNITY_EDITOR
            Debug.DrawLine(_collisionLastPoint, _position, Color.red);
#endif
            
            Collider overlapResult = null;
            Vector3 overlapPoint = Vector3.zero;
            {
                var count = Physics.OverlapCapsuleNonAlloc(_collisionLastPoint, _position, 0.15f, _cacheColliders, _dynamicMask);
                var minDistance = float.MaxValue;
                for (int i = 0; i < count; i++)
                {
                    var delta = _collisionLastPoint - _position;
                    var rayOrigin = _position + (delta.normalized * 100f);

                    //Debug.DrawRay(ray.origin, ray.direction * boundsMagnitude, Color.yellow);
                    //Debug.Break();

                    var closestPoint = _cacheColliders[i].ClosestPointOnBounds(rayOrigin);
                    var tempDistance = Vector3.Distance(closestPoint, rayOrigin);

                    if (tempDistance < minDistance)
                    {
                        minDistance = tempDistance;
                        overlapPoint = closestPoint;
                        overlapResult = _cacheColliders[i];
                    }
                }
            }

            if (overlapResult != null)
            {
                if (TryGetComponent(out IProjectileListener listener))
                {
                    listener.OnCollision(overlapResult, overlapPoint);
                }

                Destroy();
            }
            else if (Physics.Linecast(_collisionLastPoint, _position, out var hitInfo, _staticMask))
            {

                if (TryGetComponent(out IProjectileListener listener))
                {
                    listener.OnCollision(hitInfo.collider, hitInfo.point);
                }

                Destroy();
            }


        }
        #endregion

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public Vector3 velocity
        {
            set { _velocity = value; }
        }

        public void SetVelocityToPoint(Vector3 point, float speedMultiplier)
        {
            var duration = Vector3.Distance(point, _position) * 0.05f * speedMultiplier;
            velocity = Bibyter.BallisticsExtension.GetVelocity(_position, point, _gravity, duration);
        }
    }
}