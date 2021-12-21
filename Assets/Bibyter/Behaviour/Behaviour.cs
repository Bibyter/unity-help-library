using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter
{
    [System.Serializable]
    public class Behaviour
    {
        bool _enabled;
        public bool enabled
        {
            set
            {
                if (_enabled != value)
                {
                    if (value) Enter(); else Exit();
                    _enabled = value;
                }
            }
            get => _enabled;
        }

        public void Run()
        {
            if (_enabled) Update();
        }

        public virtual void Awake(IInjector injector) { }
        protected virtual void Enter() { }
        protected virtual void Exit() { }
        protected virtual void Update() { }
    }
}