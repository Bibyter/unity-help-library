using UnityEngine;

namespace Client.Ai
{
    public interface IAiBrain
    {
        void Enter();
        void Exit();
        void Execute();
    }

    public sealed class AiBrainController
    {
        IAiBrain _current;
        IAiBrain _next;

        public void SetBrain(IAiBrain brain)
        {
            _next = brain;
        }

        public void ForceExit()
        {
            _current?.Exit();
            _current = null;
        }

        public void Update()
        {
            if (_next != null)
            {
                _current?.Exit();
                _next.Enter();
                _current = _next;
                _next = null;
            }

            if (_current != null)
            {
                _current.Execute();
            }
        }
    }
}
