using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Mathematics
{
    public struct Circle
    {
        public float radius;

        public Circle(float radius)
        {
            this.radius = radius;
        }

        /// <summary>
        /// Применить угловую скорость
        /// </summary>
        public float ApplyAngularSpeed(float currentRad, float angularSpeed)
        {
            return currentRad + angularSpeed;
        }

        /// <summary>
        /// Применить линейную скорость
        /// </summary>
        public float ApplyLinearSpeed(float currentRad, float linearSpeed)
        {
            return currentRad + (linearSpeed / radius);
        }

        public float ConvertLinearToAngularSpeed(float linearSpeed)
        {
            return linearSpeed / radius;
        }

        public static float ConvertLinearToAngularSpeed(float linearSpeed, float radius)
        {
            return linearSpeed / radius;
        }

        public float ConvertAngulaToLinearSpeed(float angularSpeed)
        {
            return angularSpeed * radius;
        }

        /// <summary>
        /// Получить точку на окружности
        /// </summary>
        public Vector2 GetPoint(float rad)
        {
            return new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)) * radius;
        }

        /// <summary>
        /// Получить нормализованное направление
        /// </summary>
        public static Vector2 GetDirection(float rad)
        {
            return new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
        }

        public static float DirectionXYToRad(Vector2 direction)
        {
            return Mathf.Atan2(direction.x, direction.y);
        }
    }
}
