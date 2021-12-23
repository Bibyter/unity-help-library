using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Fsm2.Behaviours
{
    public sealed class TimerTransition : StateBehaviour
    {
        [SerializeField] float _delay = 1f;
        [SerializeField] string _toState;

        public override void Update()
        {
            if (GetStateTime() >= _delay)
            {
                SetState(_toState);
            }
        }
    }
}