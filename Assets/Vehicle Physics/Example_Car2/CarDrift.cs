using Bibyter.CustomEvent;
using SharedObjectNs.BaseVariables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bibyter.Mathematics;

namespace Example_Car2
{
    public class CarDrift : MonoBehaviour
    {
        [Range(-0.9f, 0.9f)]
        public float steerAngle;
        public float speed;

        public float kat1Length = 2f;
        public float kat1Angle = 0f;

        public float rotation;
        public Vector2 position;

        public WheelView wheelLeft, wheelRight;
        public WheelView wheelBackLeft, wheelBackRight;

        public Wheel wheelLeft1;
        public Wheel wheelRight1;

        [System.Serializable]
        public struct Wheel
        {
        }

        private void OnDrawGizmos()
        {
            //Gizmos.DrawRay(Vector3.zero, Shape2dExt.RadToDirection(kat1Angle) * kat1Length);
            Gizmos.DrawRay(Vector3.zero, Vector2.down * kat1Length);
            Gizmos.DrawRay(Vector2.down * kat1Length, Vector2.left * Mathf.Tan(kat1Angle) * kat1Length);


            return;

            if (Mathf.Abs(steerAngle) > 0.05f)
            {
                var rad = steerAngle + Mathf.PI * 0.5f;
                var dir = Shape2dExt.RadToDirection(rad);

                Shape2dExt.Intersection_LineVsLine(new Line2d(Vector2.up, Vector2.up + dir), new Line2d(Vector2.zero, Vector2.left), out var point);

                var circleCenter = point;
                var circle = new Circle(point.magnitude);

                Gizmos.DrawRay(Vector2.up - dir * 50f, dir * 100f);
                Gizmos.DrawRay(Vector2.zero - Vector2.left * 50f, Vector2.left * 100f);

                Gizmos.DrawWireSphere(circleCenter, circle.radius);

                Gizmos.color = Color.green;

                var localSpeed = steerAngle < 0f ? -speed : speed;
                var startRad = steerAngle < 0f ? 1.57f : -1.57f;

                var nextRad = circle.ApplyLinearSpeed(startRad, localSpeed);
                var nextPoint = circle.GetPoint(nextRad) + circleCenter;

                Gizmos.DrawWireSphere(nextPoint, 0.3f);
            }
            else
            {
                Gizmos.DrawRay(Vector2.zero, Vector2.up * 100f);

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(Vector2.up * speed, 0.3f);
            }

        }

        private void OnEnable()
        {
            float k1 = 2f;
            float anglek1 = 1.57f;

            float k2 = Mathf.Tan(anglek1) * k1;

            print(k2);
        }

        public static void Move(float steerAngle, float speed, float deltaTime, ref float rotation, ref Vector2 position)
        {
            if (Mathf.Abs(steerAngle) > 0.05f)
            {
                var rad = steerAngle + Mathf.PI * 0.5f;

                var wheelRight = Shape2dExt.RadToDirection(rad + rotation);
                var forward = Shape2dExt.RadToDirection(rotation);
                var right = new Vector2(forward.y, -forward.x);


                //Debug.DrawRay(position, forward * 5f, Color.red);

                float katVertical = 0.85f; // от переднего моста до заднего
                float katHorizontal = Mathf.Tan(rad) * katVertical; // радиус поворота

                var steerLocalPoint = new Vector2(katHorizontal, katVertical + speed);

                var velocity = new Vector2(-steerLocalPoint.y, steerLocalPoint.x);
                velocity = velocity.normalized * speed * Mathf.Sign(-steerAngle);


                rotation += Circle.ConvertLinearToAngularSpeed(speed * deltaTime * Mathf.Sign(-steerAngle), steerLocalPoint.magnitude);

                var worldVelocity = Shape2dExt.RotatePoint(velocity, -rotation);

                position += worldVelocity * deltaTime;

                Debug.DrawRay(position, worldVelocity, Color.blue);

                Debug.DrawRay(position, forward * steerLocalPoint.y, Color.green);
                Debug.DrawRay(position + forward * steerLocalPoint.y, right * steerLocalPoint.x, Color.green);



                Debug.DrawRay(position + forward * katVertical, 20f * wheelRight, Color.red);
                Debug.DrawRay(position + forward * katVertical, -20f * wheelRight, Color.red);
            }
            else
            {
                position += Shape2dExt.RadToDirection(rotation) * speed * deltaTime;
            }
        }

        private void FixedUpdate()
        {
            steerAngle = Mathf.Clamp(steerAngle + Input.GetAxis("Horizontal") * Time.deltaTime * 2f, -0.9f, 0.9f);
            speed += Input.GetAxis("Vertical") * Time.deltaTime;

            Move(steerAngle, speed, Time.deltaTime, ref rotation, ref position);

            transform.position = position;
            transform.eulerAngles = new Vector3(0f, 0f, rotation * -Mathf.Rad2Deg);

            WheelRotation();
        }

        void WheelRotation()
        {
            if (Mathf.Abs(steerAngle) > 0.03f)
            {
                var rad = steerAngle + Mathf.PI * 0.5f;

                float katVertical = 0.85f; // от переднего моста до заднего
                float katHorizontal = Mathf.Tan(rad) * katVertical; // радиус поворота

                float wheelLeft_KatHorizontal = katHorizontal + 0.4f; // 0.4 позиция колеса по x
                float wheelRight_KatHorizontal = katHorizontal - 0.4f;

                float wheelLeft_Hypotenuse = Mathf.Sqrt(wheelLeft_KatHorizontal * wheelLeft_KatHorizontal + katVertical * katVertical);
                float wheelRight_Hypotenuse = Mathf.Sqrt(wheelRight_KatHorizontal * wheelRight_KatHorizontal + katVertical * katVertical);

                float wheelLeft_LinearSpeed = wheelLeft_Hypotenuse / Mathf.Abs(katHorizontal) * speed;
                float wheelRight_LinearSpeed = wheelRight_Hypotenuse / Mathf.Abs(katHorizontal) * speed;

                wheelLeft.UpdateRotationZ(wheelLeft_LinearSpeed * Time.deltaTime);
                wheelRight.UpdateRotationZ(wheelRight_LinearSpeed * Time.deltaTime);

                var wheelLeftRotation = Mathf.Atan(wheelLeft_KatHorizontal / katVertical);
                var wheelRightRotation = Mathf.Atan(wheelRight_KatHorizontal / katVertical);


                if (steerAngle > 0f)
                {
                    wheelLeftRotation += Mathf.PI;
                    wheelRightRotation += Mathf.PI;
                }
                else
                {
                    //wheelLeftRotation -= Mathf.PI;
                    //wheelRightRotation -= Mathf.PI;
                }

                var wheelLeftAxis = Shape2dExt.RadToDirection(wheelLeftRotation);
                var wheelRightAxis = Shape2dExt.RadToDirection(wheelRightRotation);

                DrawRay(new Vector3(-0.4f, 0.85f), wheelLeftAxis * 10f);
                DrawRay(new Vector3(0.4f, 0.85f), wheelRightAxis * 10f);

                wheelLeft.transform.localRotation = Quaternion.Euler(0f, 0f, (wheelLeftRotation - Mathf.PI * 0.5f) * -Mathf.Rad2Deg);
                wheelRight.transform.localRotation = Quaternion.Euler(0f, 0f, (wheelRightRotation - Mathf.PI * 0.5f) * -Mathf.Rad2Deg);

                // back

                float wheelBackLeft_LinearSpeed = Mathf.Abs(wheelLeft_KatHorizontal) / Mathf.Abs(katHorizontal) * speed;
                float wheelBackRight_LinearSpeed = Mathf.Abs(wheelRight_KatHorizontal) / Mathf.Abs(katHorizontal) * speed;
            }
            else
            {
                wheelLeft.transform.localEulerAngles = wheelRight.transform.localEulerAngles = Vector3.zero;

                wheelLeft.UpdateRotationZ(speed * Time.deltaTime);
                wheelRight.UpdateRotationZ(speed * Time.deltaTime);

            }

            wheelBackLeft.UpdateRotationZ(speed * Time.deltaTime * 5f);
            wheelBackRight.UpdateRotationZ(speed * Time.deltaTime * 5f);
        }

        void DrawRay(Vector2 origin, Vector2 dir)
        {
            Debug.DrawRay(Shape2dExt.RotatePoint(origin, -rotation) + position, Shape2dExt.RotatePoint(dir, -rotation));
        }
    }
}