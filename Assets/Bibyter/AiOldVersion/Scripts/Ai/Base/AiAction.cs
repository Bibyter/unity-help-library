using System.Collections.Generic;

namespace Client.Ai
{
    public enum AiActionState { None, Run, Failed, Complete }

    public sealed class AiController
    {
        List<AiAction> _cachedActions;
        AiContainer _container;
        AiAction _currentAction;
        AiAction _nextAction;
        bool _needUpdateInTransition;

        public AiController(AiContainer container, bool needUpdateInTransition)
        {
            _cachedActions = new List<AiAction>(4);
            _container = container;
            _needUpdateInTransition = needUpdateInTransition;
        }

        public void SetState<T>() where T : AiAction, new()
        {
            _nextAction = GetAction<T>();
        }

        public void Update()
        {
            if (_nextAction != null)
            {
                _currentAction?.OnExit();
                _nextAction.OnEnter();
                _currentAction = _nextAction;
                _nextAction = null;

                if (_needUpdateInTransition)
                    _currentAction.Update();
            }
            else
            {
                _currentAction.Update();
            }
        }

        AiAction GetAction<T>() where T : AiAction, new()
        {
            for (int i = 0; i < _cachedActions.Count; i++)
            {
                if (_cachedActions[i] is T)
                {
                    return _cachedActions[i] as T;
                }
            }

            var newAction = new T();
            newAction.Start(_container);

            _cachedActions.Add(newAction);

            return newAction;
        }
    }

    public sealed class AiContainer
    {
        List<object> _list;

        public AiContainer()
        {
            _list = new List<object>(4);
        }

        public void Add(object item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public T Get<T>() where T : class
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i] is T)
                    return _list[i] as T;
            }

            throw new System.Exception($"Not has component {typeof(T).Name} in AiContainer");
        }
    }

    public class AiAction
    {
        public virtual void Start(AiContainer container)
        { }

        public virtual void OnEnter()
        { }

        public virtual void Update()
        { }

        public virtual void OnExit()
        { }
    }
}
