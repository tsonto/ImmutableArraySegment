using System;
using FluentAssertions;
using Tsonto.Collections.Generic;
using Xunit;

namespace Tests
{
    public class AllocateAndCopyTests
    {
        private const char C0 = '\0';

        [Fact]
		public void AllocateAndCopyArray()
		{
			var input = new[] { 'a', 'b', 'c' };
			ImmutableArraySegment<char>.AllocateAndCopyArray(2, 3, input)
				.Should().BeEquivalentTo(C0, C0, 'a', 'b', 'c', C0);
		}

		[Fact]
		public void AllocateAndCopyArraySegment()
		{
			var input = new ArraySegment<char>(new[] { 'x', 'x', 'x', 'a', 'b', 'c', 'x', 'x' }, 3, 3);
			ImmutableArraySegment<char>.AllocateAndCopyArraySegment(2, 3, input)
				.Should().BeEquivalentTo(C0, C0, 'a', 'b', 'c', C0);
		}

		[Fact]
		public void AllocateAndCopyCollection()
		{
			var input = new StrictCollection<char>(new[] { 'a', 'b', 'c' });
			ImmutableArraySegment<char>.AllocateAndCopyCollection(2, 3, input)
				.Should().BeEquivalentTo(C0, C0, 'a', 'b', 'c', C0);
		}

		[Fact]
		public void AllocateAndCopyImmutableArraySegment()
		{
			var input = new ImmutableArraySegment<char>(new[] { 'x', 'x', 'x', 'a', 'b', 'c', 'x', 'x' }, 3, 3);
			ImmutableArraySegment<char>.AllocateAndCopyImmutableArraySegment(2, 3, input)
				.Should().BeEquivalentTo(C0, C0, 'a', 'b', 'c', C0);
		}

		[Fact]
		public void AllocateAndCopyMemoryStruct()
		{
			var input = new Memory<char>(new[] { 'a', 'b', 'c' });
			ImmutableArraySegment<char>.AllocateAndCopyMemoryStruct(2, 3, input)
				.Should().BeEquivalentTo(C0, C0, 'a', 'b', 'c', C0);
		}

		[Fact]
		public void AllocateAndCopyReadOnlyMemoryStruct()
		{
			var input = new ReadOnlyMemory<char>(new[] { 'a', 'b', 'c' });
			ImmutableArraySegment<char>.AllocateAndCopyReadOnlyMemoryStruct(2, 3, input)
				.Should().BeEquivalentTo(C0, C0, 'a', 'b', 'c', C0);
		}

		[Fact]
		public void AllocateAndCopySpan()
		{
			var input = new Span<char>(new[] { 'a', 'b', 'c' });
			ImmutableArraySegment<char>.AllocateAndCopySpan(2, 3, input)
				.Should().BeEquivalentTo(C0, C0, 'a', 'b', 'c', C0);
		}

		[Fact]
		public void AllocateAndCopyReadOnlySpan()
		{
			var input = new ReadOnlySpan<char>(new[] { 'a', 'b', 'c' });
			ImmutableArraySegment<char>.AllocateAndCopyReadOnlySpan(2, 3, input)
				.Should().BeEquivalentTo(C0, C0, 'a', 'b', 'c', C0);
		}

		[Fact]
		public void AllocateAndCopyOtherEnumerable()
		{
			var input = new StrictEnumerable<char>(new[] { 'a', 'b', 'c' });
			ImmutableArraySegment<char>.AllocateAndCopyOtherEnumerable(2, 3, input)
				.Should().BeEquivalentTo(C0, C0, 'a', 'b', 'c', C0);
		}
	}
}
