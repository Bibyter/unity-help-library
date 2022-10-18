using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.UiElements.FunctionGraph
{
    [DefaultExecutionOrder(-1)]
    public sealed class UiFunctionGraphLine : MonoBehaviour
    {
        [SerializeField] RectTransform _lineRendererRect;
        [SerializeField] LineRenderer _lineRenderer;

        Queue<float> _queue;

        private void Awake()
        {
            _queue = new Queue<float>(64);
        }

        public void AddValue(float v, int anscissaAxisCapacity)
        {
            if (_queue.Count == anscissaAxisCapacity)
            {
                _queue.Dequeue();
            }

            _queue.Enqueue(v);
        }

        public void ApplyLine(int anscissaAxisCapacity, float ordinateAxisMinY, float ordinateAxisMaxY)
        {
            var rect = _lineRendererRect.rect;
            float height = rect.height;
            float width = rect.width;
            float countf = (float)_queue.Count;
            float fillamount = _queue.Count / ((float)anscissaAxisCapacity);

            _lineRenderer.positionCount = _queue.Count;

            int i = 0;
            foreach (var value in _queue)
            {
                float inorm = i / countf;

                float y = Mathf.InverseLerp(ordinateAxisMinY, ordinateAxisMaxY, value) * height;

                _lineRenderer.SetPosition(i, new Vector3(width * inorm * fillamount, y, 0f));

                i++;
            }
        }

        public void Clear()
        {
            _queue.Clear();
        }
    }
}