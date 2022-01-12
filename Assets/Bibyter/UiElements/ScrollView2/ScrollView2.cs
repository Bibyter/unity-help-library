using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.UiElements
{
    public sealed class ScrollView2 : MonoBehaviour
    {
        [SerializeField] RectTransform _content;
        [SerializeField] float _tweenDuration = 0.6f;

        RectTransform _rectTransform;

        Coroutine _tweenCoroutine;

        public int position { private set; get; }
        int _goalPosition;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void OnEnable()
        {
            position = 0;
            _goalPosition = 0;
        }

        private void OnValidate()
        {
            if (_content != null)
            {
                _content.pivot = new Vector2(0f, 1f);
            }
        }

        private void SetPosition(int value)
        {
            var size = _rectTransform.sizeDelta;
            var temp = Mathf.Clamp(value, -Mathf.FloorToInt(_content.sizeDelta.x / _rectTransform.sizeDelta.x), 0);

            if (temp != _goalPosition)
            {
                _goalPosition = temp;

                if (_tweenCoroutine != null) StopCoroutine(_tweenCoroutine);
                _tweenCoroutine = StartCoroutine(Tween(_content, new Vector2(_goalPosition * size.x, 0f), _tweenDuration, OnTweenComplete));
            }
        }

        private void OnTweenComplete()
        {
            position = -_goalPosition;
        }

        public int GetRangesCount()
        {
            return Mathf.CeilToInt(_content.sizeDelta.x / _rectTransform.sizeDelta.x);
        }

        [ContextMenu(nameof(SwipeLeft))]
        public void SwipeLeft()
        {
            SetPosition(_goalPosition + 1);
        }

        [ContextMenu(nameof(SwipeRight))]
        public void SwipeRight()
        {
            SetPosition(_goalPosition - 1);
        }

        static IEnumerator Tween(RectTransform transform, Vector2 target, float duration, System.Action onComplete)
        {
            float time = 0f;
            Vector2 startPosition = transform.anchoredPosition;

            while (time <= 1f)
            {
                time += Time.deltaTime / duration;
                transform.anchoredPosition = Vector2.Lerp(startPosition, target, InOutQuad(time));
                yield return null;
            }

            onComplete?.Invoke();
        }

        static float InOutQuad(float value)
        {
            value *= 2f;
            if (value < 1f) return 1f / 2f * value * value;
            value--;
            return -1f / 2f * (value * (value - 2f) - 1f);
        }
    }
}