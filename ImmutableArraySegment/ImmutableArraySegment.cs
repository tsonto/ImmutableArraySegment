using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Tsonto.Collections.Generic
{
    /// <summary>
    /// Provides extension methods and other static methods that work with <see cref="ImmutableArraySegment{T}"/>.
    /// </summary>
    public static class ImmutableArraySegment
    {
        /// <summary>
        /// Gets an empty instance of <see cref="ImmutableArraySegment{T}"/>.
        /// </summary>
        /// <remarks>When performance is more important than readability, just use
        /// <c>default(IImutableArraySegment&lt;Whatever&gt;)</c> instead, to avoid a copy.</remarks>
        /// <typeparam name="T">The type parameter for the <see cref="ImmutableArraySegment"/> class.</typeparam>
        /// <returns>An empty result.</returns>
        public static ImmutableArraySegment<T> Empty<T>() => default;

        /// <summary>
        /// Converts an <see cref=" ImmutableArraySegment{T}"/> of a more-derived element type to one of a less-derived
        /// element type.
        /// </summary>
        /// <typeparam name="TDerived">The element type of the original. This must be a subclass of<typeparamref name="TBase"/>.</typeparam>
        /// <typeparam name="TBase">The element type of the output.</typeparam>
        /// <param name="source">The input.</param>
        /// <returns></returns>
        /// <remarks>This operation has O(1) time and memory complexity.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ImmutableArraySegment<TBase> As<TDerived, TBase>(this ImmutableArraySegment<TDerived> source)
            where TDerived : class, TBase
            => new(source.data, source.ourStart, source.ourLength, raw: true);

        /// <summary>
        /// Creates an <see cref="ImmutableArraySegment{T}"/> from the given sequence.
        /// </summary>
        /// <typeparam name="T">The sequence's element type.</typeparam>
        /// <param name="source">The sequence that provides the segment's elements.</param>
        /// <returns>A new <see cref="ImmutableArraySegment"/> representing the same data.</returns>
        /// <remarks>This operation's performance varies by source, amd is the same as that of
        /// <see cref="ImmutableArraySegment{T}.ImmutableArraySegment(IEnumerable{T})"/>. See that constructor's performance
        /// documentation for details.</remarks>
        public static ImmutableArraySegment<T> ToImmutableArraySegment<T>(this IEnumerable<T> source)
            => new(source);
    }
}
