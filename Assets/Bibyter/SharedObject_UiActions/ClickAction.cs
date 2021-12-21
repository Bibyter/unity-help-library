using Bibyter.CustomEvent;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Bibyter
{
    public struct ClickEvent
    {
        public string name;
    }

    public sealed class ClickAction : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] string _name;

        OrderableEvent<ClickEvent> _clickEvent;

        private void Awake()
        {
            var sharedObject = GetComponentInParent<SharedObject>();
            _clickEvent = sharedObject.GetExternalEvent<ClickEvent>();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            _clickEvent.Invoke(new ClickEvent() { name = _name });
        }
    }
}
