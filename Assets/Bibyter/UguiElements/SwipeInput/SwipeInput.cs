using UnityEngine;
using UnityEngine.EventSystems;

namespace Bibyter
{
    [DefaultExecutionOrder(-1000)]
    public sealed class SwipeInput : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public bool isPressed { private set; get; }
        public Vector2 direction { private set; get; }

        Vector2 _delta;

        private void OnEnable()
        {
            direction = Vector2.zero;
            isPressed = false;
        }

        private void Update()
        {
            direction = _delta / GetScreenMinSide();
            _delta = Vector2.zero;
        }

        private int GetScreenMinSide()
        {
            return Mathf.Min(Screen.width, Screen.height);
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            isPressed = true;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            isPressed = false;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            _delta += eventData.delta;
        }
    }
}
