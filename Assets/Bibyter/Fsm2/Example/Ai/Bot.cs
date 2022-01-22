using Bibyter.Fsm2.Behaviours.Ai;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Bibyter.Fsm2.Ai
{
    public sealed class Bot : MonoBehaviour
    {
        [SerializeField] FsmController _fsmController;

        [SerializeField]
        SharedObjectNs.BaseVariables.StringLocalVariable _stringLocalVariable;

        public SharedObjectNs.BaseVariables.IntLocalVariable intLocalVariable;
        public SharedObjectNs.BaseVariables.FloatLocalVariable floatLocalVariable;

        private void Awake()
        {
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