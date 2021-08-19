using System;
using System.Collections;
using System.Collections.Generic;

namespace IxMilia.ThreeMf.Collections
{
    public abstract class ListWithPredicate<T> : IList<T>
    {
        private List<T> _items;

        public ListWithPredicate()
        {
            _items = new List<T>();
        }

        public ListWithPredicate(IEnumerable<T> items)
        {
            _items = new List<T>(items);
        }

        protected abstract bool IsValidItem(T item);

        private void ValidatePredicate(T item)
        {
            if (!IsValidItem(item))
            {
                throw new InvalidOperationException("Item does not meet the criteria to be added to this collection.");
            }
        }

        public T this[int index]
        {
            get { return _items[index]; }
            set
            {
                ValidatePredicate(value);
                _items[index] = value;
            }
        }

        public int Count => _items.Count;
        public bool IsReadOnly => false;

        public void Add(T item)
        {
            ValidatePredicate(item);
            _items.Add(item);
        }

        public void Clear() => _items.Clear();
        public bool Contains(T item) => _items.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();
        public int IndexOf(T item) => _items.IndexOf(item);

        public void Insert(int index, T item)
        {
            ValidatePredicate(item);
            _items.Insert(index, item);
        }

        public bool Remove(T item) => _items.Remove(item);
        public void RemoveAt(int index) => _items.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_items).GetEnumerator();
    }
}
