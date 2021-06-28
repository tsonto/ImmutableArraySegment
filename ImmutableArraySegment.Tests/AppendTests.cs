using FluentAssertions;
using System;
using Tsonto.Collections.Generic;
using Xunit;

namespace Tests
{
	public class AppendTests
	{
		[Fact]
		public void Append_Enumerable_AppendsAndRespectsOffsetAndLength()
		{
			var uut = new ImmutableArraySegment<char>(new[] { 'x', 'a', 'b', 'c', 'x' }, 1, 3);
			var annex = new StrictEnumerable<char>(new[] { 'd', 'e' });
			uut.Append(annex).ToArray().Should().BeEquivalentTo('a', 'b', 'c', 'd', 'e');
		}

		[Fact]
		public void Append_Memory_AppendsAndRespectsOffsetAndLength()
		{
			var uut = new ImmutableArraySegment<char>(new[] { 'x', 'a', 'b', 'c', 'x' }, 1, 3);
			var annex = new Memory<char>(new[] { 'd', 'e' });
			uut.Append(in annex).ToArray().Should().BeEquivalentTo('a', 'b', 'c', 'd', 'e');
		}

		[Fact]
		public void Append_ReadOnlyMemory_AppendsAndRespectsOffsetAndLength()
		{
			var uut = new ImmutableArraySegment<char>(new[] { 'x', 'a', 'b', 'c', 'x' }, 1, 3);
			var annex = new ReadOnlyMemory<char>(new[] { 'd', 'e' });
			uut.Append(in annex).ToArray().Should().BeEquivalentTo('a', 'b', 'c', 'd', 'e');
		}

		[Fact]
		public void Append_Span_AppendsAndRespectsOffsetAndLength()
		{
			var uut = new ImmutableArraySegment<char>(new[] { 'x', 'a', 'b', 'c', 'x' }, 1, 3);
			var annex = new Span<char>(new[] { 'd', 'e' });
			uut.Append(in annex).ToArray().Should().BeEquivalentTo('a', 'b', 'c', 'd', 'e');
		}

		[Fact]
		public void Append_ReadOnlySpan_AppendsAndRespectsOffsetAndLength()
		{
			var uut = new ImmutableArraySegment<char>(new[] { 'x', 'a', 'b', 'c', 'x' }, 1, 3);
			var annex = new ReadOnlySpan<char>(new[] { 'd', 'e' });
			uut.Append(in annex).ToArray().Should().BeEquivalentTo('a', 'b', 'c', 'd', 'e');
		}

		[Fact]
		public void Append_Value_AppendsAndRespectsOffsetAndLength()
		{
			var uut = new ImmutableArraySegment<char>(new[] { 'x', 'a', 'b', 'c', 'x' }, 1, 3);
			uut.Append('d').ToArray().Should().BeEquivalentTo('a', 'b', 'c', 'd');
		}
	}
}