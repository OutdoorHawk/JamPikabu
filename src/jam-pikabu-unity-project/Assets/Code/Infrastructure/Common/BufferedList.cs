using System.Collections;
using System.Collections.Generic;

namespace Code.Infrastructure.Common
{
    /// <summary>
    /// Лист с буффером для добавления и удаления элементов в процессе энумерации
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BufferedList<T> : IEnumerable<T> where T : class
    {
        private readonly List<T> _list = new();
        private readonly List<T> _buffer = new();

        public BufferedList()
        {
        }

        public BufferedList(int capacity)
        {
            _list = new List<T>(capacity);
            _buffer = new List<T>(capacity);
        }

        public T this[int index] => _list[index];

        public int Count => _list.Count;

        public void Add(T handler)
        {
            _list.Add(handler);
        }

        public void AddRange(List<T> collection)
        {
            _list.AddRange(collection);
        }

        public void Remove(T handler)
        {
            _list.Remove(handler);
        }

        public void AddRange(IEnumerable<T> handlers)
        {
            _list.AddRange(handlers);
        }

        public void Clear()
        {
            _list.Clear();
            _buffer.Clear();
        }

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            _buffer.Clear();
            _buffer.AddRange(_list);
            return _buffer.GetEnumerator();
        }

        #endregion
    }
}