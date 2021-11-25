using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter
{
    public static class Tweener
    {
        public static IEnumerator RotateTo(Transform transform, Quaternion targetRotation, float duration, System.Action onComplete)
        {
            float time = 0f;
            var curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
            var startRotation = transform.rotation;

            while (time <= 1f)
            {
                time += Time.deltaTime / duration;

                transform.rotation = Quaternion.Lerp(startRotation, targetRotation, curve.Evaluate(time));

                yield return null;
            }

            onComplete?.Invoke();
        }

        public static IEnumerator JumpToPointTween(Transform transform, Vector3 target, float height, float duration, System.Action onComplete)
        {
            float time = 0f;
            var curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
            var startPosition = transform.position;

            while (time <= 1f)
            {
                time += Time.deltaTime / duration;

                transform.position = Vector3.Lerp(startPosition, target, curve.Evaluate(time)) + new Vector3(0f, Mathf.Sin(time * Mathf.PI) * height, 0f);

                yield return null;
            }

            onComplete?.Invoke();
        }

        public static IEnumerator JumpToTransformTween(Transform transform, Transform target, Vector3 targetOffset, float height, float duration, System.Action onComplete)
        {
            float time = 0f;
            var curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
            var startPosition = transform.position;

            while (time <= 1f)
            {
                time += Time.deltaTime / duration;

                transform.position = Vector3.Lerp(startPosition, target.TransformPoint(targetOffset), curve.Evaluate(time)) + new Vector3(0f, Mathf.Sin(time * Mathf.PI) * height, 0f);

                yield return null;
            }

            onComplete?.Invoke();
        }

        public static IEnumerator MoveToTransformTween(Transform transform, Transform target, Vector3 targetOffset, float duration, System.Action onComplete)
        {
            float time = 0f;
            var curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
            var startPosition = transform.position;

            while (time <= 1f)
            {
                time += Time.deltaTime / duration;

                transform.position = Vector3.Lerp(startPosition, target.TransformPoint(targetOffset), curve.Evaluate(time));

                yield return null;
            }

            onComplete?.Invoke();
        }
    }
}