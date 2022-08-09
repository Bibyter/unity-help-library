using UnityEngine;
using UnityEngine.AI;

namespace Client.Ai
{
    public interface IFaceRotation
    {
        void Start(Vector3 direction);
        void Stop();
    }
}

namespace Client.Ai.Example
{
    public sealed class TestAiBehaviour : MonoBehaviour, IFaceRotation, IAiTarget
    {
        public enum AiType { Attacker, Coward }
        [SerializeField] AiType _startAiType;

        [SerializeField] Animator _animator;
        [SerializeField] NavMeshAgent _navAgent;
        [SerializeField] AiTargetFinder _targetFinder;
        [Space]
        [SerializeField] float _attackDistance = 1.5f;
        [SerializeField] float _pursuitStopDistance = 1.5f;
        [Space]
        [SerializeField] bool _aiControllerNeedUpdateInTransition;

        AiController _aiController;
        AiData _aiData;
        AiBrainController _aiBrainController;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackDistance);
        }

        void Start()
        {
            var data = new AiData();
            data.attackDistance = _attackDistance;
            data.pursuitStopDistance = _pursuitStopDistance;
            _aiData = data;

            var container = new AiContainer();
            container.Add(_animator);
            container.Add(_navAgent);
            container.Add(data);
            container.Add(this);

            _aiController = new AiController(container, _aiControllerNeedUpdateInTransition);

            _aiBrainController = new AiBrainController();
            SetBrain(_startAiType);
        }

        void Update()
        {
            AiUpdate();
            RotationUpdate();
        }

        void AiUpdate()
        {
            AiDataUpdate();
            _aiBrainController.Update();
            _aiController.Update();
        }

        void AiDataUpdate()
        {
            var position = _navAgent.nextPosition;
            _aiData.hasTarget = _targetFinder.hasTarget;
            _aiData.target = _targetFinder.target;
            _aiData.targetDistance = _aiData.hasTarget ? Vector3.Distance(position, _targetFinder.target.position) : float.MaxValue;
        }

        void SetBrain(AiType aiType)
        {
            switch (aiType)
            {
                case AiType.Attacker:
                    _aiBrainController.SetBrain(new AttackerBrain(_aiData, _aiController));
                    break;
                case AiType.Coward:
                    _aiBrainController.SetBrain(new CowardBrain(_aiData, _aiController));
                    break;
            }
        }

        #region
        bool _activeFaceRotation;
        Quaternion _faceRotation;

        void RotationUpdate()
        {
            if (_activeFaceRotation)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _faceRotation, Time.deltaTime * 360f);
            }
        }

        void IFaceRotation.Start(Vector3 direction)
        {
            _activeFaceRotation = true;

            direction.y = 0f;
            _faceRotation = Quaternion.LookRotation(direction);
        }

        void IFaceRotation.Stop()
        {
            _activeFaceRotation = false;
        }
        #endregion

        #region
        bool IAiTarget.isAlive => true;

        Transform IAiTarget.transform => transform;

        Vector3 IAiTarget.position => transform.position;
        #endregion
    }
}
