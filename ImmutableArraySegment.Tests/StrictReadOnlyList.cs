using System;
using System.Collections;
using System.Collections.Generic;

namespace Tests
{
    internal class StrictReadOnlyList<T> : IReadOnlyList<T>
	{
		private readonly List<T> inner;

		public StrictReadOnlyList(IEnumerable<T> elements)
			=> inner = new(elements);

		public T this[int index] => ((IReadOnlyList<T>)inner)[index];

		public int Count => ((IReadOnlyCollection<T>)inner).Count;

		public IEnumerator<T> GetEnumerator()
		{
			throw new InvalidOperationException("The code should not use the enumerator for lists.");
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new InvalidOperationException("The code should not use the enumerator for lists.");
		}
	}
}
