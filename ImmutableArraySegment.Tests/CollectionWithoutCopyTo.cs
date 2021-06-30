using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    internal class CollectionWithoutCopyTo<T> : ICollection<T>
    {
        private readonly T[] values;

        public CollectionWithoutCopyTo(IEnumerable<T> values)
        {
            this.values = values.ToArray();
        }

        public int Count => values.Length;
        public bool IsReadOnly => true;

        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(T item) => values.Contains(item);

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new InvalidOperationException("The code should not use CopyTo for collections.");
        }

        public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)values).GetEnumerator();

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => values.GetEnumerator();
    }
}
