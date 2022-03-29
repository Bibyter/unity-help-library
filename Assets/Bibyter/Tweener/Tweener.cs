using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter
{
    public static class Tweener
    {
        public static IEnumerator ScaleTo(Transform transform, Vector3 scale, float duration, System.Action onComplete)
        {
            float time = 0f;
            var from = transform.localScale;

            while (time <= 1f)
            {
                time += Time.deltaTime / duration;
                transform.localScale = Vector3.Lerp(from, scale, InOutQuad(time));
                yield return null;
            }

            onComplete?.Invoke();
        }

        public static IEnumerator RotateTo(Transform transform, Quaternion targetRotation, float duration, System.Action onComplete)
        {
            float time = 0f;
            var startRotation = transform.rotation;

            while (time <= 1f)
            {
                time += Time.deltaTime / duration;
                transform.rotation = Quaternion.Lerp(startRotation, targetRotation, InOutQuad(time));
                yield return null;
            }

            onComplete?.Invoke();
        }

        public static IEnumerator JumpToPointTween(Transform transform, Vector3 target, float height, float duration, System.Action onComplete)
        {
            float time = 0f;
            var startPosition = transform.position;

            while (time <= 1f)
            {
                time += Time.deltaTime / duration;
                transform.position = Vector3.Lerp(startPosition, target, InOutQuad(time)) + new Vector3(0f, Mathf.Sin(time * Mathf.PI) * height, 0f);
                yield return null;
            }

            onComplete?.Invoke();
        }

        public static IEnumerator JumpToTransformTween(Transform transform, Transform target, Vector3 targetOffset, float height, float duration, System.Action onComplete)
        {
            float time = 0f;
            var startPosition = transform.position;

            while (time <= 1f)
            {
                time += Time.deltaTime / duration;
                transform.position = Vector3.Lerp(startPosition, target.TransformPoint(targetOffset), InOutQuad(time)) + new Vector3(0f, Mathf.Sin(time * Mathf.PI) * height, 0f);
                yield return null;
            }

            onComplete?.Invoke();
        }

        public static IEnumerator MoveToTransformTween(Transform transform, Transform target, Vector3 targetOffset, float duration, System.Action onComplete)
        {
            float time = 0f;
            var startPosition = transform.position;

            while (time <= 1f)
            {
                time += Time.deltaTime / duration;
                transform.position = Vector3.Lerp(startPosition, target.TransformPoint(targetOffset), InOutQuad(time));
                yield return null;
            }

            onComplete?.Invoke();
        }

        #region IPosition
        public interface IPosition
        {
            Vector3 Get();
            void Set(in Vector3 value);
        }

        public sealed class TransformPosition : IPosition
        {
            Transform _transform;

            public TransformPosition(Transform transform)
            {
                _transform = transform;
            }

            Vector3 IPosition.Get()
            {
                return _transform.position;
            }

            void IPosition.Set(in Vector3 value)
            {
                _transform.position = value;
            }
        }

        public sealed class TransformLocalPosition : IPosition
        {
            Transform _transform;

            public TransformLocalPosition(Transform transform)
            {
                _transform = transform;
            }

            Vector3 IPosition.Get()
            {
                return _transform.localPosition;
            }

            void IPosition.Set(in Vector3 value)
            {
                _transform.localPosition = value;
            }
        }
        #endregion

        public static IEnumerator MoveToPoint(IPosition position, Vector3 point, float duration, EaseMethod easeMethod, System.Action onComplete)
        {
            float time = 0f;
            var startPosition = position.Get();

            while(time <= 1f)
            {
                time += Time.deltaTime / duration;
                position.Set(Vector3.Lerp(startPosition, point, EaseFunction(easeMethod, time)));
                yield return null;
            }

            onComplete?.Invoke();
        }

        public static IEnumerator MoveWithPath(Transform transform, List<Vector3> path, float speed, System.Action onComplete)
        {
            var position = transform.position;
            int pointId = 0;

            while(pointId < path.Count)
            {
                position = Vector3.MoveTowards(position, path[pointId], Time.deltaTime * speed);
                transform.position = position;


                if (Vector3.Distance(position, path[pointId]) < 0.05f)
                {
                    pointId++;
                }

                yield return null;
            }

            onComplete?.Invoke();
        }

        public static IEnumerator Bounce(Transform transform, float toScale, float bounceForce, float duration, EaseMethod easeMethod, System.Action onComplete)
        {
            float time = 0f;
            Vector3 startScale = transform.localScale;
            Vector3 targetScale = Vector3.one * toScale;

            while (time <= 1f)
            {
                time += Time.deltaTime / duration;
                float easeTime = EaseFunction(easeMethod, time);
                transform.localScale = Vector3.Lerp(startScale, targetScale, easeTime) + (Mathf.Sin(easeTime * Mathf.PI) * bounceForce * Vector3.one);
                yield return null;
            }

            onComplete?.Invoke();
        }

        public static IEnumerator GravityTween(Transform transform, Vector3 targetPoint, Vector3 gravity, float duration, System.Action onComplete)
        {
            float time = 0f;
            Vector3 startPosition = transform.position;

            while(time <= 1f)
            {
                time += Time.deltaTime / duration;
                transform.position = LerpTrajectory(startPosition, targetPoint, gravity, duration, time);
                yield return null;
            }

            onComplete?.Invoke();
        }

        public static Vector3 LerpTrajectory(in Vector3 from, in Vector3 to, in Vector3 gravity, float duration, float t)
        {
            return from + ((to - from - (gravity * duration * duration * 0.5f)) / duration) * t + (gravity * t * t * 0.5f);
        }

        public enum EaseMethod { Linear, InOutQuad, OutQuad, InQuad }

        public static float EaseFunction(EaseMethod method, float value)
        {
            switch (method)
            {
                case EaseMethod.Linear: return value;
                case EaseMethod.InOutQuad: return InOutQuad(value);
                case EaseMethod.InQuad: return InQuad(value);
                case EaseMethod.OutQuad: return OutQuad(value);
                default: return value;
            }
        }

        private static float InQuad(float time)
        {
            return time * time;
        }

        private static float OutQuad(float time)
        {
            return 1f - (1f - time) * (1f - time);
        }

        private static float InOutQuad(float time)
        {
            return time < 0.5f ? 2f * time * time : 1f - Mathf.Pow(-2f * time + 2f, 2f) / 2f;
        }
    }
}