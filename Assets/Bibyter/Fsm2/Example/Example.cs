using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Fsm2.Example
{
    public sealed class Example : MonoBehaviour
    {
        [SerializeField] FsmController _fsmController;

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