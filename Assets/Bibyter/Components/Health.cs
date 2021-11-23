using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public sealed class Health
    {
        [SerializeField] int _current;
        [SerializeField] int _max;

        public int max => _max;
        public int current => _current;
        public bool isZero => current == 0;
        public bool isMax => current == max;
        public float normalized => current / ((float)max);

        public Health(int start)
        {
            _max = start;
            _current = start;
        }

        public Health(int current, int max)
        {
            _max = max;
            _current = current;
        }

        public int Take(int amount)
        {
            if (amount <= 0) return 0;
            int takeAmountValidated = Mathf.Min(_current, amount);
            _current = Mathf.Max(0, current - amount);
            return takeAmountValidated;
        }

        public void Heal(int amount)
        {
            if (amount <= 0) return;
            _current = Mathf.Min(current + amount, max);
        }
    }
}
