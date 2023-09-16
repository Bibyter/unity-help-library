using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Example_Car3
{
    public class Rigidbody : MonoBehaviour
    {
        [SerializeField] Vector3 _gravity;
        [SerializeField] float _environmentDrag;
        [SerializeField] float _mass;

        public UnityEngine.Rigidbody rigidbodyy;

        public Vector3 _velocity;
        Vector3 _position;

        private void OnEnable()
        {
            _velocity = Vector3.zero;
            _position = transform.position;
        }

        void FixedUpdate()
        {
            rigidbodyy.AddForce(Vector3.right);

            Vector3 environmentDragForce = -(_environmentDrag * _velocity * _velocity.magnitude);

            Vector3 totalForce = Vector3.right;

            Vector3 acceleration = (totalForce / _mass) + _gravity;

            _velocity += acceleration * Time.deltaTime;

            _velocity = Vector3Drag(_velocity, _environmentDrag, Time.deltaTime);

            _position += _velocity * Time.deltaTime;

            transform.position = _position;
        }

        public static Vector3 Vector3Drag(Vector3 source, float drag, float deltaTime)
        {
            return Vector3.MoveTowards(source, Vector3.zero, source.magnitude * deltaTime * drag);
        }
    }
}