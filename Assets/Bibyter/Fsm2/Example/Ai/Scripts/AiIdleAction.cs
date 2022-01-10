using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Bibyter.Fsm2.Behaviours.Ai
{
    public sealed class AiIdleAction : StateBehaviour
    {
        [SerializeField] string _animationName = "idle";

        Animator _animator;
        NavMeshAgent _navAgent;

        public override void Awake(IInjector injector)
        {
            _animator = injector.GetInternalLink<Animator>();
            _navAgent = injector.GetInternalLink<NavMeshAgent>();
        }

        public override void Enter()
        {
            _animator.CrossFadeInFixedTime(_animationName, 0.25f);
            _navAgent.ResetPath();
        }
    }
}