using FluentAssertions;
using System;
using System.Collections.Generic;
using Tsonto.Collections.Generic;
using Xunit;

namespace Tests
{
	public class ConstructorTests
	{
		[Fact]
		public void CtorWithReadOnlySpan_CopiesContents()
		{
			var original = new[] { 'a', 'b', 'c' };
			var wrap = new ReadOnlySpan<char>(original);
			var uut = new ImmutableArraySegment<char>(wrap);
			original[1] = 'x';
			uut.data.Should().BeEquivalentTo('a', 'b', 'c');
		}

		[Fact]
		public void CtorWithSpan_CopiesContents()
		{
			var original = new[] { 'a', 'b', 'c' };
			var wrap = new Span<char>(original);
			var uut = new ImmutableArraySegment<char>(wrap);
			original[1] = 'x';
			uut.data.Should().BeEquivalentTo('a', 'b', 'c');
		}

		[Fact]
		public void CtorWithReadOnlyMemory_CopiesContents()
		{
			var original = new[] { 'a', 'b', 'c' };
			var wrap = new ReadOnlyMemory<char>(original);
			var uut = new ImmutableArraySegment<char>(wrap);
			original[1] = 'x';
			uut.data.Should().BeEquivalentTo('a', 'b', 'c');
		}

		[Fact]
		public void CtorWithMemory_CopiesContents()
		{
			var original = new[] { 'a', 'b', 'c' };
			var wrap = new Memory<char>(original);
			var uut = new ImmutableArraySegment<char>(wrap);
			original[1] = 'x';
			uut.data.Should().BeEquivalentTo('a', 'b', 'c');
		}

		[Fact]
		public void CtorWithArray_CopiesContents()
		{
			var original = new[] { 'a', 'b', 'c' };
			var uut = new ImmutableArraySegment<char>(original);
			original[1] = 'x';
			uut.data.Should().BeEquivalentTo('a', 'b', 'c');
		}

		[Fact]
		public void CtorWithArrayAndRange_FullRange_CopiesArray()
		{
			var original = new[] { 'a', 'b', 'c' };
			var uut = new ImmutableArraySegment<char>(original, 0..);
			original[1] = 'x';
			uut.data.Should().BeEquivalentTo('a', 'b', 'c');
		}

		[Fact]
		public void CtorWithArrayAndRange_PartialRange_CopiesCorrectPortionOfArray()
		{
			var original = new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
			var uut = new ImmutableArraySegment<char>(original, 2..6);
			uut.ToArray().Should().BeEquivalentTo('c','d','e','f');
		}

		[Fact]
		public void CtorWithArrayAsEnumerable_CopiesContents()
		{
			var original = new[] { 'a', 'b', 'c' };
			var uut = new ImmutableArraySegment<char>((IEnumerable<char>)original);
			original[1] = 'x';
			uut.data.Should().BeEquivalentTo('a', 'b', 'c');
		}

		[Fact]
		public void CtorWithArrayAsEnumerableWithRange_CopiesContents()
		{
			var original = new[] { 'a', 'b', 'c' };
			var uut = new ImmutableArraySegment<char>((IEnumerable<char>)original, 1..);
			original[1] = 'x';
			uut.data.Should().BeEquivalentTo('b', 'c');
		}

		[Fact]
		public void CtorWithImmutableArraySegmentAsEnumerable_DoesNotCopy()
		{
			var original = new[] { 'a', 'b', 'c' };
			var wrap = new ImmutableArraySegment<char>(original);
			var uut = new ImmutableArraySegment<char>((IEnumerable<char>)wrap);
			original[1] = 'x';
			uut.data.Should().BeSameAs(wrap.data);
			uut.ToArray().Should().BeEquivalentTo(wrap.ToArray());
		}

		[Fact]
		public void CtorWithImmutableArraySegmentAsEnumerableWithRange_DoesNotCopy()
		{
			var original = new[] { 'a', 'b', 'c' };
			var wrap = new ImmutableArraySegment<char>(original);
			var uut = new ImmutableArraySegment<char>((IEnumerable<char>)wrap, 1..);
			original[1] = 'x';
			uut.data.Should().BeSameAs(wrap.data);
			uut.ToArray().Should().BeEquivalentTo('b','c');
		}

		[Fact]
		public void CtorWithArraySegmentAsEnumerableWithRange_CopiesContent()
		{
			var original = new[] { 'a', 'b', 'c' };
			var wrap = new ArraySegment<char>(original);
			var uut = new ImmutableArraySegment<char>((IEnumerable<char>)wrap, 1..);
			original[1] = 'x';
			uut.data.Should().NotBeSameAs(wrap.Array);
			uut.ToArray().Should().BeEquivalentTo('b', 'c');
		}

		[Fact]
		public void CtorWithCollectionAsEnumerable_CopiesContents()
		{
			var original = new[] { 'a', 'b', 'c' };
			var wrap = new StrictCollection<char>(original);
			var uut = new ImmutableArraySegment<char>(wrap);
			original[1] = 'x';
			uut.data.Should().BeEquivalentTo('a', 'b', 'c');
		}

		[Fact]
		public void CtorWithCollectionAsEnumerableAndRange_FullRange_CopiesContentsAndDoesNotUseEnumerator()
		{
			var original = new[] { 'a', 'b', 'c' };
			var wrap = new StrictCollection<char>(original);
			var uut = new ImmutableArraySegment<char>(wrap, ..);
			original[1] = 'x';
			uut.data.Should().BeEquivalentTo('a', 'b', 'c');
		}

		[Fact]
		public void CtorWithCollectionAsEnumerableAndRange_PartialRange_CopiesContentsAndDoesNotUseCopyTo()
		{
			var original = new[] { 'a', 'b', 'c' };
			var wrap = new CollectionWithoutCopyTo<char>(original);
			var uut = new ImmutableArraySegment<char>(wrap, 1..);
			original[1] = 'x';
			uut.data.Should().BeEquivalentTo('b', 'c');
		}

		[Fact]
		public void CtorWithReadOnlyListAsEnumerable_CopiesContents()
		{
			var original = new[] { 'a', 'b', 'c' };
			var wrap = new StrictReadOnlyList<char>(original);
			var uut = new ImmutableArraySegment<char>(wrap);
			original[1] = 'x';
			uut.data.Should().BeEquivalentTo('a', 'b', 'c');
		}

		[Fact]
		public void CtorWithReadOnlyListAsEnumerableAndRange_CopiesContents()
		{
			var original = new[] { 'a', 'b', 'c' };
			var wrap = new StrictReadOnlyList<char>(original);
			var uut = new ImmutableArraySegment<char>(wrap, 1..);
			original[1] = 'x';
			uut.data.Should().BeEquivalentTo('b', 'c');
		}

		[Fact]
		public void CtorWithEnumerable_CopiesContents()
		{
			var original = new[] { 'a', 'b', 'c' };
			var wrap = new StrictEnumerable<char>(original);
			var uut = new ImmutableArraySegment<char>(wrap);
			original[1] = 'x';
			uut.data.Should().BeEquivalentTo('a', 'b', 'c');
		}

		[Fact]
		public void CtorWithEnumerableAndSimpleRange_CopiesContents()
		{
			var original = new[] { 'a', 'b', 'c' };
			var wrap = new StrictEnumerable<char>(original);
			var uut = new ImmutableArraySegment<char>(wrap, 1..);
			original[1] = 'x';
			uut.data.Should().BeEquivalentTo('b', 'c');
		}

		[Fact]
		public void CtorWithEnumerableAndFromEndRange_CopiesContents()
		{
			var original = new[] { 'a', 'b', 'c' };
			var wrap = new StrictEnumerable<char>(original);
			var uut = new ImmutableArraySegment<char>(wrap, ^2..);
			original[1] = 'x';
			uut.data.Should().BeEquivalentTo('b', 'c');
		}

		[Fact]
		public void CtorWithArrayAndStartAndLength_CopiesContents()
		{
			var original = new[] { 'a', 'b', 'c' };
			var uut = new ImmutableArraySegment<char>(original, 1, 1);
			original[1] = 'x';
			uut.data.Should().BeEquivalentTo('b');
		}
	}
}