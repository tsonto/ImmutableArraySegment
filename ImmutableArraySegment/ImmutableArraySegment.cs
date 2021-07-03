using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Tsonto.Collections.Generic
{
    /// <summary>
    /// Provides extension methods and other static methods that work with <see cref="ImmutableArraySegment{T}"/>.
    /// </summary>
    public static class ImmutableArraySegment
    {
        /// <summary>
        /// Converts an <see cref=" ImmutableArraySegment{T}"/> of a more-derived element type to one of a less-derived
        /// element type.
        /// </summary>
        /// <typeparam name="TDerived">
        /// The element type of the original. This must be a subclass of <typeparamref name="TBase"/>.
        /// </typeparam>
        /// <typeparam name="TBase">The element type of the output.</typeparam>
        /// <param name="source">The input.</param>
        /// <returns></returns>
        /// <remarks>This operation has O(1) time and memory complexity.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ImmutableArraySegment<TBase> As<TDerived, TBase>(this ImmutableArraySegment<TDerived> source)
            where TDerived : class, TBase
            => new(source.data, source.ourStart, source.ourLength, raw: true);

        /// <summary>
        /// Combines the inputs sequentially to produce a <see cref="ImmutableArraySegment{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type parameter for the <see cref="ImmutableArraySegment"/> class.</typeparam>
        /// <param name="sources">The data to combine.</param>
        /// <returns>An <see cref="ImmutableArraySegment{T}"/> holding the combined data from the inputs.</returns>
        /// <remarks>
        /// This operation has O(n) memory usage and O(s+n) time complexity, where n is the total length of all inputs
        /// and s is the number of inputs. Some special cases are O(1).
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static ImmutableArraySegment<T> Concat<T>(IReadOnlyList<T>[] sources)
        {
            if (sources.Length == 0)
                return default;
            if (sources.Length == 1)
                return new(sources[0]);

            var combined = new T[sources.Sum(source => source.Count)];

            int position = 0;
            foreach (var source in sources)
            {
                // Copy the source's data to the destination array.
                if (source is T[] array)
                {
                    Array.Copy(array, 0, combined, position, array.Length);
                    position += array.Length;
                }
                else if (source is ImmutableArraySegment<T> ias)
                {
                    ias.CopyTo(combined, position);
                    position += ias.Length;
                }
                else
                {
                    for (int i = 0; i < source.Count; ++i)
                        combined[position++] = source[i];
                }
            }

            return new(combined, raw: true);
        }

        /// <summary>
        /// Gets an empty instance of <see cref="ImmutableArraySegment{T}"/>.
        /// </summary>
        /// <remarks>
        /// When performance is more important than readability, just use
        /// <c>default(IImutableArraySegment&lt;Whatever&gt;)</c> instead, to avoid a copy.
        /// </remarks>
        /// <typeparam name="T">The type parameter for the <see cref="ImmutableArraySegment"/> class.</typeparam>
        /// <returns>An empty result.</returns>
        public static ImmutableArraySegment<T> Empty<T>() => default;

        /// <summary>
        /// Combines the inputs sequentially with a given delimiter between each source's content.
        /// </summary>
        /// <typeparam name="T">The type parameter for the <see cref="ImmutableArraySegment"/> class.</typeparam>
        /// <param name="delimiter">An element to place between each input source's content.</param>
        /// <param name="sources">The data to combine.</param>
        /// <returns>An <see cref="ImmutableArraySegment{T}"/> holding the combined data from the inputs.</returns>
        /// <remarks>
        /// This operation has O(n) memory usage and O(s+n) time complexity, where n is the total length of all inputs
        /// and s is the number of inputs. Some special cases are O(1).
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static ImmutableArraySegment<T> Join<T>(T delimiter, IReadOnlyList<T>[] sources) 
            => Join(new[] { delimiter }, sources);

        /// <summary>
        /// Combines the inputs sequentially with a given delimiter between each source's content.
        /// </summary>
        /// <typeparam name="T">The type parameter for the <see cref="ImmutableArraySegment"/> class.</typeparam>
        /// <param name="delimiter">A sequence of elements to place between each input source's content.</param>
        /// <param name="sources">The data to combine.</param>
        /// <returns>An <see cref="ImmutableArraySegment{T}"/> holding the combined data from the inputs.</returns>
        /// <remarks>
        /// This operation has O(n + s*d) time and memory complexity, where n is the total length of all source inputs,
        /// s is the number of source inputs, and d is the length of the delimiter. Some special cases are O(1).
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static ImmutableArraySegment<T> Join<T>(IReadOnlyList<T> delimiter, IReadOnlyList<T>[] sources)
        {
            if (sources.Length == 0)
                return default;
            if (sources.Length == 1)
                return new(sources[0]);
            if (delimiter.Count == 0)
                return Concat(sources);

            var newSources = new IReadOnlyList<T>[2 * sources.Length - 1];
            newSources[0] = sources[0];
            for (int i = 1; i < sources.Length; ++i)
            {
                newSources[i * 2 - 1] = delimiter;
                newSources[i * 2] = sources[i];
            }

            return Concat(newSources);
        }

        /// <summary>
        /// Creates an <see cref="ImmutableArraySegment{T}"/> from the given sequence.
        /// </summary>
        /// <typeparam name="T">The sequence's element type.</typeparam>
        /// <param name="source">The sequence that provides the segment's elements.</param>
        /// <returns>A new <see cref="ImmutableArraySegment"/> representing the same data.</returns>
        /// <remarks>
        /// This operation's performance varies by source, amd is the same as that of <see
        /// cref="ImmutableArraySegment{T}.ImmutableArraySegment(IEnumerable{T})"/>. See that constructor's performance
        /// documentation for details.
        /// </remarks>
        public static ImmutableArraySegment<T> ToImmutableArraySegment<T>(this IEnumerable<T> source)
            => new(source);
    }
}
