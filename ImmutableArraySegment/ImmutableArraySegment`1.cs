using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Tsonto.Collections.Generic
{
	public readonly struct ImmutableArraySegment<T> : IImmutableList<T>, IEnumerable<T>, IReadOnlyList<T>, IReadOnlyCollection<T>
	{
		internal readonly T[] data;
		internal readonly int ourStart;
		internal readonly int ourLength;

		internal int ourEnd
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ourStart + ourLength;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArraySegment(in ReadOnlySpan<T> span)
		{
			data = span.ToArray();
			ourStart = 0;
			ourLength = data.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArraySegment(in Span<T> span)
		{
			data = span.ToArray();
			ourStart = 0;
			ourLength = data.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArraySegment(in ReadOnlyMemory<T> memory)
		{
			data = memory.ToArray();
			ourStart = 0;
			ourLength = data.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArraySegment(in Memory<T> memory)
		{
			data = memory.ToArray();
			ourStart = 0;
			ourLength = data.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArraySegment(T[] array)
		{
			data = array.ToArray();
			ourStart = 0;
			ourLength = data.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		public ImmutableArraySegment(T[] array, Range range)
		{
			(int start, int length) = range.GetOffsetAndLength(array.Length);
			data = new T[length];
			Array.Copy(array, start, data, 0, length);
			ourStart = 0;
			ourLength = data.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		public ImmutableArraySegment(IEnumerable<T> source)
		{
			if (source is T[] sourceArray)
			{
				data = sourceArray.ToArray();
				ourStart = 0;
				ourLength = data.Length;
			}
			else if (source is ImmutableArraySegment<T> sourceIM)
			{
				this = sourceIM;
			}
			else if (source is ArraySegment<T> sourceArraySegment)
			{
				data = new T[sourceArraySegment.Count];
				sourceArraySegment.CopyTo(data);
				ourStart = 0;
				ourLength = data.Length;
			}
			else if (source is ICollection<T> sourceCollection)
			{
				data = new T[sourceCollection.Count];
				sourceCollection.CopyTo(data, 0);
				ourStart = 0;
				ourLength = data.Length;
			}
			else if (source is IReadOnlyList<T> sourceROList)
			{
				data = new T[sourceROList.Count];
				for (int i = 0; i < data.Length; ++i)
					data[i] = sourceROList[i];
				ourStart = 0;
				ourLength = data.Length;
			}
			else
			{
				data = source.ToArray();
				ourStart = 0;
				ourLength = data.Length;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		public ImmutableArraySegment(IEnumerable<T> source, Range range)
		{
			if (source is T[] sourceArray)
			{
				data = sourceArray[range];
				ourStart = 0;
				ourLength = data.Length;
			}
			else if (range.Equals(Range.All))
			{
				this = new(source);
			}
			else if (source is ImmutableArraySegment<T> sourceIM)
			{
				this = sourceIM[range];
			}
			else if (source is ArraySegment<T> sourceArraySegment)
			{
				(int rangeStart, int rangeLength) = range.GetOffsetAndLength(sourceArraySegment.Count);
				data = new T[rangeLength];
				Array.Copy(sourceArraySegment.Array!, rangeStart + sourceArraySegment.Offset, data, 0, rangeLength);
				ourStart = 0;
				ourLength = rangeLength;
			}
			else if (source is IReadOnlyList<T> sourceROList)
			{
				(int rangeStart, int rangeLength) = range.GetOffsetAndLength(sourceROList.Count);
				data = new T[rangeLength];
				for (int i = 0; i < rangeLength; ++i)
					data[i] = sourceROList[i + rangeStart];
				ourStart = 0;
				ourLength = data.Length;
			}
			else if (source is ICollection<T> sourceCollection)
			{
				int collectionLength = sourceCollection.Count;
				(int rangeStart, int rangeLength) = range.GetOffsetAndLength(collectionLength);
				if (rangeStart == 0 && rangeLength == collectionLength)
				{
					data = new T[collectionLength];
					sourceCollection.CopyTo(data, 0);
				}
				else
				{
					data = source.Skip(rangeStart).Take(rangeLength).ToArray();
				}
				ourStart = 0;
				ourLength = data.Length;
			}
			else if (!range.End.IsFromEnd && !range.Start.IsFromEnd)
			{
				int rangeStart = range.Start.Value;
				int rangeLength = range.End.Value - rangeStart;
				data = source.Skip(rangeStart).Take(rangeLength).ToArray();
				ourStart = 0;
				ourLength = data.Length;
			}
			else
			{
				int sequenceLength = source.Count();
				(int rangeStart, int rangeLength) = range.GetOffsetAndLength(sequenceLength);
				data = source.Skip(rangeStart).Take(rangeLength).ToArray();
				ourStart = 0;
				ourLength = data.Length;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArraySegment(T[] array, int offset, int length)
		{
			data = new T[length];
			Array.Copy(array, offset, data, 0, length);
			ourStart = 0;
			ourLength = data.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ImmutableArraySegment(T[] array, bool raw)
		{
			data = array;
			ourStart = 0;
			ourLength = data.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ImmutableArraySegment(T[] array, int offset, int length, bool raw)
		{
			data = array;
			ourStart = offset;
			ourLength = length;
		}

		public static ImmutableArraySegment<T> Empty
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => default;
		}

		public int Length
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ourLength;
		}

		/// <inheritdoc/>
		int IReadOnlyCollection<T>.Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ourLength;
		}

		public T this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => data[ourStart + index];
		}

		public T this[Index index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => data[ourStart + index.GetOffset(ourLength)];
		}

		public ImmutableArraySegment<T> this[Range range]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				(int newOffset, int newLength) = range.GetOffsetAndLength(ourLength);
				return Slice(newOffset, newLength);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArraySegment<T> Slice(int offset, int length)
		{
			if (offset < 0 || offset > length || length < 0 || offset + length > ourLength)
				throw new IndexOutOfRangeException();
			return new(data, ourStart + offset, length, true);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArraySegment<T> Slice(int offset)
		{
			if (offset < 0 || offset > ourLength)
				throw new IndexOutOfRangeException();
			return new(data, ourStart + offset, ourLength - offset, true);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerator<T> GetEnumerator() => new Enumerator(this);

		private class Enumerator : IEnumerator<T>
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Enumerator(ImmutableArraySegment<T> parent)
			{
				position = parent.ourStart - 1;
				this.parent = parent;
			}

			private int position;
			private readonly ImmutableArraySegment<T> parent;

			public T Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
				get => position >= parent.ourStart && position < parent.ourEnd
					? parent.data[position]
					: throw new InvalidOperationException();
			}

			object IEnumerator.Current => Current!;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Dispose()
			{
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				++position;
				return position < parent.ourEnd;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Reset()
				=> position = parent.ourStart - 1;
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArraySegment<T> Append(T value)
		{
			var newArray = new T[ourLength + 1];
			Array.Copy(data, ourStart, newArray, 0, ourLength);
			newArray[ourLength] = value;
			return new(newArray, raw: true);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArraySegment<T> Prepend(T value)
		{
			var newArray = new T[ourLength + 1];
			Array.Copy(data, ourStart, newArray, 1, ourLength);
			data[0] = value;
			return new(newArray, raw: true);
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IImmutableList<T> IImmutableList<T>.Add(T value)
			=> Append(value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArraySegment<T> Append(IEnumerable<T> items)
		{
			T[] newArray = AllocateAndCopy(items, ourLength, ourLength);
			Array.Copy(data, ourStart, newArray, 0, ourLength);
			return new(newArray, true);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArraySegment<T> Append(in ReadOnlyMemory<T> items)
		{
			T[] newArray = AllocateAndCopyReadOnlyMemoryStruct(ourLength, ourLength, items);
			Array.Copy(data, ourStart, newArray, 0, ourLength);
			return new(newArray, true);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArraySegment<T> Append(in Memory<T> items)
		{
			T[] newArray = AllocateAndCopyMemoryStruct(ourLength, ourLength, items);
			Array.Copy(data, ourStart, newArray, 0, ourLength);
			return new(newArray, true);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArraySegment<T> Append(in ReadOnlySpan<T> items)
		{
			T[] newArray = AllocateAndCopyReadOnlySpan(ourLength, ourLength, items);
			Array.Copy(data, ourStart, newArray, 0, ourLength);
			return new(newArray, true);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArraySegment<T> Append(in Span<T> items)
		{
			T[] newArray = AllocateAndCopySpan(ourLength, ourLength, items);
			Array.Copy(data, ourStart, newArray, 0, ourLength);
			return new(newArray, true);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static T[] AllocateAndCopy(IEnumerable<T> source, int destOffset, int extraDestLength)
		{
			if (source is T[] sourceArray)
				return AllocateAndCopyArray(destOffset, extraDestLength, sourceArray);
			else if (source is ImmutableArraySegment<T> sourceIM)
				return AllocateAndCopyImmutableArraySegment(destOffset, extraDestLength, sourceIM);
			else if (source is ArraySegment<T> sourceArraySegment)
				return AllocateAndCopyArraySegment(destOffset, extraDestLength, sourceArraySegment);
			else if (source is IReadOnlyList<T> sourceROList)
				return AllocateAndCopyReadOnlyList(destOffset, extraDestLength, sourceROList);
			else if (source is ICollection<T> sourceCollection)
				return AllocateAndCopyCollection(destOffset, extraDestLength, sourceCollection);
			else
				return AllocateAndCopyOtherEnumerable(destOffset, extraDestLength, source);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		internal static T[] AllocateAndCopyArraySegment(int destOffset, int extraDestLength, ArraySegment<T> source)
		{
			var dest = new T[source.Count + extraDestLength];
			source.CopyTo(dest, destOffset);
			return dest;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		internal static T[] AllocateAndCopyImmutableArraySegment(int destOffset, int extraDestLength, ImmutableArraySegment<T> source)
		{
			var dest = new T[source.Length + extraDestLength];
			source.CopyTo(dest, destOffset);
			return dest;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		internal static T[] AllocateAndCopyCollection(int destOffset, int extraDestLength, ICollection<T> source)
		{
			var dest = new T[source.Count + extraDestLength];
			source.CopyTo(dest, destOffset);
			return dest;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		internal static T[] AllocateAndCopyOtherEnumerable(int destOffset, int extraDestLength, IEnumerable<T> source)
		{
			int sourceLength = source.Count();
			var dest = new T[sourceLength + extraDestLength];
			var enumerator = source.GetEnumerator();
			for (int i = 0; i < sourceLength; ++i)
			{
				if (!enumerator.MoveNext())
					throw new InvalidOperationException("The input sequence is shorter the second time than the first time.");
				dest[i + destOffset] = enumerator.Current;
			}
			if (enumerator.MoveNext())
				throw new InvalidOperationException("The input sequence is longer the second time than the first time.");
			return dest;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		internal static T[] AllocateAndCopyReadOnlyList(int destOffset, int extraDestLength, IReadOnlyList<T> source)
		{
			var dest = new T[source.Count + extraDestLength];
			for (int i = 0; i < source.Count; ++i)
				dest[i + destOffset] = source[i];
			return dest;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		internal static T[] AllocateAndCopyMemoryStruct(int destOffset, int extraDestLength, in Memory<T> source)
		{
			var dest = new T[source.Length + extraDestLength];
			source.CopyTo(new Memory<T>(dest)[destOffset..]);
			return dest;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		internal static T[] AllocateAndCopyReadOnlyMemoryStruct(int destOffset, int extraDestLength, in ReadOnlyMemory<T> source)
		{
			var dest = new T[source.Length + extraDestLength];
			source.CopyTo(new Memory<T>(dest)[destOffset..]);
			return dest;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		internal static T[] AllocateAndCopyReadOnlySpan(int destOffset, int extraDestLength, in ReadOnlySpan<T> source)
		{
			var dest = new T[source.Length + extraDestLength];
			source.CopyTo(new Span<T>(dest)[destOffset..]);
			return dest;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		internal static T[] AllocateAndCopySpan(int destOffset, int extraDestLength, in Span<T> source)
		{
			var dest = new T[source.Length + extraDestLength];
			source.CopyTo(new Span<T>(dest)[destOffset..]);
			return dest;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		internal static T[] AllocateAndCopyArray(int destOffset, int extraDestLength, T[] source)
		{
			var dest = new T[source.Length + extraDestLength];
			Array.Copy(source, 0, dest, destOffset, source.Length);
			return dest;
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IImmutableList<T> IImmutableList<T>.AddRange(IEnumerable<T> items)
			=> Append(items);

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		public void CopyTo(T[] dest, int destOffset, int length)
		{
			if (destOffset < 0 || destOffset > dest.Length)
				throw new ArgumentOutOfRangeException(nameof(destOffset));
			if (length > ourLength | destOffset + length > dest.Length)
				throw new ArgumentOutOfRangeException(nameof(length));

			Array.Copy(data, ourStart, dest, destOffset, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(T[] dest, int destOffset)
			=> CopyTo(dest, destOffset, ourLength);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(in Span<T> dest, int destOffset)
			=> CopyTo(in dest, destOffset, ourLength);

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		public void CopyTo(in Span<T> dest, int destOffset, int length)
		{
			if (destOffset < 0 || destOffset > dest.Length)
				throw new ArgumentOutOfRangeException(nameof(destOffset));
			if (length > ourLength | destOffset + length > dest.Length)
				throw new ArgumentOutOfRangeException(nameof(length));

			for (int i = 0; i < length; ++i)
				dest[i + destOffset] = data[i + ourStart];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(in Memory<T> dest, int destOffset)
			=> CopyTo(in dest, destOffset, ourLength);

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		public void CopyTo(in Memory<T> dest, int destOffset, int length)
			=> CopyTo(dest.Span, destOffset, length);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IImmutableList<T> IImmutableList<T>.Clear()
			=> Empty;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item)
			=> IndexOf(item, 0, ourLength, (IEqualityComparer<T>?)null);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(in T item, int index)
			=> IndexOf(in item, index, ourLength - index, (IEqualityComparer<T>?)null);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item, int index, int count)
			=> IndexOf(item, index, count, (IEqualityComparer<T>?)null);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item, IEqualityComparer<T>? equalityComparer)
			=> IndexOf(item, 0, ourLength, equalityComparer);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexOf(T item, int index, IEqualityComparer<T>? equalityComparer)
			=> IndexOf(item, index, ourLength - index, equalityComparer);

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		public int IndexOf(in T item, int index, int count, IEqualityComparer<T>? equalityComparer)
		{
			if (index < 0 || index > ourLength)
				throw new ArgumentOutOfRangeException(nameof(index));
			if (count < 0 || index + count > ourLength)
				throw new ArgumentOutOfRangeException(nameof(count));

			if (equalityComparer is null)
			{
				var result = Array.IndexOf(data, item, index + ourStart, count);
				if (result == -1)
					return -1;
				else
					return result - ourStart;
			}

			for (int i = index; i < index + count; ++i)
				if (equalityComparer.Equals(item, data[i + ourStart]))
					return i;
			return -1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		public int IndexOf(in T item, FastEqualityFunction<T> areEqual)
			=> IndexOf(in item, 0, ourLength, areEqual);

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		public int IndexOf(in T item, int index, FastEqualityFunction<T> areEqual)
			=> IndexOf(in item, index, ourLength - index, areEqual);

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		public int IndexOf(in T item, int index, int count, FastEqualityFunction<T> areEqual)
		{
			if (index < 0 || index > ourLength)
				throw new ArgumentOutOfRangeException(nameof(index));
			if (count < 0 || index + count > ourLength)
				throw new ArgumentOutOfRangeException(nameof(count));
			if (areEqual is null)
				throw new ArgumentNullException(nameof(areEqual));

			for (int i = index; i < index + count; ++i)
				if (areEqual(in item, in data[i + ourStart]))
					return i;
			return -1;
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		int IImmutableList<T>.IndexOf(T item, int index, int count, IEqualityComparer<T>? equalityComparer)
			=> IndexOf(item, index, count, equalityComparer);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArraySegment<T> Insert(Index index, in T element)
		{
			var indexVal = index.GetOffset(ourLength);

			if (indexVal == ourLength)
				return Append(element);
			if (indexVal == 0)
				return Prepend(element);
			if (indexVal < 0 || indexVal > ourLength)
				throw new ArgumentOutOfRangeException(nameof(index));

			var dest = new T[ourLength + 1];

			Array.Copy(data, ourStart, dest, 0, indexVal);
			dest[indexVal] = element;
			Array.Copy(data, ourStart + indexVal, dest, indexVal + 1, indexVal);

			return new(dest, raw: true);
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IImmutableList<T> IImmutableList<T>.Insert(int index, T element)
			=> Insert(index, element);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArraySegment<T> Insert(Index index, T[] items)
		{
			var indexVal = index.GetOffset(ourLength);

			if (indexVal == ourLength)
				return Append(items);
			if (indexVal < 0 || indexVal > ourLength)
				throw new ArgumentOutOfRangeException(nameof(index));

			var dest = AllocateAndCopyArray(indexVal, ourLength, items);
			if (indexVal > 0)
				Array.Copy(data, ourStart, dest, 0, indexVal);
			Array.Copy(data, ourStart + indexVal, dest, indexVal, ourLength - indexVal);

			return new(dest, raw: true);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArraySegment<T> Insert(Index index, IEnumerable<T> items)
		{
			var indexVal = index.GetOffset(ourLength);

			if (indexVal == ourLength)
				return Append(items);
			if (indexVal < 0 || indexVal > ourLength)
				throw new ArgumentOutOfRangeException(nameof(index));

			var dest = AllocateAndCopy(items, indexVal, ourLength);
			if (indexVal > 0)
				Array.Copy(data, ourStart, dest, 0, indexVal);
			Array.Copy(data, ourStart + indexVal, dest, indexVal, ourLength - indexVal);

			return new(dest, raw: true);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArraySegment<T> Insert(Index index, in ReadOnlySpan<T> items)
		{
			var indexVal = index.GetOffset(ourLength);

			if (indexVal == ourLength)
				return Append(in items);
			if (indexVal < 0 || indexVal > ourLength)
				throw new ArgumentOutOfRangeException(nameof(index));

			var dest = AllocateAndCopyReadOnlySpan(indexVal, ourLength, in items);
			if (indexVal > 0)
				Array.Copy(data, ourStart, dest, 0, indexVal);
			Array.Copy(data, ourStart + indexVal, dest, indexVal, ourLength - indexVal);

			return new(dest, raw: true);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArraySegment<T> Insert(Index index, in Span<T> items)
		{
			var indexVal = index.GetOffset(ourLength);

			if (indexVal == ourLength)
				return Append(in items);
			if (indexVal < 0 || indexVal > ourLength)
				throw new ArgumentOutOfRangeException(nameof(index));

			var dest = AllocateAndCopySpan(indexVal, ourLength, in items);
			if (indexVal > 0)
				Array.Copy(data, ourStart, dest, 0, indexVal);
			Array.Copy(data, ourStart + indexVal, dest, indexVal, ourLength - indexVal);

			return new(dest, raw: true);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArraySegment<T> Insert(Index index, in ReadOnlyMemory<T> items)
		{
			var indexVal = index.GetOffset(ourLength);

			if (indexVal == ourLength)
				return Append(in items);
			if (indexVal < 0 || indexVal > ourLength)
				throw new ArgumentOutOfRangeException(nameof(index));

			var dest = AllocateAndCopyReadOnlyMemoryStruct(indexVal, ourLength, in items);
			if (indexVal > 0)
				Array.Copy(data, ourStart, dest, 0, indexVal);
			Array.Copy(data, ourStart + indexVal, dest, indexVal, ourLength - indexVal);

			return new(dest, raw: true);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ImmutableArraySegment<T> Insert(Index index, in Memory<T> items)
		{
			var indexVal = index.GetOffset(ourLength);

			if (indexVal == ourLength)
				return Append(in items);
			if (indexVal < 0 || indexVal > ourLength)
				throw new ArgumentOutOfRangeException(nameof(index));

			var dest = AllocateAndCopyMemoryStruct(indexVal, ourLength, in items);
			if (indexVal > 0)
				Array.Copy(data, ourStart, dest, 0, indexVal);
			Array.Copy(data, ourStart + indexVal, dest, indexVal, ourLength - indexVal);

			return new(dest, raw: true);
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IImmutableList<T> IImmutableList<T>.InsertRange(int index, IEnumerable<T> items)
			=> Insert(index, items);

		int IImmutableList<T>.LastIndexOf(T item, int index, int count, IEqualityComparer<T>? equalityComparer)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		IImmutableList<T> IImmutableList<T>.Remove(T value, IEqualityComparer<T>? equalityComparer)
		{
			throw new NotImplementedException();
		}

		public ImmutableArraySegment<T> RemoveAll(Predicate<T> match)
		{
			var output = new List<T>();

			for (int i = 0; i < ourLength; ++i)
			{
				T item = data[i + ourStart];
				if (!match(item))
					output.Add(item);
			}

			if (output.Count == 0)
				return Empty;
			else if (output.Count == ourLength)
				return this;
			else
				return new(output.ToArray(), raw: true);
		}

		/// <inheritdoc/>
		IImmutableList<T> IImmutableList<T>.RemoveAll(Predicate<T> match)
			=> RemoveAll(match);

		/// <inheritdoc/>
		IImmutableList<T> IImmutableList<T>.RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		IImmutableList<T> IImmutableList<T>.RemoveRange(IEnumerable<T> items, IEqualityComparer<T>? equalityComparer)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		IImmutableList<T> IImmutableList<T>.RemoveRange(int index, int count)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		IImmutableList<T> IImmutableList<T>.Replace(T oldValue, T newValue, IEqualityComparer<T>? equalityComparer)
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc/>
		IImmutableList<T> IImmutableList<T>.SetItem(int index, T value)
		{
			throw new NotImplementedException();
		}

		public bool SequenceEquals(IEnumerable<T> other)
			=> SequenceEquals(other, EqualityComparer<T>.Default);

		public bool SequenceEquals(IEnumerable<T> other, IEqualityComparer<T> equalityComparer)
		{
			if (other is IReadOnlyCollection<T> roc)
			{
				if (roc.Count != ourLength)
					return false;
			}
			else if (other is ICollection col)
			{
				if (col.Count != ourLength)
					return false;
			}

			int i = 0;
			var enumerator = other.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (i >= ourLength)
					return false;
				if (!equalityComparer.Equals(data[i + ourStart], enumerator.Current))
					return false;
				++i;
			}
			return i == ourLength;
		}

		public bool SequenceEquals(ImmutableArraySegment<T> other)
			=> SequenceEquals(other, EqualityComparer<T>.Default);

		public bool SequenceEquals(ImmutableArraySegment<T> other, IEqualityComparer<T> equalityComparer)
		{
			if (ourLength != other.ourLength)
				return false;
			for (int i = 0; i < ourLength; ++i)
				if (!equalityComparer.Equals(data[i + ourStart], other.data[i + other.ourStart]))
					return false;
			return true;
		}

		public bool SequenceEquals(IReadOnlyList<T> other, IEqualityComparer<T>? equalityComparer)
		{
			equalityComparer ??= EqualityComparer<T>.Default;

			if (ourLength != other.Count)
				return false;
			for (int i = 0; i < ourLength; ++i)
				if (!equalityComparer.Equals(data[i + ourStart], other[i]))
					return false;
			return true;
		}

		public T[] ToArray()
		{
			var output = new T[ourLength];
			Array.Copy(data, ourStart, output, 0, ourLength);
			return output;
		}

		public override string ToString()
			=> $"{{ImmutableArraySegment<{typeof(T).Name}>, length {ourLength}}}";
	}
}