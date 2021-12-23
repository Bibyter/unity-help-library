using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Fsm2
{
    [System.Serializable]
    public sealed class FsmController
    {
        [SerializeField] State _attachedFsmAsset;
        State _runtimeState;

        public void Awake(IInjector injector)
        {
            if (_attachedFsmAsset != null)
            {
                _runtimeState = _attachedFsmAsset.Clone();
                _runtimeState.Awake2(injector);
            }
        }

        bool _enabled;
        public bool enabled
        {
            set
            {
                if (_enabled != value && _runtimeState != null)
                {
                    if (value) _runtimeState.Enter(); else _runtimeState.Exit();
                    _enabled = value;
                }
            }
            get => _enabled;
        }

        public void Update()
        {
            if (_runtimeState != null)
                _runtimeState.Update();
        }
    }
}
