using System.Collections.Generic;

namespace Bibyter.CustomEvent
{
    public struct EventOptions
    {
        public bool needBreak;
    }

    public sealed class OrderableEvent<T>
    {
        public delegate void Action(ref T data, ref EventOptions eventOptions);

        SortedList<int, List<Action>> _orderableList;

        public OrderableEvent()
        {
            _orderableList = new SortedList<int, List<Action>>();
        }


        public void Add(Action handler, int order)
        {
            List<Action> actionList;

            if (_orderableList.ContainsKey(order))
            {
                actionList = _orderableList[order];
            }
            else
            {
                actionList = new List<Action>();
                _orderableList.Add(order, actionList);
            }

            actionList.Add(handler);
        }

        public void Del(Action action, int order)
        {
            if (_orderableList.ContainsKey(order))
            {
                var actionList = _orderableList[order];
                actionList.Remove(action);
            }
            else
            {
                throw new System.Exception("Invalid order");
            }
        }


        public void Invoke(T data)
        {
            InvokeRef(ref data);
        }

        public void InvokeRef(ref T data)
        {
            var eventOptions = new EventOptions();

            foreach (var item in _orderableList)
            {
                var list = item.Value;


                for (int i = 0; i < list.Count; i++)
                {
                    var action = list[i];
                    action.Invoke(ref data, ref eventOptions);

                    if (eventOptions.needBreak)
                    {
                        break;
                    }
                }

                if (eventOptions.needBreak)
                {
                    break;
                }
            }
        }
    }
}
