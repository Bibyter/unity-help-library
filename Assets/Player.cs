using Bibyter.CustomEvent;
using SharedObjectNs.BaseVariables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Camera _camera;
    Camera _uiCamera;

    TakeDamage _takeDamage;


    OrderableEvent<int> _intEvent;

    void Start()
    {
        _intEvent = new OrderableEvent<int>();

        print(_intEvent.GetType().GetGenericArguments()[0]);

        GetComponent<ILinkRegistragor>().AddInterLink(_takeDamage);
        _camera = StaticInjector.GetLink<Camera>();
        _uiCamera = StaticInjector.GetLink<Camera>("ui-camera");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public sealed class TakeDamage
{
    IntVariable _health;

    TakeDamage _takeDamage;

    Bibyter.CustomEvent.OrderableEvent<int> _event;

    void Awake(IInjector injector)
    {
        _takeDamage = injector.GetInternalLink<TakeDamage>();
        _health = injector.GetInternalLink<IntVariable>("health");
        _event = injector.GetInternalLink<Bibyter.CustomEvent.OrderableEvent<int>>();
    }

    void Handle(int value)
    {
        _health.value -= value;
    }
}

public sealed class TakeDamage2
{
    IntVariable _health;

    TakeDamage _takeDamage;

    void Awake(IInjector injector)
    {
        _takeDamage = injector.GetInternalLink<TakeDamage>();
        _health = injector.GetInternalLink<IntVariable>("health");
    }

    void Handle(int value)
    {
        _health.value -= value;
    }
}
