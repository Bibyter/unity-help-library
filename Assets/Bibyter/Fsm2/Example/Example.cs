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
            var injector = GetComponent<IInjector>();
            var linkRegistrator = GetComponent<InjectionNode>();

            linkRegistrator.AddInterLink(transform);

            _fsmController.Awake(injector);
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