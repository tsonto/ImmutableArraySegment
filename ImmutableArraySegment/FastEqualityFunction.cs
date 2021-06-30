namespace Tsonto.Collections.Generic
{
    /// <summary>
    /// A delegate type for defining an equality comparison.
    /// </summary>
    /// <remarks>
    /// This is an alternative to <see cref="System.Collections.Generic.IEqualityComparer{T}"/> that minimizes
    /// copies. This can greatly improve performance for value types.
    /// </remarks>
    /// <typeparam name="T">The type of thing to compare.</typeparam>
    /// <param name="a">One of the elements.</param>
    /// <param name="b">The other element.</param>
    /// <returns>True if the inputs are equal according to the implementation; false otherwise.</returns>
    public delegate bool FastEqualityFunction<T>(in T a, in T b);
}
