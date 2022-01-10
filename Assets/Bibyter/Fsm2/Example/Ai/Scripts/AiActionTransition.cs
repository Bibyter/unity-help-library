using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Fsm2.Behaviours.Ai
{
    public sealed class AiActionTransition : StateBehaviour
    {
        [SerializeField] AiActionState _value;
        [SerializeField] string _toState;

        AiData _aiData;

        public override void Awake(IInjector injector)
        {
            _aiData = injector.GetInternalLink<AiData>();
        }

        public override void Update()
        {
            if (_aiData.actionState == _value)
            {
                SetState(_toState);
            }
        }
    }
}