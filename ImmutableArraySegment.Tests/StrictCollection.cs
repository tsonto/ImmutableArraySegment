using System;
using System.Collections;
using System.Collections.Generic;

namespace Tests
{
    internal class StrictCollection<T> : ICollection<T>
    {
        private readonly List<T> inner;

        public StrictCollection(IEnumerable<T> elements)
            => inner = new(elements);

        public int Count => ((ICollection<T>)inner).Count;

        public bool IsReadOnly => ((ICollection<T>)inner).IsReadOnly;

        public void Add(T item)
        {
            ((ICollection<T>)inner).Add(item);
        }

        public void Clear()
        {
            ((ICollection<T>)inner).Clear();
        }

        public bool Contains(T item)
        {
            return ((ICollection<T>)inner).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((ICollection<T>)inner).CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new InvalidOperationException("The code should not use the enumerator for collections.");
        }

        public bool Remove(T item)
        {
            return ((ICollection<T>)inner).Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new InvalidOperationException("The code should not use the enumerator for collections.");
        }
    }
}
