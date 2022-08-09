using UnityEngine;
using UnityEngine.AI;

namespace Client.Ai
{
    public sealed class IdleAction : AiAction
    {
        Animator _animator;
        NavMeshAgent _navAgent;

        public override void Start(AiContainer container)
        {
            _animator = container.Get<Animator>();
            _navAgent = container.Get<NavMeshAgent>();
        }

        public override void OnEnter()
        {
            _animator.SetBool("walk", false);
            _navAgent.ResetPath();
        }
    }
}
