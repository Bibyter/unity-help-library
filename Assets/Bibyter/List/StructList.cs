using System.Runtime.CompilerServices;

public struct StructList<T>
{
    T[] _array;
    int _count;

    public StructList(int capacity)
    {
        _array = new T[capacity];
        _count = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetRef(int i)
    {
        return ref _array[i];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(in T item)
    {
        if (_count >= _array.Length)
        {
            System.Array.Resize(ref _array, _array.Length * 2);
        }

        _array[_count] = item;
        _count++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveWithLastElement(int id)
    {
        _array[id] = _array[_count - 1];
        _array[_count - 1] = default(T);
        _count--;
    }

    public void Clear()
    {
        if (_count > 0)
        {
            System.Array.Clear(_array, 0, _count);
            _count = 0;
        }
    }

    public int Count
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return _count;
        }
    }
}
