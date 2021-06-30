using System.Collections.Immutable;
using FluentAssertions;
using Tsonto.Collections.Generic;
using Xunit;

namespace Tests
{
    public class PropertyTests
	{
		[Fact]
		public void Length_GivesSegmentLength()
		{
			var original = new[] { 'a', 'b', 'c', 'd', 'e' };
			var uut = new ImmutableArraySegment<char>(original, 1, 2, raw: true);
			uut.Length.Should().Be(2);
		}

		[Fact]
		public void Count_GivesSegmentLength()
		{
			var original = new[] { 'a', 'b', 'c', 'd', 'e' };
			var uut = new ImmutableArraySegment<char>(original, 1, 2, raw: true);
			var wrap = (IImmutableList<char>)uut;
			wrap.Count.Should().Be(2);
		}

		[Fact]
		public void IntIndexer_RespectsOffset()
		{
			var original = new[] { 'a', 'b', 'c', 'd', 'e' };
			var uut = new ImmutableArraySegment<char>(original, 1, 2, raw: true);
			uut[2].Should().Be('d');
		}

		[Fact]
		public void IndexIndexer_RespectsOffsetAndLength()
		{
			var original = new[] { 'a', 'b', 'c', 'd', 'e' };
			var uut = new ImmutableArraySegment<char>(original, 1, 3, raw: true);
			uut[^1].Should().Be('d');
		}

		[Fact]
		public void RangeIndexer_RespectsOffsetAndLength()
		{
			var original = new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
			var uut = new ImmutableArraySegment<char>(original, 1, 5, raw: true);
			uut[1..^2].ToArray().Should().BeEquivalentTo('c', 'd');
		}
	}
}
