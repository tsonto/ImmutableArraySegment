using System.Collections;
using System.Collections.Generic;

namespace Tests
{
	internal class StrictEnumerable<T> : IEnumerable<T>
	{
		private readonly List<T> inner;

		public StrictEnumerable(IEnumerable<T> elements)
			=> inner = new(elements);

		public IEnumerator<T> GetEnumerator()
		{
			return ((IEnumerable<T>)inner).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)inner).GetEnumerator();
		}
	}
}