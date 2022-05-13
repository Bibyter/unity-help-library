using Bibyter.DependencyInjection;
using Bibyter.CustomEvent;
using System.Collections.Generic;
using UnityEngine;


[DefaultExecutionOrder(-100)]
public sealed class InjectionNode : MonoBehaviour, IInjector, ILinkRegistragor
{
    #region
    public T GetExternalLink<T>() where T : class
    {
        return SceneLinkRegistrator.GetLink<T>();
    }

    public T GetExternalLink<T>(string name) where T : class
    {
        return SceneLinkRegistrator.GetNamedLink<T>(name);
    }
    #endregion

    #region
    [System.Serializable]
    public struct ValueVar
    {
        public string name;
        [SerializeReference] public System.Object var;
    }

    [System.Serializable]
    public struct ReferenceVar
    {
        public string name;
        [SerializeReference] public UnityEngine.Object var;
    }

    [SerializeField] ValueVar[] _valueVars;
    [SerializeField] ReferenceVar[] _referenceVars;
    List<System.Object> _optionalLinks;

    public T GetInternalLink<T>() where T : class
    {
        return GetInternalLink<T>(null);
    }

    public T GetInternalLink<T>(string name) where T : class
    {
        bool hasName = !string.IsNullOrEmpty(name);

        if (typeof(T).IsSubclassOf(typeof(UnityEngine.Object)))
        {
            for (int i = 0; i < _referenceVars.Length; i++)
            {
                if (_referenceVars[i].var is T && (!hasName || _referenceVars[i].name == name))
                    return _referenceVars[i].var as T;
            }
        }

        for (int i = 0; i < _valueVars.Length; i++)
        {
            if (_valueVars[i].var is T && (!hasName || _valueVars[i].name == name))
                return _valueVars[i].var as T;
        }

        if (_optionalLinks != null)
        {
            for (int i = 0; i < _optionalLinks.Count; i++)
            {
                if (_optionalLinks[i] is T)
                    return _optionalLinks[i] as T;
            }
        }

        throw new System.Exception($"{nameof(InjectionNode)}.GetInternalLink(NotFoundLink, Type={typeof(T).Name}, Name={name})");
    }

    /// <summary>
    /// Нужен, что-бы регастрировать чистые классы с поведением.
    /// Альнернативой может быть только какой-то контейнер для поведений, либо делать их как MonoBehaviour
    /// </summary>
    /// <param name="obj"></param>
    public void AddInterLink(System.Object obj)
    {
        if (_optionalLinks == null)
            _optionalLinks = new List<object>();

        _optionalLinks.Add(obj);
    }
    #endregion

    #region Events
    List<object> _cachedEvents;

    public OrderableEvent<T> GetInternalEvent<T>()
    {
        if (_cachedEvents == null)
            _cachedEvents = new List<object>();

        for (int i = 0; i < _cachedEvents.Count; i++)
        {
            if (_cachedEvents[i] is Bibyter.CustomEvent.OrderableEvent<T>)
            {
                return _cachedEvents[i] as Bibyter.CustomEvent.OrderableEvent<T>;
            }
        }

        var newEvent = new Bibyter.CustomEvent.OrderableEvent<T>();
        _cachedEvents.Add(newEvent);
        return newEvent;
    }
    #endregion

    #region ExternalEvents
    static List<object> _cachedExternalEvents = new List<object>(12);

    public OrderableEvent<T> GetExternalEvent<T>()
    {
        for (int i = 0; i < _cachedExternalEvents.Count; i++)
        {
            if (_cachedExternalEvents[i] is OrderableEvent<T>)
            {
                return _cachedExternalEvents[i] as OrderableEvent<T>;
            }
        }

        var newEvent = new OrderableEvent<T>();
        _cachedExternalEvents.Add(newEvent);
        return newEvent;
    }
    #endregion
}

public interface IInjector
{
    T GetExternalLink<T>() where T : class;
    T GetExternalLink<T>(string name) where T : class;
    T GetInternalLink<T>() where T : class;
    T GetInternalLink<T>(string name) where T : class;

    OrderableEvent<T> GetExternalEvent<T>();
    OrderableEvent<T> GetInternalEvent<T>();
}

public interface ILinkRegistragor
{
    void AddInterLink(System.Object obj);
}