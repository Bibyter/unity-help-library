using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Client.Ai
{
    public sealed class MoveToAction : AiAction
    {
        AiData _data;
        NavMeshAgent _navAnent;
        Animator _animator;

        bool _complete;
        string _moveKey;

        public override void Start(AiContainer container)
        {
            _data = container.Get<AiData>();
            _navAnent = container.Get<NavMeshAgent>();
            _animator = container.Get<Animator>();
        }

        public override void OnEnter()
        {
            _complete = false;
            _moveKey = _data.GetMoveAnimKey();
            _animator.SetBool(_moveKey, true);
            _navAnent.SetDestination(_data.destination);
            _data.actionState = AiActionState.Run;
        }

        public override void Update()
        {
            if (!_complete)
            {
                if (_navAnent.remainingDistance < 0.5f)
                {
                    _data.actionState = AiActionState.Complete;
                    _animator.SetBool(_moveKey, false);
                    _navAnent.ResetPath();
                    _complete = true;
                }
            }

        }
    }
}
