using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace task03;

public class CustomCollection<T> : IEnumerable<T>
{
    private readonly List<T> _storage = new();

    public void AddElement(T element) => _storage.Add(element);
    public bool DeleteElement(T element) => _storage.Remove(element);
    public int ElementsCount => _storage.Count;

    public IEnumerator<T> GetEnumerator() => _storage.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerable<T> GetReverse()
    {
        for (int index = _storage.Count - 1; index >= 0; index--)
        {
            yield return _storage[index];
        }
    }

    public static IEnumerable<int> CreateNumberRange(int start, int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            yield return start + i;
        }
    }

    public IEnumerable<T> ApplyFilterAndOrder(Func<T, bool> condition, Func<T, IComparable> orderKey)
    {
        return _storage.Where(condition).OrderBy(orderKey);
    }
}