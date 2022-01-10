using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Fsm2.Behaviours.Ai
{
    public sealed class AiSetRandomDestinationAction : StateBehaviour
    {
        [SerializeField] Vector2 _min, _max;

        AiData _aiData;

        public override void Awake(IInjector injector)
        {
            _aiData = injector.GetInternalLink<AiData>();
        }

        public override void Enter()
        {
            _aiData.destination = new Vector3(Random.Range(_min.x, _max.x), 0f, Random.Range(_min.y, _max.y));
        }
    }
}