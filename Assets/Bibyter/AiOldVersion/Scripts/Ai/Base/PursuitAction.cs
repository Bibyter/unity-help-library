using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Client.Ai
{
    public enum PursuitState
    {
        None, Move, Complete
    }

    public sealed class PursuitAction : AiAction
    {
        AiData _data;
        NavMeshAgent _navAnent;
        Animator _animator;

        bool _isMove;
        bool _needInit;


        public override void Start(AiContainer container)
        {
            _data = container.Get<AiData>();
            _navAnent = container.Get<NavMeshAgent>();
            _animator = container.Get<Animator>();
        }

        public override void OnEnter()
        {
            float distance = _data.hasTarget ? Vector3.Distance(_navAnent.nextPosition, _data.target.position) : 0f;
            _isMove = distance > _data.pursuitStopDistance;
            _needInit = true;
            _data.actionState = AiActionState.None;
        }

        public override void Update()
        {
            float distance = _data.hasTarget ? Vector3.Distance(_navAnent.nextPosition, _data.target.position) : 0f;

            if (_isMove)
            {
                if (_needInit)
                {
                    _animator.SetBool("walk", true);
                    _navAnent.SetDestination(_data.target.position);
                    _data.actionState = AiActionState.Run;
                    _needInit = false;
                }

                if (distance < _data.pursuitStopDistance || !_data.hasTarget)
                {
                    _isMove = false;
                    _needInit = true;
                }
                else if (_navAnent.remainingDistance < 0.3f)
                {
                    _navAnent.SetDestination(_data.target.position);
                }
            }
            else
            {
                if (_needInit)
                {
                    _animator.SetBool("walk", false);
                    _navAnent.ResetPath();
                    _data.actionState = AiActionState.Complete;
                    _needInit = false;
                }

                if (distance > (_data.pursuitStopDistance + 0.2f) && _data.hasTarget)
                {
                    _needInit = true;
                    _isMove = true;
                }
            }
        }

        public override void OnExit()
        {
        }

    }

}
