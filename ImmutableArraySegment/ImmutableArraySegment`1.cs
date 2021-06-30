using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Tsonto.Collections.Generic
{
    /// <summary>
    /// Provides a view into an immutable array.
    /// </summary>
    /// <typeparam name="T">The element type of the array.</typeparam>
    /// <remarks>
    /// <para>
    /// Objects of this type are immutable, and they guarantee that the array they point to is immutable. However (like other .NET
    /// array types), if the type <typeparamref name="T"/> is not immutable then the elements themselves may change. Example: since
    /// <c>SortedSet&lt;int&gt;</c> is mutable, for an <c>ImmutableArraySegment&lt;SortedSet&lt;int&gt;&gt; s</c>, <c>s[0]</c> will
    /// always refer to the same set object, but the contents of that object may vary over time.
    /// </para>
    /// <para>Comparison with built-int types:
    /// <list type="bullet">
    /// <item>This type is, in many ways, a drop-in replacement for <see cref="ImmutableArray{T}"/>. ImmutableArraySegment's
    /// advantages are that it provides more functionality and does much of it (such as getting array slices) faster.</item>
    /// <item>This type can be seen as an immutable version of <see cref="ArraySegment{T}"/>, although ImmutableArraySegment
    /// provides more features.</item>
    /// <item>
    /// </item>
    /// It's also somewhat similar to <see cref="ReadOnlyMemory{T}"/>, but only works with managed memory. (And, of course,
    /// ReadOnlyMemory doesn't actually guarantee immutability, since the data it wraps could be changed by other code.)
    /// </list>
    /// </para>
    /// <para>
    /// This package's developers have taken care to optimize this type for performance, both in terms of speed and memory usage.
    /// Additionally, every method's documentation provides information about its performance characteristics.
    /// </para>
    /// </remarks>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableArraySegment"/>
        /// </summary>
        /// <param name="source">The contents to copy to the array.</param>
        /// <remarks>This operation is O(n) for speed and memory.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableArraySegment(in ReadOnlySpan<T> source)
        {
            data = source.ToArray();
            ourStart = 0;
            ourLength = data.Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableArraySegment"/>
        /// </summary>
        /// <param name="source">The contents to copy to the array.</param>
        /// <remarks>This operation is O(n) for speed and memory.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableArraySegment(in Span<T> source)
        {
            data = source.ToArray();
            ourStart = 0;
            ourLength = data.Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableArraySegment"/>
        /// </summary>
        /// <param name="source">The contents to copy to the array.</param>
        /// <remarks>This operation is O(n) for speed and memory.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableArraySegment(in ReadOnlyMemory<T> source)
        {
            data = source.ToArray();
            ourStart = 0;
            ourLength = data.Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableArraySegment"/>
        /// </summary>
        /// <param name="source">The contents to copy to the array.</param>
        /// <remarks>This operation is O(n) for speed and memory.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableArraySegment(in Memory<T> source)
        {
            data = source.ToArray();
            ourStart = 0;
            ourLength = data.Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableArraySegment"/>
        /// </summary>
        /// <param name="source">The contents to copy to the array.</param>
        /// <remarks>This operation is O(n) for speed and memory.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableArraySegment(T[] source)
        {
            ValidateNotNull(source);

            data = source.ToArray();
            ourStart = 0;
            ourLength = data.Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableArraySegment"/>
        /// </summary>
        /// <param name="source">The contents to copy to the array.</param>
        /// <remarks>This operation is O(r) for speed and memory, where r is the length of the range.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The requested range is invalid, or is invalid for the length of the source.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public ImmutableArraySegment(T[] source, Range range)
        {
            ValidateNotNull(source);

            (int start, int length) = range.GetOffsetAndLength(source.Length);
            data = new T[length];
            Array.Copy(source, start, data, 0, length);
            ourStart = 0;
            ourLength = data.Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableArraySegment"/>
        /// </summary>
        /// <param name="source">The contents to copy to the array.</param>
        /// <remarks>
        /// This operation's performance varies based on the actual type of the input and which interfaces it
        /// implements:
        /// <list type="bullet">
        /// <item><c><typeparamref name="T"/>[]</c>: O(n)</item>
        /// <item><c>ImmutableArraySegment&lt;<typeparamref name="T"/>&gt;</c>: O(1)</item>
        /// <item><c>ArraySegment&lt;<typeparamref name="T"/>&gt;</c>: O(n), where n is the length of the segment
        /// rather than of its backing array</item>
        /// <item><c>ICollection&lt;<typeparamref name="T"/>&gt;</c>: O(n)</item>
        /// <item><c>IReadOnlyList&lt;<typeparamref name="T"/>&gt;</c>: O(n)</item>
        /// <item>otherwise: same as ToArray, which is approximately O(n*log(n)); iterates through the sequence only once</item>
        /// </list>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableArraySegment"/>
        /// </summary>
        /// <param name="source">The contents to copy to the array.</param>
        /// <param name="range">The range of items from the source.</param>
        /// <remarks>
        /// This operation's performance varies based on the actual type of the input and which interfaces it
        /// implements. n refers to the full length of the source, r to the length of the range, and s to the
        /// start index of the range:
        /// <list type="bullet">
        /// <item><c><typeparamref name="T"/>[]</c>: O(r)</item>
        /// <item><c>ImmutableArraySegment&lt;<typeparamref name="T"/>&gt;</c>: O(1)</item>
        /// <item><c>ArraySegment&lt;<typeparamref name="T"/>&gt;</c>: O(r)</item>
        /// <item><c>ICollection&lt;<typeparamref name="T"/>&gt; with n == r</c>: O(r)</item>
        /// <item><c>IReadOnlyList&lt;<typeparamref name="T"/>&gt;</c>: O(n)</item>
        /// <item><c>ICollection&lt;<typeparamref name="T"/>&gt; with n != r</c>: O(s) + O(r*log(r)); sequence
        /// is only enunmerated once, and not past s+r</item>
        /// <item>otherwise, if r is implicit in how the range was given: O(s) + O(r*log(r)); sequence is only
        /// enumerated once, and not past s+r</item>
        /// <item,>otherwise, O(n) + O(s) + O(r*log(r)); sequence is fully enumerated and then partially
        /// enumerated</item>
        /// </list>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The requested range is invalid, or is invalid for the
        /// length of the source.</exception>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableArraySegment"/>
        /// </summary>
        /// <param name="source">The source of contents to copy to the array.</param>
        /// <param name="offset">How many elements from the source's start to begin copying from.</param>
        /// <param name="length">How many elements to copy from the source.</param>
        /// <remarks>This operation is O(r) for speed and memory, where r is the provided length rather than
        /// the length of the source array.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The requested range is invalid, or is invalid for the
        /// length of the source.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableArraySegment(T[] source, int offset, int length)
        {
            ValidateNotNull(source);
            ValidateOffsetAndLength(source.Length, offset, length);

            data = new T[length];
            Array.Copy(source, offset, data, 0, length);
            ourStart = 0;
            ourLength = data.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ImmutableArraySegment(T[] array, bool raw)
        {
            if (raw)
            {
                data = array;
                ourStart = 0;
                ourLength = data.Length;
            }
            else
            {
                this = new(array);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ImmutableArraySegment(T[] array, int offset, int length, bool raw)
        {
            if (raw)
            {
                data = array;
                ourStart = offset;
                ourLength = length;
            }
            else
            {
                this = new(array, offset, length);
            }
        }

        /// <summary>
        /// Gets an empty instance of <see cref="ImmutableArraySegment{T}"/>.
        /// </summary>
        /// <remarks>When performance is more important than readability, just use
        /// <c>default(IImutableArraySegment&lt;Whatever&gt;)</c> instead, to avoid a copy.</remarks>
        /// <remarks>This operation is O(1) for time and memory.</remarks>
        public static ImmutableArraySegment<T> Empty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => default;
        }

        /// <summary>
        /// Gets 
        /// </summary>
        /// <remarks>This operation is O(1) for time and memory.</remarks>
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

        /// <summary>
        /// Gets the element at the given index.
        /// </summary>
        /// <param name="index">The position to read from, relative to the start of this object's view of
        /// the underlying array.</param>
        /// <returns>The element.</returns>
        /// <remarks>This operation is O(1) for time and memory. For value types, and especially large value 
        /// types, you can avoid a copy operation by using <see cref="ItemRef(int)"/> instead.</remarks>
        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => data[ourStart + index];
        }

        /// <summary>
        /// Gets the element at the given index.
        /// </summary>
        /// <param name="index">The position to read from, relative to the start or end of this object's view of
        /// the underlying array.</param>
        /// <returns>The element.</returns>
        /// <remarks>This operation is O(1) for time and memory. For value types, and especially large value 
        /// types, you can avoid a copy operation by using <see cref="ItemRef(Index)"/> instead.</remarks>
        public T this[Index index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => data[ourStart + index.GetOffset(ourLength)];
        }

        /// <summary>
        /// Gets the element at the given index by reference.
        /// </summary>
        /// <param name="index">The position to read from, relative to the start of this object's view of
        /// the underlying array.</param>
        /// <returns>The element.</returns>
        /// <remarks>This operation is O(1) for time and consumes no memory.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly T ItemRef(int index)
            => ref data[ourStart + index];

        /// <summary>
        /// Gets the element at the given index by reference.
        /// </summary>
        /// <param name="index">The position to read from, relative to the start or end of this object's view of
        /// the underlying array.</param>
        /// <returns>The element.</returns>
        /// <remarks>This operation is O(1) for time and consumes no memory.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly T ItemRef(Index index)
            => ref data[ourStart + index.GetOffset(ourLength)];

        /// <summary>
        /// Gets a new <see cref="ImmutableArraySegment{T}"/> representing a portion of this instance's view
        /// into the backing array.
        /// </summary>
        /// <param name="range">A range defining the portion of the current view that the new view should
        /// have.</param>
        /// <returns>A new <see cref="ImmutableArraySegment{T}"/>.</returns>
        /// <remarks>This operation is O(1) for time and memory.</remarks>
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ValidateNotNull(object? source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ValidateOffsetAndLength(int sourceLength, int offset, int length)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "The requested offset is negative.");
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "The requested length is negative.");
            if (offset + length > sourceLength)
            {
                if (offset > sourceLength)
                    throw new ArgumentException("The offset is greater than the length of the source.", nameof(offset));
                else
                    throw new ArgumentException("The requested range would go past end of the source.");
            }
        }
    }
}
