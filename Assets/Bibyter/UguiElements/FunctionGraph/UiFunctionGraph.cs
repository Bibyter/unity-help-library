using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.UiElements.FunctionGraph
{
    public sealed class UiFunctionGraph : MonoBehaviour
    {
        [SerializeField] Transform _linesParent;
        [SerializeField] AxisNumbers _ordinateAxisNumbers;
        [SerializeField] float _ordinateAxisMaxYDefault = 100f;
        [SerializeField] float _ordinateAxisMinYDefault = 0f;
        [SerializeField] int _anscissaAxisCapacity = 64;

        Queue<float> _queue;

        float _ordinateAxisMaxY;
        float _ordinateAxisMinY;

        private void Awake()
        {
            _queue = new Queue<float>(_anscissaAxisCapacity);
        }

        private void OnEnable()
        {
            ClearAllLines();
            OrdinateAxisReset();
        }

        public void AddValue(int lineId, float v)
        {
            if (v < _ordinateAxisMinY)
                _ordinateAxisMinY = v;

            if (v > _ordinateAxisMaxY)
                _ordinateAxisMaxY = v;

            if (_linesParent.GetChild(lineId).TryGetComponent(out UiFunctionGraphLine line))
            {
                line.AddValue(v, _anscissaAxisCapacity);
                line.ApplyLine(_anscissaAxisCapacity, _ordinateAxisMinY, _ordinateAxisMaxY);
            }

            _ordinateAxisNumbers.Apply(_ordinateAxisMinY, _ordinateAxisMaxY);
        }

        public void AddValue(float v)
        {
            AddValue(0, v);
        }

        public void ClearAllLines()
        {
            for (int i = 0; i < _linesParent.childCount; i++)
            {
                if (_linesParent.GetChild(i).TryGetComponent(out UiFunctionGraphLine line))
                {
                    line.Clear();
                    line.ApplyLine(_anscissaAxisCapacity, _ordinateAxisMinY, _ordinateAxisMaxY);
                }
            }
        }

        public void OrdinateAxisReset()
        {
            _ordinateAxisMinY = _ordinateAxisMinYDefault;
            _ordinateAxisMaxY = _ordinateAxisMaxYDefault;

            for (int i = 0; i < _linesParent.childCount; i++)
            {
                if (_linesParent.GetChild(i).TryGetComponent(out UiFunctionGraphLine line))
                {
                    line.ApplyLine(_anscissaAxisCapacity, _ordinateAxisMinY, _ordinateAxisMaxY);
                }
            }

            _ordinateAxisNumbers.Apply(_ordinateAxisMinY, _ordinateAxisMaxY);
        }

        [System.Serializable]
        sealed class AxisNumbers
        {
            [SerializeField] TMPro.TextMeshProUGUI[] _labels;

            float _cacheMinY = -1f;
            float _cacheMaxY = -1f;

            public void Apply(float minY, float maxY)
            {
                if (_cacheMinY == minY && _cacheMaxY == maxY)
                    return;

                float lengthf = _labels.Length;

                for (int i = 0; i < _labels.Length; i++)
                {
                    float inorm = i / (lengthf - 1f);

                    _labels[i].text = Mathf.Lerp(minY, maxY, inorm).ToString("0");
                }

                _cacheMinY = minY;
                _cacheMaxY = maxY;
            }
        }
    }
}