using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.UiElements
{
    public sealed class ScrollView2Indicator : MonoBehaviour
    {
        [SerializeField] ScrollView2 _scrollView2;
        [SerializeField] Sprite _selectedSprite;
        [SerializeField] Sprite _unselectedSprite;

        int _cacheRangesCount;
        int _cachePosition;

        private void OnEnable()
        {
            _cacheRangesCount = int.MinValue;
            _cachePosition = int.MinValue;
        }

        private void LateUpdate()
        {
            if (_cacheRangesCount != _scrollView2.GetRangesCount())
            {
                _cacheRangesCount = _scrollView2.GetRangesCount();
                SetSlotsCount(_cacheRangesCount);
                _cachePosition = int.MinValue;
            }

            if (_cachePosition != _scrollView2.position)
            {
                _cachePosition = _scrollView2.position;
                SetPosition(_cachePosition);
            }
        }

        private void SetSlotsCount(int value)
        {
            for (int i = transform.childCount; i < value; i++)
            {
                Instantiate(transform.GetChild(0), transform);
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(i < value);
            }
        }

        private void SetPosition(int value)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent(out UnityEngine.UI.Image image))
                {
                    if (i == value)
                    {
                        image.sprite = _selectedSprite;
                    }
                    else
                    {
                        image.sprite = _unselectedSprite;
                    }
                }
            }
        }
    }
}