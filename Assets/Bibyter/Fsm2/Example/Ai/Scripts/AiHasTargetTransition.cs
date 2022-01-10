using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Fsm2.Behaviours.Ai
{
    public sealed class AiHasTargetTransition : StateBehaviour
    {
        [SerializeField] bool _hasTarget;
        [SerializeField] string _toState;

        IAiTargetFinder _aiTargetFinder;

        public override void Update()
        {
            if (_aiTargetFinder.hasTarget == _hasTarget)
            {
                SetState(_toState);
            }
        }
    }
}