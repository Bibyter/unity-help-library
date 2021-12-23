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
        [SerializeField] State _parentState;

        State _currentChildState;
        float _enterTime;
        string _deferredSetState;
        int _enterBehavioursCount;

        public void Awake2(IInjector injector)
        {
            _deferredSetState = null;

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
            _enterBehavioursCount = 0;

            for (int i = 0; i < _behaviours.Length; i++)
            {
                _behaviours[i].Enter();
                _enterBehavioursCount++;

                if (_deferredSetState != null)
                {
                    _parentState.SetState(_deferredSetState);
                    _deferredSetState = null;
                    return;
                }
            }

            _enterTime = Time.time;

            // child state entry
            SetChildState(0);
        }

        public void Exit()
        {
            for (int i = 0; i < _enterBehavioursCount; i++)
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

                if (_deferredSetState != null)
                {
                    _parentState.SetState(_deferredSetState);
                    _deferredSetState = null;
                    return;
                }
            }

            if (_currentChildState != null)
            {
                _currentChildState.Update();
            }
        }

        public void SetState(string name)
        {
            SetChildStateInstantly(name);
        }

        public void SetStateOfBehaviour(string name)
        {
            _deferredSetState = name;
        }

        void SetChildStateInstantly(string name)
        {
            for (int i = 0; i < _childStates.Length; i++)
            {
                if (_childStates[i].name == name)
                {
                    SetChildState(i);
                    return;
                }
            }

            Debug.LogWarning($"Not Find Child State {name} of {this.name} State");
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

                _deferredSetState = null;
                _currentChildState.Enter();
            }
        }

        public State Clone()
        {
            return Clone(null);
        }

        State Clone(State parent)
        {
            var instance = Instantiate(this);
            instance.name = this.name;
            instance._parentState = parent;
            instance.ChildStatesClone();
            return instance;
        }

        void ChildStatesClone()
        {
            for (int i = 0; i < _childStates.Length; i++)
            {
                _childStates[i] = _childStates[i].Clone(this);
            }
        }

        public void InvokeEvent<T>(ref T data)
        {
            _deferredSetState = null;

            for (int i = 0; i < _behaviours.Length; i++)
            {
                if (_behaviours[i] is IEventHandler<T> handler)
                {
                    handler.OnEvent(data);

                    if (_deferredSetState != null)
                    {
                        //SetStateInstantly(_deferredSetStateName);
                        _deferredSetState = null;
                        return;
                    }

                }
            }

            if (_currentChildState != null)
            {
                _currentChildState.InvokeEvent(ref data);
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

        public State parentState
        {
            set => _parentState = value;
            get => _parentState;
        }
#endif
    }

    [System.Serializable]
    public class StateBehaviour
    {
        internal State state;

        public void SetState(string name)
        {
            state.SetStateOfBehaviour(name);
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

    public interface IEventHandler<T>
    {
        void OnEvent(T data);
    }
}