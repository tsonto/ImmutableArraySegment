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
    /// Objects of this type are immutable, and they guarantee that the array they point to is immutable. However (like
    /// other .NET array types), if the type <typeparamref name="T"/> is not immutable then the elements themselves may
    /// change. Example: since <c>SortedSet&lt;int&gt;</c> is mutable, for an
    /// <c>ImmutableArraySegment&lt;SortedSet&lt;int&gt;&gt; s</c>, <c>s[0]</c> will always refer to the same set
    /// object, but the contents of that object may vary over time.
    /// </para>
    /// <para>Comparison with built-int types:
    /// <list type="bullet">
    /// <item>
    /// This type is, in many ways, a drop-in replacement for <see cref="ImmutableArray{T}"/>. ImmutableArraySegment's
    /// advantages are that it provides more functionality and does much of it (such as getting array slices) faster.
    /// </item>
    /// <item>
    /// This type can be seen as an immutable version of <see cref="ArraySegment{T}"/>, although ImmutableArraySegment
    /// provides more features.
    /// </item>
    /// <item></item>
    /// It's also somewhat similar to <see cref="ReadOnlyMemory{T}"/>, but only works with managed memory. (And, of
    /// course, ReadOnlyMemory doesn't actually guarantee immutability, since the data it wraps could be changed by
    /// other code.)
    /// </list>
    /// </para>
    /// <para>
    /// This package's developers have taken care to optimize this type for performance, both in terms of speed and
    /// memory usage. Additionally, every method's documentation provides information about its performance
    /// characteristics.
    /// </para>
    /// </remarks>
    public readonly struct ImmutableArraySegment<T> : IImmutableList<T>, IEnumerable<T>, IReadOnlyList<T>, IReadOnlyCollection<T>
    {
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
        /// <param name="range">The portion of the source to copy.</param>
        /// <remarks>This operation is O(r) for speed and memory, where r is the length of the range.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The requested range is invalid, or is invalid for the length of the source.
        /// </exception>
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
        /// <item><c>ImmutableArraySegment&lt; <typeparamref name="T"/>&gt;</c>: O(1)</item>
        /// <item>
        /// <c>ArraySegment&lt; <typeparamref name="T"/>&gt;</c>: O(n), where n is the length of the segment rather than
        /// of its backing array
        /// </item>
        /// <item><c>ICollection&lt; <typeparamref name="T"/>&gt;</c>: O(n)</item>
        /// <item><c>IReadOnlyList&lt; <typeparamref name="T"/>&gt;</c>: O(n)</item>
        /// <item>
        /// otherwise: same as ToArray, which is approximately O(n*log(n)); iterates through the sequence only once
        /// </item>
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
        /// implements. n refers to the full length of the source, r to the length of the range, and s to the start
        /// index of the range:
        /// <list type="bullet">
        /// <item><c><typeparamref name="T"/>[]</c>: O(r)</item>
        /// <item><c>ImmutableArraySegment&lt; <typeparamref name="T"/>&gt;</c>: O(1)</item>
        /// <item><c>ArraySegment&lt; <typeparamref name="T"/>&gt;</c>: O(r)</item>
        /// <item><c>ICollection&lt; <typeparamref name="T"/>&gt; with n == r</c>: O(r)</item>
        /// <item><c>IReadOnlyList&lt; <typeparamref name="T"/>&gt;</c>: O(n)</item>
        /// <item>
        /// <c>ICollection&lt; <typeparamref name="T"/>&gt; with n != r</c>: O(s) + O(r*log(r)); sequence is only
        /// enunmerated once, and not past s+r
        /// </item>
        /// <item>
        /// otherwise, if r is implicit in how the range was given: O(s) + O(r*log(r)); sequence is only enumerated
        /// once, and not past s+r
        /// </item>
        /// <item>otherwise, O(n) + O(s) + O(r*log(r)); sequence is fully enumerated and then partially enumerated</item>
        /// </list>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The requested range is invalid, or is invalid for the length of the source.
        /// </exception>
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
        /// <remarks>
        /// This operation is O(r) for speed and memory, where r is the provided length rather than the length of the
        /// source array.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The requested range is invalid, or is invalid for the length of the source.
        /// </exception>
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
        /// <remarks>
        /// When performance is more important than readability, just use
        /// <c>default(IImutableArraySegment&lt;Whatever&gt;)</c> instead, to avoid a copy.
        /// </remarks>
        /// <remarks>This operation is O(1) for time and memory.</remarks>
        public static ImmutableArraySegment<T> Empty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => default;
        }

        /// <inheritdoc/>
        int IReadOnlyCollection<T>.Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ourLength;
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

        /// <summary>
        /// Gets the element at the given index.
        /// </summary>
        /// <param name="index">
        /// The position to read from, relative to the start of this object's view of the underlying array.
        /// </param>
        /// <returns>The element.</returns>
        /// <remarks>
        /// This operation is O(1) for time and memory. For value types, and especially large value types, you can avoid
        /// a copy operation by using <see cref="ItemRef(int)"/> instead.
        /// </remarks>
        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => data[ourStart + index];
        }

        /// <summary>
        /// Gets the element at the given index.
        /// </summary>
        /// <param name="index">
        /// The position to read from, relative to the start or end of this object's view of the underlying array.
        /// </param>
        /// <returns>The element.</returns>
        /// <remarks>
        /// This operation is O(1) for time and memory. For value types, and especially large value types, you can avoid
        /// a copy operation by using <see cref="ItemRef(Index)"/> instead.
        /// </remarks>
        /// <exception cref="IndexOutOfRangeException">
        /// The specified position is beyond the array segment's bounds.
        /// </exception>
        public T this[Index index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => data[ourStart + index.GetOffset(ourLength)];
        }

        /// <summary>
        /// Gets a new <see cref="ImmutableArraySegment{T}"/> representing a portion of this instance's view into the
        /// backing array.
        /// </summary>
        /// <param name="range">A range defining the portion of the current view that the new view should have.</param>
        /// <returns>A new <see cref="ImmutableArraySegment{T}"/>.</returns>
        /// <remarks>This operation is O(1) for time and memory.</remarks>
        /// <exception cref="IndexOutOfRangeException">
        /// The start or end of the range is beyond the array segment's bounds.
        /// </exception>
        public ImmutableArraySegment<T> this[Range range]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                (int newOffset, int newLength) = range.GetOffsetAndLength(ourLength);
                return Slice(newOffset, newLength);
            }
        }

        internal int ourEnd
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ourStart + ourLength;
        }

        internal readonly T[] data;
        internal readonly int ourLength;
        internal readonly int ourStart;

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IImmutableList<T> IImmutableList<T>.Add(T value)
            => Append(value);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IImmutableList<T> IImmutableList<T>.AddRange(IEnumerable<T> items)
            => Append(items);

        /// <summary>
        /// Creates a new <see cref="ImmutableArraySegment{T}"/> that's the same as this one except with a specified
        /// element appended.
        /// </summary>
        /// <param name="value">The new element.</param>
        /// <returns>A new <see cref="ImmutableArraySegment{T}"/>.</returns>
        /// <remarks>This operation is O(n) for time and memory, where n is this segment's length.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableArraySegment<T> Append(T value)
        {
            var newArray = new T[ourLength + 1];
            Array.Copy(data, ourStart, newArray, 0, ourLength);
            newArray[ourLength] = value;
            return new(newArray, raw: true);
        }

        /// <summary>
        /// Creates a new <see cref="ImmutableArraySegment{T}"/> that's the same as this one except with a specified
        /// elements appended.
        /// </summary>
        /// <param name="items">The new elements.</param>
        /// <returns>A new <see cref="ImmutableArraySegment{T}"/>.</returns>
        /// <remarks>
        /// This operation's time and memory characteristics are O(n), where n is the combined length of the output. The
        /// copy operation is done efficiently if the actual type of the <paramref name="items"/> input is <c>T[]</c>,
        /// <c>ImmutableArraySegment&lt;T&gt;</c>, or <c>ArraySegment&lt;T&gt;</c>, or implements
        /// <c>IReadOnlyList&lt;T&gt;</c> or <c>ICollection&lt;T&gt;</c>. Otherwise, the copy is somewhat less
        /// efficient, and iterates over the enumerable twice.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// The length of the enumerated sequence differs when run the first time vs. the second time.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableArraySegment<T> Append(IEnumerable<T> items)
        {
            T[] newArray = AllocateAndCopy(items, ourLength, ourLength);
            Array.Copy(data, ourStart, newArray, 0, ourLength);
            return new(newArray, true);
        }

        /// <summary>
        /// Creates a new <see cref="ImmutableArraySegment{T}"/> that's the same as this one except with a specified
        /// elements appended.
        /// </summary>
        /// <param name="items">The new elements.</param>
        /// <returns>A new <see cref="ImmutableArraySegment{T}"/>.</returns>
        /// <remarks>This operation is O(n) for time and memory, where n is the combined length.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableArraySegment<T> Append(in ReadOnlyMemory<T> items)
        {
            T[] newArray = AllocateAndCopyReadOnlyMemoryStruct(ourLength, ourLength, in items);
            Array.Copy(data, ourStart, newArray, 0, ourLength);
            return new(newArray, true);
        }

        /// <summary>
        /// Creates a new <see cref="ImmutableArraySegment{T}"/> that's the same as this one except with a specified
        /// elements appended.
        /// </summary>
        /// <param name="items">The new elements.</param>
        /// <returns>A new <see cref="ImmutableArraySegment{T}"/>.</returns>
        /// <remarks>This operation is O(n) for time and memory, where n is the combined length.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableArraySegment<T> Append(in Memory<T> items)
        {
            T[] newArray = AllocateAndCopyMemoryStruct(ourLength, ourLength, in items);
            Array.Copy(data, ourStart, newArray, 0, ourLength);
            return new(newArray, true);
        }

        /// <summary>
        /// Creates a new <see cref="ImmutableArraySegment{T}"/> that's the same as this one except with a specified
        /// elements appended.
        /// </summary>
        /// <param name="items">The new elements.</param>
        /// <returns>A new <see cref="ImmutableArraySegment{T}"/>.</returns>
        /// <remarks>This operation is O(n) for time and memory, where n is the combined length.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableArraySegment<T> Append(in ReadOnlySpan<T> items)
        {
            T[] newArray = AllocateAndCopyReadOnlySpan(ourLength, ourLength, in items);
            Array.Copy(data, ourStart, newArray, 0, ourLength);
            return new(newArray, true);
        }

        /// <summary>
        /// Creates a new <see cref="ImmutableArraySegment{T}"/> that's the same as this one except with a specified
        /// elements appended.
        /// </summary>
        /// <param name="items">The new elements.</param>
        /// <returns>A new <see cref="ImmutableArraySegment{T}"/>.</returns>
        /// <remarks>This operation is O(n) for time and memory, where n is the combined length.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableArraySegment<T> Append(in Span<T> items)
        {
            T[] newArray = AllocateAndCopySpan(ourLength, ourLength, in items);
            Array.Copy(data, ourStart, newArray, 0, ourLength);
            return new(newArray, true);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IImmutableList<T> IImmutableList<T>.Clear()
            => Empty;

        /// <summary>
        /// Copies the array segment's elements to the given array.
        /// </summary>
        /// <param name="dest">The array to copy to.</param>
        /// <param name="destOffset">The position in the destination array to start the copy at.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The given offset and length exceed the bounds of the source and/or the destination.
        /// </exception>
        /// <remarks>This operation is O(n) for time and memory, where n is length copied.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void CopyTo(T[] dest, int destOffset, int length)
        {
            if (destOffset < 0 || destOffset > dest.Length)
                throw new ArgumentOutOfRangeException(nameof(destOffset));
            if (length > ourLength | destOffset + length > dest.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            Array.Copy(data, ourStart, dest, destOffset, length);
        }

        /// <summary>
        /// Copies the array segment's elements to the given array.
        /// </summary>
        /// <param name="dest">The array to copy to.</param>
        /// <param name="destOffset">The position in the destination array to start the copy at.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The given offset and the source's length exceed the bounds of the source and/or the destination.
        /// </exception>
        /// <remarks>This operation is O(n) for time and memory, where n is source's length.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(T[] dest, int destOffset)
            => CopyTo(dest, destOffset, ourLength);

        /// <summary>
        /// Copies the array segment's elements to the given array.
        /// </summary>
        /// <param name="dest">The array to copy to.</param>
        /// <param name="destOffset">The position in the destination array to start the copy at.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The given offset and length exceed the bounds of the source and/or the destination.
        /// </exception>
        /// <remarks>This operation is O(n) for time and memory, where n is length copied.</remarks>
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

        /// <summary>
        /// Copies the array segment's elements to the given array.
        /// </summary>
        /// <param name="dest">The array to copy to.</param>
        /// <param name="destOffset">The position in the destination array to start the copy at.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The given offset and the source's length exceed the bounds of the source and/or the destination.
        /// </exception>
        /// <remarks>This operation is O(n) for time and memory, where n the source's length.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(in Span<T> dest, int destOffset)
            => CopyTo(in dest, destOffset, ourLength);

        /// <summary>
        /// Copies the array segment's elements to the given array.
        /// </summary>
        /// <param name="dest">The array to copy to.</param>
        /// <param name="destOffset">The position in the destination array to start the copy at.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The given offset and length exceed the bounds of the source and/or the destination.
        /// </exception>
        /// <remarks>This operation is O(n) for time and memory, where n is length copied.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void CopyTo(in Memory<T> dest, int destOffset, int length)
            => CopyTo(dest.Span, destOffset, length);

        /// <summary>
        /// Copies the array segment's elements to the given array.
        /// </summary>
        /// <param name="dest">The array to copy to.</param>
        /// <param name="destOffset">The position in the destination array to start the copy at.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The given offset and the source's length exceed the bounds of the source and/or the destination.
        /// </exception>
        /// <remarks>This operation is O(n) for time and memory, where n is the source's length.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(in Memory<T> dest, int destOffset)
            => CopyTo(in dest, destOffset, ourLength);

        /// <summary>
        /// Gets an enumerator for iterating over the segment's elements.
        /// </summary>
        /// <returns>An enumerator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        /// <summary>
        /// Finds the position of the first occurence of the given element in the array segment.
        /// </summary>
        /// <param name="item">The element to search for.</param>
        /// <returns>
        /// The number of elements from the beginning of the segment that the element was found at, or -1 if the element
        /// was not found.
        /// </returns>
        /// <remarks>This operation is O(n) for time and O(1) for memory, where n is the source's length.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item)
            => IndexOf(item, 0, ourLength, (IEqualityComparer<T>?)null);

        /// <summary>
        /// Finds the position of the first occurence of the given element in the array segment.
        /// </summary>
        /// <param name="item">The element to search for.</param>
        /// <param name="index">The position to start the search from. 0 is the start of the array segment.</param>
        /// <returns>
        /// The number of elements from the beginning of the segment that the element was found at, or -1 if the element
        /// was not found.
        /// </returns>
        /// <remarks>This operation is O(n) for time and O(1) for memory, where n is the search length.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(in T item, int index)
            => IndexOf(in item, index, ourLength - index, (IEqualityComparer<T>?)null);

        /// <summary>
        /// Finds the position of the first occurence of the given element in the array segment.
        /// </summary>
        /// <param name="item">The element to search for.</param>
        /// <param name="index">The position to start the search from. 0 is the start of the array segment.</param>
        /// <param name="count">How many elements to search.</param>
        /// <returns>
        /// The number of elements from the beginning of the segment that the element was found at, or -1 if the element
        /// was not found.
        /// </returns>
        /// <remarks>This operation is O(n) for time and O(1) for memory, where n is the search length.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item, int index, int count)
            => IndexOf(item, index, count, (IEqualityComparer<T>?)null);

        /// <summary>
        /// Finds the position of the first occurence of the given element in the array segment.
        /// </summary>
        /// <param name="item">The element to search for.</param>
        /// <param name="equalityComparer">
        /// The equality comparer to use for testing whether a given element is a match.
        /// </param>
        /// <returns>
        /// The number of elements from the beginning of the segment that the element was found at, or -1 if the element
        /// was not found.
        /// </returns>
        /// <remarks>
        /// This operation is O(n) for time and O(1) for memory, where n is the segment's length. If <typeparamref
        /// name="T"/> is a value type, especially a large one, <see cref="IndexOf(in T, FastEqualityFunction{T})"/> may
        /// be one or more orders of magnitude faster.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item, IEqualityComparer<T>? equalityComparer)
            => IndexOf(item, 0, ourLength, equalityComparer);

        /// <summary>
        /// Finds the position of the first occurence of the given element in the array segment.
        /// </summary>
        /// <param name="item">The element to search for.</param>
        /// <param name="index">The position to start the search from. 0 is the start of the array segment.</param>
        /// <param name="equalityComparer">
        /// The equality comparer to use for testing whether a given element is a match.
        /// </param>
        /// <returns>
        /// The number of elements from the beginning of the segment that the element was found at, or -1 if the element
        /// was not found.
        /// </returns>
        /// <remarks>
        /// This operation is O(n) for time and O(1) for memory, where n is the search length. If <typeparamref
        /// name="T"/> is a value type, especially a large one, <see cref="IndexOf(in T, int,
        /// FastEqualityFunction{T})"/> may be one or more orders of magnitude faster.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item, int index, IEqualityComparer<T>? equalityComparer)
            => IndexOf(item, index, ourLength - index, equalityComparer);

        /// <summary>
        /// Finds the position of the first occurence of the given element in the array segment.
        /// </summary>
        /// <param name="item">The element to search for.</param>
        /// <param name="index">The position to start the search from. 0 is the start of the array segment.</param>
        /// <param name="count">How many elements to search.</param>
        /// <param name="equalityComparer">
        /// The equality comparer to use for testing whether a given element is a match.
        /// </param>
        /// <returns>
        /// The number of elements from the beginning of the segment that the element was found at, or -1 if the element
        /// was not found.
        /// </returns>
        /// <remarks>
        /// This operation is O(n) for time and O(1) for memory, where n is the search length. If <typeparamref
        /// name="T"/> is a value type, especially a large one, <see cref="IndexOf(in T, int, int,
        /// FastEqualityFunction{T})"/> may be one or more orders of magnitude faster.
        /// </remarks>
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

        /// <summary>
        /// Finds the position of the first occurence of the given element in the array segment.
        /// </summary>
        /// <param name="item">The element to search for.</param>
        /// <param name="areEqual">A function to determine whether two elements should be considered a match.</param>
        /// <returns>
        /// The number of elements from the beginning of the segment that the element was found at, or -1 if the element
        /// was not found.
        /// </returns>
        /// <remarks>This operation is O(n) for time and O(1) for memory, where n is the segment's length.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int IndexOf(in T item, FastEqualityFunction<T> areEqual)
                    => IndexOf(in item, 0, ourLength, areEqual);

        /// <summary>
        /// Finds the position of the first occurence of the given element in the array segment.
        /// </summary>
        /// <param name="item">The element to search for.</param>
        /// <param name="index">The position to start the search from. 0 is the start of the array segment.</param>
        /// <param name="areEqual">A function to determine whether two elements should be considered a match.</param>
        /// <returns>
        /// The number of elements from the beginning of the segment that the element was found at, or -1 if the element
        /// was not found.
        /// </returns>
        /// <remarks>This operation is O(n) for time and O(1) for memory, where n is the search length.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public int IndexOf(in T item, int index, FastEqualityFunction<T> areEqual)
                    => IndexOf(in item, index, ourLength - index, areEqual);

        /// <summary>
        /// Finds the position of the first occurence of the given element in the array segment.
        /// </summary>
        /// <param name="item">The element to search for.</param>
        /// <param name="index">The position to start the search from. 0 is the start of the array segment.</param>
        /// <param name="count">How many elements to search.</param>
        /// <param name="areEqual">A function to determine whether two elements should be considered a match.</param>
        /// <returns>
        /// The number of elements from the beginning of the segment that the element was found at, or -1 if the element
        /// was not found.
        /// </returns>
        /// <remarks>This operation is O(n) for time and O(1) for memory, where n is the search length.</remarks>
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

        /// <summary>
        /// Produces a new <see cref="ImmutableArraySegment{T}"/> that is like the current one except with content
        /// inserted somewhere within it.
        /// </summary>
        /// <param name="index">The location to insert the content at.</param>
        /// <param name="element">The content to insert.</param>
        /// <returns>The new <see cref="ImmutableArraySegment{T}"/>.</returns>
        /// <remarks>This operation is O(n) for time and memory, where n is the combined length.</remarks>
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

        /// <summary>
        /// Produces a new <see cref="ImmutableArraySegment{T}"/> that is like the current one except with content
        /// inserted somewhere within it.
        /// </summary>
        /// <param name="index">The location to insert the content at.</param>
        /// <param name="items">The content to insert.</param>
        /// <returns>The new <see cref="ImmutableArraySegment{T}"/>.</returns>
        /// <remarks>
        /// This operation's time and memory characteristics are O(n), where n is the combined length of the output. The
        /// copy operation is done efficiently if the actual type of the <paramref name="items"/> input is <c>T[]</c>,
        /// <c>ImmutableArraySegment&lt;T&gt;</c>, or <c>ArraySegment&lt;T&gt;</c>, or implements
        /// <c>IReadOnlyList&lt;T&gt;</c> or <c>ICollection&lt;T&gt;</c>. Otherwise, the copy is somewhat less
        /// efficient, and iterates over the enumerable twice.
        /// </remarks>
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

        /// <summary>
        /// Produces a new <see cref="ImmutableArraySegment{T}"/> that is like the current one except with content
        /// inserted somewhere within it.
        /// </summary>
        /// <param name="index">The location to insert the content at.</param>
        /// <param name="items">The content to insert.</param>
        /// <returns>The new <see cref="ImmutableArraySegment{T}"/>.</returns>
        /// <remarks>
        /// This operation's time and memory characteristics are O(n), where n is the combined length of the output. The
        /// copy operation is done efficiently if the actual type of the <paramref name="items"/> input is <c>T[]</c>,
        /// <c>ImmutableArraySegment&lt;T&gt;</c>, or <c>ArraySegment&lt;T&gt;</c>, or implements
        /// <c>IReadOnlyList&lt;T&gt;</c> or <c>ICollection&lt;T&gt;</c>. Otherwise, the copy is somewhat less
        /// efficient, and iterates over the enumerable twice.
        /// </remarks>
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

        /// <summary>
        /// Produces a new <see cref="ImmutableArraySegment{T}"/> that is like the current one except with content
        /// inserted somewhere within it.
        /// </summary>
        /// <param name="index">The location to insert the content at.</param>
        /// <param name="items">The content to insert.</param>
        /// <returns>The new <see cref="ImmutableArraySegment{T}"/>.</returns>
        /// <remarks>This operation is O(n) for time and memory, where n is the combined length.</remarks>
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

        /// <summary>
        /// Produces a new <see cref="ImmutableArraySegment{T}"/> that is like the current one except with content
        /// inserted somewhere within it.
        /// </summary>
        /// <param name="index">The location to insert the content at.</param>
        /// <param name="items">The content to insert.</param>
        /// <returns>The new <see cref="ImmutableArraySegment{T}"/>.</returns>
        /// <remarks>This operation is O(n) for time and memory, where n is the combined length.</remarks>
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

        /// <summary>
        /// Produces a new <see cref="ImmutableArraySegment{T}"/> that is like the current one except with content
        /// inserted somewhere within it.
        /// </summary>
        /// <param name="index">The location to insert the content at.</param>
        /// <param name="items">The content to insert.</param>
        /// <returns>The new <see cref="ImmutableArraySegment{T}"/>.</returns>
        /// <remarks>This operation is O(n) for time and memory, where n is the combined length.</remarks>
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

        /// <summary>
        /// Produces a new <see cref="ImmutableArraySegment{T}"/> that is like the current one except with content
        /// inserted somewhere within it.
        /// </summary>
        /// <param name="index">The location to insert the content at.</param>
        /// <param name="items">The content to insert.</param>
        /// <returns>The new <see cref="ImmutableArraySegment{T}"/>.</returns>
        /// <remarks>This operation is O(n) for time and memory, where n is the combined length.</remarks>
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

        /// <summary>
        /// Gets the element at the given index by reference.
        /// </summary>
        /// <param name="index">
        /// The position to read from, relative to the start of this object's view of the underlying array.
        /// </param>
        /// <returns>The element.</returns>
        /// <remarks>This operation is O(1) for time and consumes no memory.</remarks>
        /// <exception cref="IndexOutOfRangeException">
        /// The specified position is beyond the array segment's bounds.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly T ItemRef(int index)
            => ref data[ourStart + index];

        /// <summary>
        /// Gets the element at the given index by reference.
        /// </summary>
        /// <param name="index">
        /// The position to read from, relative to the start or end of this object's view of the underlying array.
        /// </param>
        /// <returns>The element.</returns>
        /// <remarks>This operation is O(1) for time and consumes no memory.</remarks>
        /// <exception cref="IndexOutOfRangeException">
        /// The specified position is beyond the array segment's bounds.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly T ItemRef(Index index)
            => ref data[ourStart + index.GetOffset(ourLength)];

        int IImmutableList<T>.LastIndexOf(T item, int index, int count, IEqualityComparer<T>? equalityComparer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="ImmutableArraySegment{T}"/> that's the same as this one except with a specified
        /// element prepended.
        /// </summary>
        /// <param name="value">The new element.</param>
        /// <returns>A new <see cref="ImmutableArraySegment{T}"/>.</returns>
        /// <remarks>This operation is O(n) for time and memory, where n is this segment's length.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableArraySegment<T> Prepend(T value)
        {
            var newArray = new T[ourLength + 1];
            Array.Copy(data, ourStart, newArray, 1, ourLength);
            data[0] = value;
            return new(newArray, raw: true);
        }

        /// <inheritdoc/>
        IImmutableList<T> IImmutableList<T>.Remove(T value, IEqualityComparer<T>? equalityComparer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Produces a new <see cref="ImmutableArraySegment{T}"/> that's like the current one except with certain
        /// elements removed.
        /// </summary>
        /// <param name="match">
        /// A condition to apply to the source elements. If the condition evaluates to true, the element will be omitted
        /// from the output.
        /// </param>
        /// <returns>The new <see cref="ImmutableArraySegment{T}"/>.</returns>
        /// <remarks>
        /// This operation is O(n) for time and memory, where n is this segment's length. Time complexity is modified by
        /// the performance of the predicate. If <typeparamref name="T"/> is a value type, this method will involve
        /// numerous copy operations, which may be detrimental to performance.
        /// </remarks>
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

        /// <summary>
        /// Determines whether the segment's elements match the elements of a given sequence, using the default equality
        /// comparer.
        /// </summary>
        /// <param name="other">The sequence to compare with.</param>
        /// <returns>True if the sequences are equal; false otherwise.</returns>
        /// <remarks>
        /// This method has O(n) time complexity and O(1) memory complexity, where n is the length of the shorter
        /// sequence. If the input implements either <c>IReadOnlyCollection&lt;T&gt;</c> or <c>ICollection</c>, the
        /// method can use an O(1) shortcut if the sequence lengths differ. The enumeration is only iterated over once.
        /// If <typeparamref name="T"/> is a value type, this method will involve many copy operations, which may be
        /// detrimental to performance.
        /// </remarks>
        public bool SequenceEquals(IEnumerable<T> other)
             => SequenceEquals(other, EqualityComparer<T>.Default);

        /// <summary>
        /// Determines whether the segment's elements match the elements of a given sequence, using the given equality
        /// comparer.
        /// </summary>
        /// <param name="other">The sequence to compare with.</param>
        /// <param name="equalityComparer">
        /// The equality comparer to use for determining whether two elements are equal.
        /// </param>
        /// <returns>True if the sequences are equal; false otherwise.</returns>
        /// <remarks>
        /// This method has O(n) time complexity and O(1) memory complexity, where n is the length of the shorter
        /// sequence. If the input implements either <c>IReadOnlyCollection&lt;T&gt;</c> or <c>ICollection</c>, the
        /// method can use an O(1) shortcut if the sequence lengths differ. The enumeration is only iterated over once.
        /// If <typeparamref name="T"/> is a value type, this method will involve many copy operations, which may be
        /// detrimental to performance.
        /// </remarks>
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

        /// <summary>
        /// Determines whether the segment's elements match the elements of a given sequence, using the default equality
        /// comparer.
        /// </summary>
        /// <param name="other">The sequence to compare with.</param>
        /// <returns>True if the sequences are equal; false otherwise.</returns>
        /// <remarks>
        /// This method has O(n) time complexity and O(1) memory complexity, where n is the length of the shorter
        /// sequence. The method uses an O(1) shortcut if the sequence lengths differ. If <typeparamref name="T"/> is a
        /// value type, this method will involve many copy operations, which may be detrimental to performance.
        /// </remarks>
        public bool SequenceEquals(ImmutableArraySegment<T> other)
            => SequenceEquals(other, EqualityComparer<T>.Default);

        /// <summary>
        /// Determines whether the segment's elements match the elements of a given sequence, using the given equality
        /// comparer.
        /// </summary>
        /// <param name="other">The sequence to compare with.</param>
        /// <param name="equalityComparer">
        /// The equality comparer to use for determining whether two elements are equal.
        /// </param>
        /// <returns>True if the sequences are equal; false otherwise.</returns>
        /// <remarks>
        /// This method has O(n) time complexity and O(1) memory complexity, where n is the length of the shorter
        /// sequence. The method uses an O(1) shortcut if the sequence lengths differ. If <typeparamref name="T"/> is a
        /// value type, this method will involve many copy operations, which may be detrimental to performance.
        /// </remarks>
        public bool SequenceEquals(ImmutableArraySegment<T> other, IEqualityComparer<T> equalityComparer)
        {
            if (ourLength != other.ourLength)
                return false;
            for (int i = 0; i < ourLength; ++i)
                if (!equalityComparer.Equals(data[i + ourStart], other.data[i + other.ourStart]))
                    return false;
            return true;
        }

        /// <summary>
        /// Determines whether the segment's elements match the elements of a given sequence, using the given equality
        /// comparer.
        /// </summary>
        /// <param name="other">The sequence to compare with.</param>
        /// <param name="equalityComparer">
        /// The equality comparer to use for determining whether two elements are equal.
        /// </param>
        /// <returns>True if the sequences are equal; false otherwise.</returns>
        /// <remarks>
        /// This method has O(n) time complexity and O(1) memory complexity, where n is the length of the shorter
        /// sequence. The method uses an O(1) shortcut if the sequence lengths differ. If <typeparamref name="T"/> is a
        /// value type, this method will involve many copy operations, which may be detrimental to performance.
        /// </remarks>
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

        /// <inheritdoc/>
        IImmutableList<T> IImmutableList<T>.SetItem(int index, T value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a new <see cref="ImmutableArraySegment{T}"/> representing a portion of this instance's view into the
        /// backing array.
        /// </summary>
        /// <param name="offset">The starting point of the range.</param>
        /// <param name="length">The length of the range.</param>
        /// <returns>A new <see cref="ImmutableArraySegment{T}"/>.</returns>
        /// <remarks>This operation is O(1) for time and memory.</remarks>
        /// <exception cref="IndexOutOfRangeException">
        /// The start or end of the range is beyond the array segment's bounds.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableArraySegment<T> Slice(int offset, int length)
        {
            if (offset < 0 || offset > length || length < 0 || offset + length > ourLength)
                throw new IndexOutOfRangeException();
            return new(data, ourStart + offset, length, true);
        }

        /// <summary>
        /// Gets a new <see cref="ImmutableArraySegment{T}"/> representing a portion of this instance's view into the
        /// backing array.
        /// </summary>
        /// <param name="offset">The starting point of the range.</param>
        /// <returns>A new <see cref="ImmutableArraySegment{T}"/>.</returns>
        /// <remarks>This operation is O(1) for time and memory.</remarks>
        /// <exception cref="IndexOutOfRangeException">
        /// The start of the range is beyond the array segment's bounds.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableArraySegment<T> Slice(int offset)
        {
            if (offset < 0 || offset > ourLength)
                throw new IndexOutOfRangeException();
            return new(data, ourStart + offset, ourLength - offset, true);
        }

        /// <summary>
        /// Copies the elements to an array.
        /// </summary>
        /// <returns>The array.</returns>
        /// <remarks>This operation is O(1) for time and memory.</remarks>
        public T[] ToArray()
        {
            var output = new T[ourLength];
            Array.Copy(data, ourStart, output, 0, ourLength);
            return output;
        }

        /// <inheritdoc/>
        public override string ToString()
            => $"{{ImmutableArraySegment<{typeof(T).Name}>, length {ourLength}}}";

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        internal static T[] AllocateAndCopyArray(int destOffset, int extraDestLength, T[] source)
        {
            var dest = new T[source.Length + extraDestLength];
            Array.Copy(source, 0, dest, destOffset, source.Length);
            return dest;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        internal static T[] AllocateAndCopyArraySegment(int destOffset, int extraDestLength, ArraySegment<T> source)
        {
            var dest = new T[source.Count + extraDestLength];
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
        internal static T[] AllocateAndCopyImmutableArraySegment(int destOffset, int extraDestLength, ImmutableArraySegment<T> source)
        {
            var dest = new T[source.Length + extraDestLength];
            source.CopyTo(dest, destOffset);
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

        private class Enumerator : IEnumerator<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator(ImmutableArraySegment<T> parent)
            {
                position = parent.ourStart - 1;
                this.parent = parent;
            }

            public T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
                get => position >= parent.ourStart && position < parent.ourEnd
                    ? parent.data[position]
                    : throw new InvalidOperationException();
            }

            object IEnumerator.Current => Current!;
            private readonly ImmutableArraySegment<T> parent;
            private int position;

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
    }
}
