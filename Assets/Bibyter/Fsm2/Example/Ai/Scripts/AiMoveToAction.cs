using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Bibyter.Fsm2.Behaviours.Ai
{
    public sealed class AiMoveToAction : StateBehaviour
    {
        [SerializeField] string _completeToState;
        [SerializeField] string _animationName = "walk";

        AiData _aiData;
        NavMeshAgent _navAnent;
        Animator _animator;

        public override void Awake(IInjector injector)
        {
            _aiData = injector.GetInternalLink<AiData>();
            _navAnent = injector.GetInternalLink<NavMeshAgent>();
            _animator = injector.GetInternalLink<Animator>();
        }

        public override void Enter()
        {
            _animator.CrossFadeInFixedTime(_animationName, 0.25f);
            _navAnent.SetDestination(_aiData.destination);
        }

        public override void Update()
        {
            if (_navAnent.remainingDistance < 0.5f)
            {
                SetState(_completeToState);
            }
        }

        public override void Exit()
        {
            _navAnent.ResetPath();
        }
    }
}