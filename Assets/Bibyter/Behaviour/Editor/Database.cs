using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.Editor
{
    [CreateAssetMenu]
    public sealed class Database : ScriptableObject
    {
        [System.Serializable]
        public struct State
        {
            public string typeName;
            public string[] stateNames;
            public bool[] foldoutValues;
            public bool statesFoldout;
            public bool behavioursFoldout;
        }
        [SerializeField] State[] _states;

        private void OnValidate()
        {
            for (int i = 0; i < _states.Length; i++)
            {
                if (_states[i].foldoutValues.Length != 32)
                {
                    _states[i].foldoutValues = new bool[32];
                }
            }
        }
    }
}