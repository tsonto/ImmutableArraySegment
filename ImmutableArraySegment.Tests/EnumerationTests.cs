using FluentAssertions;
using System;
using System.Linq;
using Tsonto.Collections.Generic;
using Xunit;

namespace Tests
{
	public class EnumerationTests
	{
		[Fact]
		public void Enumerable_RespectsOffsetAndLength()
		{
			var uut = new ImmutableArraySegment<char>(new[] { 'a', 'b', 'c', 'd', 'e', 'f' }, 1, 4, raw: true);
			uut.AsEnumerable().ToArray().Should().BeEquivalentTo('b', 'c', 'd', 'e');
		}

		[Fact]
		public void Enumerable_CurrentBeforeMoveNext_Throws()
		{
			var uut = new ImmutableArraySegment<char>(new[] { 'a', 'b', 'c', 'd', 'e', 'f' }, 1, 4, raw: true);
			var e = uut.GetEnumerator();
			Assert.Throws<InvalidOperationException>(() => e.Current);
		}

		[Fact]
		public void Enumerable_CurrentAfterFailedMoveNext_Throws()
		{
			var uut = new ImmutableArraySegment<char>(new[] { 'a', 'b', 'c', 'd', 'e', 'f' }, 1, 4, raw: true);
			var e = uut.GetEnumerator();
			while (e.MoveNext()) { }
			Assert.Throws<InvalidOperationException>(() => e.Current);
		}

		[Fact]
		public void Enumerable_Reset_ResetsAndRespectsOffset()
		{
			var uut = new ImmutableArraySegment<char>(new[] { 'a', 'b', 'c', 'd', 'e', 'f' }, 3, 3, raw: true);
			var e = uut.GetEnumerator();
			while (e.MoveNext()) { }
			e.Reset();
			e.MoveNext();
			e.Current.Should().Be('d');
		}
	}
}