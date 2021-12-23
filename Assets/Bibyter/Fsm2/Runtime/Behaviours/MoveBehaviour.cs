using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Fsm2
{
    public sealed class MoveBehaviour : StateBehaviour
    {
        [SerializeField] Vector3 _speed;

        Transform _transform;

        public override void Awake(IInjector injector)
        {
            _transform = injector.GetInternalLink<Transform>();
        }

        public override void Update()
        {
            _transform.position += _speed * Time.deltaTime;
            base.Update();
        }
    }
}