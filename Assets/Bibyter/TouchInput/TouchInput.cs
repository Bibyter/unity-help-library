using UnityEngine;
using UnityEngine.EventSystems;

namespace Bibyter
{
    [DefaultExecutionOrder(-1000)]
    public sealed class TouchInput : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public bool isDragged { private set; get; }
        public Vector2 percentSwipeDelta { private set; get; }

        Vector2 _delta;

        private void OnEnable()
        {
            percentSwipeDelta = Vector2.zero;
            isDragged = false;
        }

        private void Update()
        {
            percentSwipeDelta = _delta / GetScreenMinSide();
            _delta = Vector2.zero;
        }

        private int GetScreenMinSide()
        {
            return Mathf.Min(Screen.width, Screen.height);
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            isDragged = true;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            isDragged = false;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            _delta += eventData.delta;
        }
    }
}
