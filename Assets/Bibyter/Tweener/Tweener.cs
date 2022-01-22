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

        public static IEnumerator MoveToPoint(Transform transform, Vector3 point, float duration, EaseMethod easeMethod, System.Action onComplete)
        {
            float time = 0f;
            var startPosition = transform.position;

            while(time <= 1f)
            {
                time += Time.deltaTime / duration;
                transform.position = Vector3.Lerp(startPosition, point, EaseFunction(easeMethod, time));
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

        public enum EaseMethod { Linear, InOutQuad }

        public static float EaseFunction(EaseMethod method, float value)
        {
            switch (method)
            {
                case EaseMethod.Linear: return value;
                case EaseMethod.InOutQuad: return InOutQuad(value);
                default: return value;
            }
        }

        public static float InOutQuad(float value)
        {
            value *= 2f;
            if (value < 1f) return 1f / 2f * value * value;
            value--;
            return -1f / 2f * (value * (value - 2f) - 1f);
        }
    }
}