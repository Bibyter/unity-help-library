using System.Collections.Generic;
using UnityEngine;

namespace Bibyter.DependencyInjection
{
    [DefaultExecutionOrder(-10000)]
    public sealed class SceneLinkRegistrator : MonoBehaviour
    {
        [SerializeField] Object[] _typeLinks;


        [System.Serializable]
        public struct NamedLink
        {
            public string name;
            public Object link;
        }
        public NamedLink[] _namedLinks;


        private void Awake()
        {
            for (int i = 0; i < _typeLinks.Length; i++)
            {
                _sharedDependencies.Add(_typeLinks[i]);
            }

            for (int i = 0; i < _namedLinks.Length; i++)
            {
                ref var namedLink = ref _namedLinks[i];
                AddNamedLink(namedLink.name, namedLink.link);
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _typeLinks.Length; i++)
            {
                _sharedDependencies.Remove(_typeLinks[i]);
            }

            for (int i = 0; i < _namedLinks.Length; i++)
            {
                ref var namedLink = ref _namedLinks[i];
                DelNamedLink(namedLink.name);
            }
        }


        #region
        static List<object> _sharedDependencies = new List<object>(16);

        public static T GetLink<T>() where T : class
        {
            for (int i = 0; i < _sharedDependencies.Count; i++)
            {
                if (_sharedDependencies[i] is T)
                {
                    return _sharedDependencies[i] as T;
                }
            }

            return null;
        }

        public static void AddLink(object value)
        {
            _sharedDependencies.Add(value);
        }

        public static void DelLink(object value)
        {
            _sharedDependencies.Remove(value);
        }
        #endregion

        #region
        static Dictionary<string, object> _sharedNamedLinks = new Dictionary<string, object>(4);

        public static T GetNamedLink<T>(string name) where T : class
        {
#if DEBUG
            if (!_sharedNamedLinks.ContainsKey(name))
            {
                throw new System.Exception($"Not Found Link {name}");
            }
#endif

            object link = _sharedNamedLinks[name];

#if DEBUG
            if (!(link is T))
            {
                throw new System.Exception($"Invalid type {link.GetType().FullName} of link:{name}, need type {typeof(T).FullName}");
            }
#endif

            return link as T;
        }

        public static void AddNamedLink(string name, object link)
        {
            _sharedNamedLinks.Add(name, link);
        }

        public static void DelNamedLink(string name)
        {
            _sharedNamedLinks.Remove(name);
        }
        #endregion

    }
}