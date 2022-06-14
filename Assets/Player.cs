using Bibyter.CustomEvent;
using SharedObjectNs.BaseVariables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform circleCenter;
    public float radius;
    public float speed;

    public float result;

    public float _currentPosition;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(circleCenter.position, Vector2.Distance(circleCenter.position, transform.position));
    }

    private void Update()
    {
        var position = (Vector2)transform.position;
        var circleCenterPosition = (Vector2)circleCenter.position;


        var rad = Bibyter.Mathematics.Circle.DirectionXYToRad(position - circleCenterPosition);

        var circle = new Bibyter.Mathematics.Circle(Vector2.Distance(position, circleCenterPosition));


        rad = circle.ApplyLinearSpeed(rad, speed * Time.deltaTime);

        var resultPosition = circle.GetPoint(rad) + circleCenterPosition;


        transform.position = resultPosition;
    }
}