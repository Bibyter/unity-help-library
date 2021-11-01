using UnityEngine;

namespace Bibyter
{
    [DefaultExecutionOrder(-1000)]
    public sealed class TouchInput : MonoBehaviour
    {
        public Vector2 percentSwipeDelta { private set; get; }

        Vector2 _prevMousePosition;

        private void OnEnable()
        {
            _prevMousePosition = Input.mousePosition;
        }

        private void Update()
        {
            var mousePosition = (Vector2)Input.mousePosition;
            var canSetSwipe = Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0);

            percentSwipeDelta = canSetSwipe ? (mousePosition - _prevMousePosition) / GetScreenMinSide() : Vector2.zero;

            _prevMousePosition = mousePosition;
        }

        private int GetScreenMinSide()
        {
            return Mathf.Min(Screen.width, Screen.height);
        }
    }
}
