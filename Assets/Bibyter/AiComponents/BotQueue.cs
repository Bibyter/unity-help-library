using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Environment
{
    [DefaultExecutionOrder(-1)]
    public sealed class BotQueue : MonoBehaviour
    {
        public sealed class Place
        {
            Transform _point;
            
            public System.Object ownerObject { set; get; }

            public Place(Transform point)
            {
                _point = point;
            }

            public bool HasOwner()
            {
                return ownerObject != null;
            }

            public Transform pointTransform => _point;

            public bool IsFirst()
            {
                return id == 0;
            }

            public int id => _point.GetSiblingIndex();
        }

        Place[] _places;


        private void Awake()
        {
            _places = new Place[transform.childCount];
            for (int i = 0; i < _places.Length; i++)
            {
                _places[i] = new Place(transform.GetChild(i));
            }
        }

        public int placesCount => transform.childCount;

        public Place MoveToNextPlace(Place currentPlace, out bool isUpdated)
        {
            isUpdated = false;

            if (currentPlace == null) return currentPlace;
            if (currentPlace.IsFirst()) return currentPlace;

            var nextPlace = _places[currentPlace.id - 1];

            if (nextPlace.ownerObject == null)
            {
                isUpdated = true;
                nextPlace.ownerObject = currentPlace.ownerObject;
                currentPlace.ownerObject = null;
                return nextPlace;
            }

            return currentPlace;
        }

        public Place GetPlace(int i)
        {
            return _places[i];
        }

        public Place FindFreePlace()
        {
            for (int i = 0; i < _places.Length; i++)
            {
                if (_places[i].ownerObject == null)
                {
                    return _places[i];
                }
            }

            return null;
        }

        public bool HasEmptyPlace()
        {
            return FindFreePlace() != null;
        }

        private void OnDrawGizmosSelected()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.GetChild(i).position, 0.1f);
            }
        }
    }
}
