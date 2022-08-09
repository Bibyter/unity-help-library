using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Ai.Example
{
    public sealed class AttackerBrain : IAiBrain
    {
        int _state;
        AiData _aiData;
        AiController _aiController;


        public AttackerBrain(AiData aiData, AiController aiController)
        {
            _aiData = aiData;
            _aiController = aiController;
        }

        public void Enter()
        {
            _state = 0;
            _aiController.SetState<IdleAction>();
        }

        public void Execute()
        {
            if (_state == 0 && _aiData.hasTarget)
            {
                _aiController.SetState<PursuitAction>();
                _state++;
            }

            if (_state == 1)
            {
                if (_aiData.actionState == AiActionState.Complete)
                {
                    _aiController.SetState<Attack2Action>();
                    _state++;
                }
            }
            else if (_state == 2)
            {
                if (_aiData.actionState == AiActionState.Failed)
                {
                    _aiController.SetState<PursuitAction>();
                    _state--;
                }
            }
        }

        public void Exit()
        {
        }
    }
}
