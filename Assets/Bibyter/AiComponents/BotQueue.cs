using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Environment
{
    /// <summary>
    /// Очередь для ботов
    /// Точки по которым боты следуют
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public sealed class BotQueue : MonoBehaviour
    {
        public sealed class Place
        {
            Transform _point;
            BotQueue _botQueue;
            
            public System.Object ownerObject { set; get; }

            public Place(Transform point, BotQueue botQueue)
            {
                _point = point;
                _botQueue = botQueue;
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

            public bool IsFinished()
            {
                return IsFirst() && _botQueue.botQueueJoint == null;
            }

            public int id => _point.GetSiblingIndex();

            public Place MoveToNext(out bool isUpdated)
            {
                return _botQueue.MoveToNextPlace(this, out isUpdated);
            }


            /// <summary>
            /// назначаем owner любой объект (так как не важно, просто чтобы не кто не занял), и запукаем таймер когда его очистить
            /// </summary>
            public void DelayClearOwner(float delay)
            {
                ownerObject = _botQueue;
                _botQueue.StartCoroutine(ClearOwner(this, _botQueue, delay));
            }

            private IEnumerator ClearOwner(Place place, System.Object emptyObject, float delay)
            {
                yield return new WaitForSeconds(delay);
                if (place != null && place.ownerObject == emptyObject) place.ownerObject = null;
            }
        }

        Place[] _places;
        public BotQueueJoint botQueueJoint { set; get; }

        private void Awake()
        {
            _places = new Place[transform.childCount];
            for (int i = 0; i < _places.Length; i++)
            {
                _places[i] = new Place(transform.GetChild(i), this);
            }
        }

        public int placesCount => transform.childCount;

        private Place MoveToNextPlace(Place currentPlace, out bool isUpdated)
        {
            isUpdated = false;

            if (currentPlace == null) return currentPlace;
            if (currentPlace.IsFirst())
            {
                if (botQueueJoint != null)
                {
                    var jointNextPlace = botQueueJoint.GetPlace(this);

                    if (jointNextPlace != null)
                    {
                        isUpdated = true;
                        jointNextPlace.ownerObject = currentPlace.ownerObject;
                        currentPlace.ownerObject = null;
                        return jointNextPlace;
                    }
                }

                return currentPlace;
            }

            var nextPlace = _places[currentPlace.id - 1];

            if (!nextPlace.HasOwner())
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

        public Place GetFirstPlace()
        {
            return GetPlace(0);
        }

        public Place GetLastPlace()
        {
            return GetPlace(placesCount - 1);
        }

        public Place FindFreePlace()
        {
            for (int i = 0; i < _places.Length; i++)
            {
                if (!_places[i].HasOwner())
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

        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.GetChild(i).position, 0.1f);
            }
        }

        

#if UNITY_EDITOR
        [ContextMenu(nameof(PrintInfo))]
        void PrintInfo()
        {
            var str = new System.Text.StringBuilder();

            for (int i = 0; i < placesCount; i++)
            {
                var place = GetPlace(i);

                str.Append($"Place {place.id}, hasOwner={place.HasOwner()}, ");
            }

            print(str);
        }
#endif
    }
}
