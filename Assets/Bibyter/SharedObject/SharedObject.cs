using Bibyter;
using Bibyter.CustomEvent;
using System.Collections.Generic;
using UnityEngine;


[DefaultExecutionOrder(-100)]
public sealed class SharedObject : MonoBehaviour, IInjector
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
    List<System.Object> _cachedRuntimeInternalLinks;

    public T GetInternalLink<T>() where T : class
    {
        if (_cachedRuntimeInternalLinks == null)
            _cachedRuntimeInternalLinks = new List<object>();

        for (int i = 0; i < _cachedRuntimeInternalLinks.Count; i++)
        {
            if (_cachedRuntimeInternalLinks[i] is T)
                return _cachedRuntimeInternalLinks[i] as T;
        }

        if (!typeof(T).IsSubclassOf(typeof(UnityEngine.Object)))
        {
            var instance = System.Activator.CreateInstance<T>();
            _cachedRuntimeInternalLinks.Add(instance);
            return instance;
        }

        throw new System.Exception($"SharedObject.GetInternalLink(NotFindLink {typeof(T).FullName})");
    }

    public void AddInterLink(System.Object obj)
    {
        if (_cachedRuntimeInternalLinks == null)
            _cachedRuntimeInternalLinks = new List<object>();

        _cachedRuntimeInternalLinks.Add(obj);
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

    OrderableEvent<T> GetExternalEvent<T>();
    OrderableEvent<T> GetInternalEvent<T>();
}