using Bibyter.CustomEvent;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Bibyter
{
    public struct ClickSlotEvent
    {
        public string name;
        public int id;
    }

    public sealed class ClickSlotAction : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] Transform _indexedTransform;
        [SerializeField] string _name;

        OrderableEvent<ClickSlotEvent> _clickEvent;

        private void Awake()
        {
            var sharedObject = GetComponentInParent<InjectionNode>();
            _clickEvent = sharedObject.GetExternalEvent<ClickSlotEvent>();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            _clickEvent.Invoke(new ClickSlotEvent() { name = _name, id = _indexedTransform.GetSiblingIndex() });
        }
    }
}
