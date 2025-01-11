using System;
using System.Collections;
using System.Collections.Generic;

namespace Code.Infrastructure.Common
{
    public class CircularList<T> : IEnumerable<T>
    {
        private readonly List<T> _list = new();
        private int _currentIndex = 0;

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public int Count => _list.Count;

        public void Add(T item)
        {
            _list.Add(item);
        }

        public bool Remove(T item)
        {
            return _list.Remove(item);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            _list.AddRange(collection);
        }

        public void Clear()
        {
            _list.Clear();
            _currentIndex = 0;
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public T GetCurrent()
        {
            if (_list.Count == 0)
               return default;

            if (_currentIndex >= _list.Count) 
                _currentIndex = 0;

            T currentItem = _list[_currentIndex];

            _currentIndex++;
            return currentItem;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}