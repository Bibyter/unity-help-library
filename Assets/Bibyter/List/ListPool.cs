using System.Collections.Generic;

public static class ListPool<T>
{
    static List<List<T>> _poolList;

    static ListPool()
    {
        _poolList = new List<List<T>>();
    }

    public static List<T> Get(int capacity)
    {
        for (int i = 0; i < _poolList.Count; i++)
        {
            var iterList = _poolList[i];

            if (iterList.Capacity >= capacity && iterList.Capacity <= capacity * 4)
            {
                _poolList[i] = _poolList[_poolList.Count - 1];
                _poolList.RemoveAt(_poolList.Count - 1);

                return iterList;
            }
        }

        return new List<T>(capacity);
    }

    public static void Release(List<T> list)
    {
        list.Clear();
        _poolList.Add(list);
    }
}
