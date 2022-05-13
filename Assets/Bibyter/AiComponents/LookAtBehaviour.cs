using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public sealed class LookAtBehaviour : Bibyter.Behaviour
{
    Vector3 _point;
    Transform _target;
    enum TargetType { Point, Transform }
    TargetType _targetType;

    [SerializeField] float _speed = 1f;

    Transform _transform;
    float _rotation;
    float _goalRotation;

    public override void Awake(IInjector injector)
    {
        _transform = injector.GetInternalLink<Transform>();
    }

    protected override void Enter()
    {
        _rotation = _transform.eulerAngles.y;
    }

    protected override void Update()
    {
        var point = _point;

        switch (_targetType)
        {
            case TargetType.Transform:
                if (_target != null)
                {
                    point = _target.position;
                }
                break;
        }

        var dir = point - _transform.position; dir.y = 0f;
        _goalRotation = Quaternion.LookRotation(dir, Vector3.up).eulerAngles.y;

        _rotation = Mathf.MoveTowardsAngle(_rotation, _goalRotation, Time.deltaTime * _speed);
        _transform.eulerAngles = new Vector3(0f, _rotation, 0f);
    }

    public void SetPoint(Vector3 point)
    {
        _targetType = TargetType.Point;
        _point = point;
    }

    public void SetPoint(Transform transform)
    {
        _targetType = TargetType.Transform;
        _target = transform;
    }
}
