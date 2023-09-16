using Bibyter.CustomEvent;
using SharedObjectNs.BaseVariables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bibyter.Mathematics;

namespace Example_Car2
{
    public class Player1 : MonoBehaviour
    {
        [Range(-1f, 1f)]
        public float steerAngle;
        public float speed;


        public float rotation;
        //public Vector2 position;

        private void OnDrawGizmos()
        {
        }


        public static void Move(float steerAngle, float speed, float deltaTime, ref float rotation, ref Vector2 position)
        {
            if (Mathf.Abs(steerAngle) > 0.05f)
            {
                var rad = steerAngle + Mathf.PI * 0.5f;
                var dir = Shape2dExt.RadToDirection(rad);

                Shape2dExt.Intersection_LineVsLine(new Line2d(Vector2.up, Vector2.up + dir), new Line2d(Vector2.zero, Vector2.left), out var point);

                var circleCenter = point;
                var circle = new Circle(point.magnitude);

                var localSpeed = steerAngle < 0f ? -speed : speed;
                var startRad = steerAngle < 0f ? Mathf.PI * 0.5f : -(Mathf.PI * 0.5f);

                var nextRad = circle.ApplyLinearSpeed(startRad, localSpeed * deltaTime);

                var localDeltaPosition = circle.GetPoint(nextRad) + circleCenter;


                rotation += nextRad - startRad;
            }
            else
            {
                position += Shape2dExt.RadToDirection(rotation) * speed * deltaTime;
            }
        }

        private void Update()
        {
            rotation += steerAngle * Time.deltaTime;
            transform.eulerAngles = new Vector3(0f, 0f, rotation * -Mathf.Rad2Deg);

            transform.position += transform.up * speed * Time.deltaTime;
        }
    }
}