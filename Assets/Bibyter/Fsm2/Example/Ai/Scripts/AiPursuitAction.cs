using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Bibyter.Fsm2.Behaviours.Ai
{
    public sealed class AiPursuitAction : StateBehaviour
    {
        [SerializeField] string _failedToState;
        [SerializeField] string _completeToState;

        NavMeshAgent _navAnent;
        Animator _animator;

        Vector3 _destination;

        IAiTargetFinder _aiTargetFinder;


        public override void Awake(IInjector injector)
        {
            _aiTargetFinder = injector.GetInternalLink<IAiTargetFinder>();
            _navAnent = injector.GetInternalLink<NavMeshAgent>();
            _animator = injector.GetInternalLink<Animator>();
        }

        public override void Enter()
        {
            _animator.SetBool("walk", true);
            _destination = _aiTargetFinder.target.position;
            _navAnent.SetDestination(_destination);
        }

        public override void Update()
        {
            if (_aiTargetFinder.hasTarget)
            {
                var targetPosition = _aiTargetFinder.target.position;

                if (Vector3.Distance(_destination, targetPosition) > 1.5f)
                {
                    _destination = targetPosition;
                    _navAnent.SetDestination(_destination);
                }

                if (_navAnent.remainingDistance < 2f)
                {
                    SetState(_completeToState);
                }
            }
            else
            {
                SetState(_failedToState);
            }
        }
    }

    public static class AiTargetExtension
    {
        public static bool IsAlive(this IAiTarget target)
        {
            return target != null && target.isAlive;
        }
    }

    public interface IAiTarget
    {
        bool isAlive { get; }
        UnityEngine.Transform transform { get; }
        UnityEngine.Vector3 position { get; }
    }

    public enum AiActionState { None, Run, Failed, Complete }

    [System.Serializable]
    public sealed class AiData
    {
        public AiActionState actionState;

        public Vector3 destination;

        public float pursuitStopDistance = 1f;
        public float attackDistance = 1.1f;
    }
}