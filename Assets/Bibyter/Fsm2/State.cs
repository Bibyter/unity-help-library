using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Fsm2
{
    [CreateAssetMenu(menuName = "Bibyter/Fsm Asset", fileName = "New Fsm Asset")]
    public sealed class State : ScriptableObject
    {
        [SerializeField, SerializeReference] StateBehaviour[] _behaviours;
        [SerializeField] State[] _childStates;

        State _currentChildState;
        float _enterTime;

        public void Awake2(IInjector injector)
        {
            for (int i = 0; i < _behaviours.Length; i++)
            {
                _behaviours[i].state = this;
                _behaviours[i].Awake(injector);
            }

            for (int i = 0; i < _childStates.Length; i++)
            {
                _childStates[i].Awake2(injector);
            }
        }

        public void Enter()
        {
            for (int i = 0; i < _behaviours.Length; i++)
            {
                _behaviours[i].Enter();
            }

            _enterTime = Time.time;

            // child state entry
            SetChildState(0);
        }

        public void Exit()
        {
            for (int i = 0; i < _behaviours.Length; i++)
            {
                _behaviours[i].Exit();
            }

            // child state exit
            if (_currentChildState != null)
            {
                _currentChildState.Exit();
                _currentChildState = null;
            }
        }

        public void Update()
        {
            for (int i = 0; i < _behaviours.Length; i++)
            {
                _behaviours[i].Update();
            }

            if (_currentChildState != null)
            {
                _currentChildState.Update();
            }
        }

        public void SetState(string name)
        {
            for (int i = 0; i < _childStates.Length; i++)
            {
                if (_childStates[i].name == name)
                {
                    SetChildState(i);
                }
            }
        }

        public float GetActiveTime()
        {
            return Time.time - _enterTime;
        }

        void SetChildState(int id)
        {
            if (_currentChildState != null)
            {
                _currentChildState.Exit();
                _currentChildState = null;
            }

            if (id >= 0 && id < _childStates.Length)
            {
                _currentChildState = _childStates[id];
                _currentChildState.Enter();
            }
        }

        public State Clone()
        {
            var instance = Instantiate(this);
            instance.ChildStatesClone();
            return instance;
        }

        void ChildStatesClone()
        {
            for (int i = 0; i < _childStates.Length; i++)
            {
                _childStates[i] = _childStates[i].Clone();
            }
        }

#if UNITY_EDITOR
        public Vector2 position;

        public int GetChildStatesCount()
        {
            return _childStates.Length;
        }

        public State GetChildState(int i)
        {
            return _childStates[i];
        }

        public void ChildStatesFlip(State a, State b)
        {
            var indexA = System.Array.IndexOf(_childStates, a);
            var indexB = System.Array.IndexOf(_childStates, b);
            _childStates[indexA] = b;
            _childStates[indexB] = a;
        }

        public void AddChildState(State state)
        {
            System.Array.Resize(ref _childStates, _childStates.Length + 1);
            _childStates[_childStates.Length - 1] = state;
        }

        public void DelChildState(State state)
        {
            var index = System.Array.IndexOf(_childStates, state);
            if (index != -1)
            {
                _childStates[index] = _childStates[_childStates.Length - 1];
                System.Array.Resize(ref _childStates, _childStates.Length - 1);
            }
        }
#endif
    }

    [System.Serializable]
    public class StateBehaviour
    {
        internal State state;

        public void SetState(string name)
        {
            state.SetState(name);
        }

        public float GetStateTime()
        {
            return state.GetActiveTime();
        }

        public virtual void Awake(IInjector injector) { }
        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Update() { }
    }
}