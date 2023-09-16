using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lite fsm is designed to simplify the implementation of fsm in code using enum
/// </summary>
public sealed class LiteFsm<T> where T : System.Enum
{
    Dictionary<T, State> _states;

    State _currentState;
    public State currentState => _currentState;

    T _currentStateKey;
    public T currentStateKey => _currentStateKey;

    public float stateTime => _currentState.time;

    bool _isActive;


    public LiteFsm()
    {
        _states = new Dictionary<T, State>();
        _isActive = false;
    }

    /// <summary>
    /// Add a new state
    /// </summary>
    public LiteFsm<T> Add(T stateKey, State state)
    {
#if DEBUG
        if (state == null)
        {
            throw new System.NullReferenceException("State should not null");
        }

        if (_states.ContainsKey(stateKey))
        {
            throw new System.Exception($"Add Duplicate State {stateKey}");
        }
#endif

        _states.Add(stateKey, state);
        state.InitInternal(stateKey, this);

        return this;
    }

    /// <summary>
    /// Add a new state with method delegation
    /// </summary>
    public LiteFsm<T> AddActions(T stateKey, System.Action enter = null, System.Action update = null, System.Action exit = null)
    {
        Add(stateKey, new StateAction(enter, update, exit));
        return this;
    }

    /// <summary>
    /// If you need to add an fsm to another fsm, you can use this method
    /// </summary>
    public Adapter<CastType> AsState<CastType>(T entryState) where CastType : System.Enum
    {
        return new Adapter<CastType>(entryState, this);
    }

    /// <summary>
    /// If you need to add an fsm to another fsm, you can use this method
    /// </summary>
    public Adapter<CastType> AsState<CastType>(System.Func<T> getEntryState) where CastType : System.Enum
    {
        return new Adapter<CastType>(getEntryState, this);
    }

    public void Entry(T stateKey)
    {
        if (!_isActive)
        {
            if (HasStateValidate(stateKey))
            {
                _currentStateKey = stateKey;
                _currentState = _states[stateKey];

                _isActive = true;

                _currentState.EnterInternal();
            }
        }
    }

    public void Exit()
    {
        if (_isActive)
        {
            _currentState.ExitInternal();
            _currentState = null;

            _isActive = false;
        }
    }

    public void SetState(T stateKey)
    {
        if (_isActive)
        {
            if (HasStateValidate(stateKey))
            {
                _currentState.ExitInternal();

                var newState = _states[stateKey];

                newState.EnterInternal();

                _currentStateKey = stateKey;
                _currentState = newState;
            }
            else
            {
                // если не нашли состояние, то выключаем fsm
                _currentState.ExitInternal();
                _isActive = false;
            }
        }
    }

    public void Update(float deltaTime)
    {
        if (_isActive)
        {
            _currentState.UpdateInternal(deltaTime);
        }
    }

    public bool TryGetState(T stateKey, out State foundState)
    {
        return _states.TryGetValue(stateKey, out foundState);
    }

    public State GetState(T stateKey)
    {
        return _states[stateKey];
    }

    private bool HasStateValidate(T stateKey)
    {
        if (_states.ContainsKey(stateKey))
        {
            return true;
        }
        else
        {
            Debug.LogError($"Not found state {stateKey}");
            return false;
        }
    }

    /// <summary>
    /// Базовый класс состояния
    /// </summary>
    public abstract class State
    {
        /// <summary>
        /// Идентификатор состояния
        /// </summary>
        public T key { private set; get; }

        /// <summary>
        /// Время, с момента включения состояния
        /// </summary>
        public float time { private set; get; }

        public LiteFsm<T> attachedFsm { private set; get; }


        internal void InitInternal(T key, LiteFsm<T> attachedFsm)
        {
            this.attachedFsm = attachedFsm;
            this.key = key;
        }

        internal void EnterInternal()
        {
            time = 0f;

            Enter();
        }

        internal void UpdateInternal(float deltaTime)
        {
            time += deltaTime;

            Update(deltaTime);
        }

        internal void ExitInternal()
        {
            Exit();
        }

        protected void SetState(T state)
        {
            attachedFsm.SetState(state);
        }

        protected virtual void Enter() { }
        protected virtual void Update(float deltaTime) { }
        protected virtual void Exit() { }
    }

    /// <summary>
    /// Адаптер для ипользования делегатов
    /// </summary>
    public sealed class StateAction : State
    {
        System.Action _enter, _update, _exit;

        public StateAction()
        { }

        public StateAction(System.Action enter, System.Action update, System.Action exit)
        {
            _enter = enter;
            _update = update;
            _exit = exit;
        }

        protected override void Enter()
        {
            _enter?.Invoke();
        }

        protected override void Update(float deltaTime)
        {
            _update?.Invoke();
        }

        protected override void Exit()
        {
            _exit?.Invoke();
        }
    }

    /// <summary>
    /// Адаптер для вложения одного fsm в другой fsm с другим enum типом
    /// </summary>
    /// <typeparam name="CastType"></typeparam>
    public sealed class Adapter<CastType> : LiteFsm<CastType>.State where CastType : System.Enum
    {
        T _entryState;
        System.Func<T> _getEntryState;
        LiteFsm<T> _fsm;

        public Adapter(T entryState, LiteFsm<T> fsm)
        {
            _entryState = entryState;
            _getEntryState = null;
            _fsm = fsm;
        }

        public Adapter(System.Func<T> getEntryState, LiteFsm<T> fsm)
        {
            _entryState = default;
            _getEntryState = getEntryState;
            _fsm = fsm;
        }

        protected override void Enter()
        {
            var entryState = _entryState;

            if (_getEntryState != null)
            {
                entryState = _getEntryState.Invoke();
            }

            _fsm.Entry(entryState);
        }

        protected override void Update(float deltaTime)
        {
            _fsm.Update(deltaTime);
        }

        protected override void Exit()
        {
            _fsm.Exit();
        }
    }


}




