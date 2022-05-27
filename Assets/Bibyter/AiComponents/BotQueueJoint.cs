using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Environment
{
    /// <summary>
    /// Соединяет несколько очередей (первая точка) в одну (последняя точка)
    /// Пропускает последовательно и зацикленно
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public sealed class BotQueueJoint : MonoBehaviour
    {
        [SerializeField] BotQueue _targetQueue;
        [SerializeField] BotQueue[] _queueJoints;

        int _lastQueueId;

        private void OnValidate()
        {
            if (GetComponent<BotQueue>() != null)
                _targetQueue = GetComponent<BotQueue>();
        }

        private void OnEnable()
        {
            for (int i = 0; i < _queueJoints.Length; i++)
            {
                _queueJoints[i].botQueueJoint = this;
            }
        }

        private void OnDisable()
        {
            for (int i = 0; i < _queueJoints.Length; i++)
            {
                _queueJoints[i].botQueueJoint = null;
            }
        }

        private int GetNextJointsQueueId()
        {
            for (int i = 0; i < _queueJoints.Length; i++)
            {
                int queueId = (i + _lastQueueId + 1) % _queueJoints.Length;

                if (_queueJoints[queueId].GetFirstPlace().HasOwner())
                {
                    return queueId;
                }
            }

            return -1;
        }

        public BotQueue.Place GetPlace(BotQueue botQueue)
        {
            var nextQueueId = GetNextJointsQueueId();
            var nextQueue = nextQueueId == -1 ? null : _queueJoints[nextQueueId];

            if (nextQueue != null && nextQueue == botQueue)
            {
                var targetNextPlace = _targetQueue.GetLastPlace();

                if (targetNextPlace != null && !targetNextPlace.HasOwner())
                {
                    _lastQueueId = nextQueueId;
                    return targetNextPlace;
                }
            }

            return null;
        }
    }
}
