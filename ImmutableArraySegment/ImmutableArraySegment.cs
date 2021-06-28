using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Tsonto.Collections.Generic
{
	public static class ImmutableArraySegment
	{
		public static ImmutableArraySegment<T> Empty<T>() => default;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ImmutableArraySegment<TBase> As<TDerived, TBase>(this ImmutableArraySegment<TDerived> source)
			where TDerived : class, TBase
			=> new(source.data, source.ourStart, source.ourLength, raw: true);

		public static ImmutableArraySegment<T> ToImmutableArraySegment<T>(this IEnumerable<T> source)
			=> new(source);
	}
}