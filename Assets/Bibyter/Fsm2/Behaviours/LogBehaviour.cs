using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Fsm2.Behaviours
{
    public sealed class LogBehaviour : StateBehaviour
    {
        [SerializeField] string _enterMessage;
        [SerializeField] string _exitMessage;

        public override void Enter()
        {
            Debug.Log(_enterMessage);
        }

        public override void Exit()
        {
            Debug.Log(_exitMessage);
        }
    }
}
