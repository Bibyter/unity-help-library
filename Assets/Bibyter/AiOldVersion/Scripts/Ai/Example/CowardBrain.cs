using UnityEngine;

namespace Client.Ai.Example
{
    public sealed class CowardBrain : IAiBrain
    {
        int _state;
        AiData _aiData;
        AiController _aiController;

        public CowardBrain(AiData aiData, AiController aiController)
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
                _aiController.SetState<IdleAction>();
                _aiData.moveType = AiData.MoveType.Run;
                _state++;
            }

            if (_state == 1)
            {
                if (_aiData.targetDistance < 3f)
                {
                    _aiController.SetState<MoveToAction>();
                    _aiData.destination = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
                    _state++;
                }
            }
            else if (_state == 2)
            {
                if (_aiData.actionState == AiActionState.Complete)
                {
                    _aiController.SetState<IdleAction>();
                    _state--;
                }
            }

            
        }

        public void Exit()
        {
        }
    }
}
