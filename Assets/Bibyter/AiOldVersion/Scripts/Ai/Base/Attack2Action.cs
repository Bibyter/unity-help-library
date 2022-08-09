using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Client.Ai
{
    public sealed class Attack2Action : AiAction
    {
        AiData _data;
        NavMeshAgent _navAnent;
        Animator _animator;
        IFaceRotation _faceRotation;

        bool _isAttack;
        bool _needInit;


        public override void Start(AiContainer container)
        {
            _data = container.Get<AiData>();
            _navAnent = container.Get<NavMeshAgent>();
            _animator = container.Get<Animator>();
            _faceRotation = container.Get<IFaceRotation>();
        }

        public override void OnEnter()
        {
            _isAttack = _data.targetDistance < _data.attackDistance;
            _needInit = true;

            _data.actionState = AiActionState.None;

            _navAnent.ResetPath();
            _animator.SetBool("walk", false);
        }

        public override void Update()
        {
            var position = _navAnent.nextPosition;
            var targetPosition = _data.hasTarget ? _data.target.position : Vector3.zero;

            if (_isAttack)
            {
                if (_needInit)
                {
                    _data.actionState = AiActionState.Run;
                    _animator.SetBool("attack", true);
                    _needInit = false;
                }

                if (_data.hasTarget)
                    _faceRotation.Start(targetPosition - position);

                if (_data.targetDistance > (_data.attackDistance + 0.3f) || !_data.hasTarget)
                {
                    _isAttack = false;
                    _needInit = true;
                }
            }
            else
            {
                if (_needInit)
                {
                    _data.actionState = AiActionState.Failed;
                    _animator.SetBool("attack", false);
                    _faceRotation.Stop();
                    _needInit = false;
                }

                if (_data.targetDistance < _data.attackDistance && _data.hasTarget)
                {
                    _needInit = true;
                    _isAttack = true;
                }
            }
        }

        public override void OnExit()
        {
            _animator.SetBool("attack", false);
            _faceRotation.Stop();
        }
    }
}
