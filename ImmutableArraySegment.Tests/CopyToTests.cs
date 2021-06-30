using System;
using FluentAssertions;
using Tsonto.Collections.Generic;
using Xunit;

namespace Tests
{
    public class CopyToTests
    {
        private const char C0 = '\0';

        [Fact]
        public void CopyTo_Array_RespectsOffsetAndLength()
        {
            var uut = new ImmutableArraySegment<char>(new[] { 'x', 'a', 'b', 'c', 'x' }, 1, 3);
            var dest = new char[6];
            uut.CopyTo(dest, 2);
            dest.Should().BeEquivalentTo(C0, C0, 'a', 'b', 'c', C0);
        }

        [Fact]
        public void CopyTo_Array_WithLength_RespectsOffsetAndLength()
        {
            var uut = new ImmutableArraySegment<char>(new[] { 'x', 'a', 'b', 'c', 'x' }, 1, 3);
            var dest = new char[6];
            uut.CopyTo(dest, 2, 2);
            dest.Should().BeEquivalentTo(C0, C0, 'a', 'b', C0, C0);
        }

        [Fact]
        public void CopyTo_Span_RespectsOffsetAndLength()
        {
            var uut = new ImmutableArraySegment<char>(new[] { 'x', 'a', 'b', 'c', 'x' }, 1, 3);
            var dest = new Span<char>(new char[6]);
            uut.CopyTo(in dest, 2);
            dest.ToArray().Should().BeEquivalentTo(C0, C0, 'a', 'b', 'c', C0);
        }

        [Fact]
        public void CopyTo_Span_WithLength_RespectsOffsetAndLength()
        {
            var uut = new ImmutableArraySegment<char>(new[] { 'x', 'a', 'b', 'c', 'x' }, 1, 3);
            var dest = new Span<char>(new char[6]);
            uut.CopyTo(in dest, 2, 2);
            dest.ToArray().Should().BeEquivalentTo(C0, C0, 'a', 'b', C0, C0);
        }

        [Fact]
        public void CopyTo_Memory_RespectsOffsetAndLength()
        {
            var uut = new ImmutableArraySegment<char>(new[] { 'x', 'a', 'b', 'c', 'x' }, 1, 3);
            var dest = new Memory<char>(new char[6]);
            uut.CopyTo(in dest, 2);
            dest.ToArray().Should().BeEquivalentTo(C0, C0, 'a', 'b', 'c', C0);
        }

        [Fact]
        public void CopyTo_Memory_WithLength_RespectsOffsetAndLength()
        {
            var uut = new ImmutableArraySegment<char>(new[] { 'x', 'a', 'b', 'c', 'x' }, 1, 3);
            var dest = new Memory<char>(new char[6]);
            uut.CopyTo(in dest, 2, 2);
            dest.ToArray().Should().BeEquivalentTo(C0, C0, 'a', 'b', C0, C0);
        }
    }
}
