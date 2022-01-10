using Bibyter.Fsm2.Behaviours.Ai;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Bibyter.Fsm2.Ai
{
    public sealed class Bot : MonoBehaviour
    {
        [SerializeField] Animator _animator;
        [SerializeField] NavMeshAgent _navMeshAgent;

        [SerializeField] FsmController _fsmController;
        [SerializeField] AiData _aiData;


        private void Awake()
        {
            GetComponent<SharedObject>().AddInterLink(_animator);
            GetComponent<SharedObject>().AddInterLink(_aiData);
            GetComponent<SharedObject>().AddInterLink(_navMeshAgent);
            _fsmController.Awake(GetComponent<IInjector>());
        }

        private void OnEnable()
        {
            _fsmController.enabled = true;
        }

        private void OnDisable()
        {
            _fsmController.enabled = false;
        }

        private void Update()
        {
            _fsmController.Update();
        }
    }
}