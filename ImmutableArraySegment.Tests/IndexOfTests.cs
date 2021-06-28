using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Tsonto.Collections.Generic;
using Xunit;

namespace Tests
{
	public class IndexOfTests
	{
		private const char c0 = '\0';

		[Theory]
		[InlineData('A', 0)] // match at start
		[InlineData('D', 3)] // match normal
		[InlineData('H', 7)] // match at end
		[InlineData(c0, -1)] // no match
		public void IndexOf_Item(char sought, int expected)
		{
			var inner = new StrictEquatable[] { new(c0), new(c0), new('a'), new('b'), new('c'), new('d'), new('e'), new('f'), new('g'), new('h'), new(c0) };
			var uut = new ImmutableArraySegment<StrictEquatable>(inner, 2, 8, raw: true);
			uut.IndexOf(new(sought)).Should().Be(expected);
		}

		[Theory]
		[InlineData('Ã', 0)] // match at start
		[InlineData('Ď', 3)] // match normal
		[InlineData('Ħ', 7)] // match at end
		[InlineData(c0, -1)] // no match
		public void IndexOf_ItemComparer(char sought, int expected)
		{
			var inner = new StrictEquatable[] { new(c0), new(c0), new('a'), new('b'), new('c'), new('d'), new('e'), new('f'), new('g'), new('h'), new(c0) };
			var uut = new ImmutableArraySegment<StrictEquatable>(inner, 2, 8, raw: true);
			uut.IndexOf(new(sought), new StrangeComparer()).Should().Be(expected);
		}

		[Theory]
		[InlineData('A', -1)] // no match (cut off by start position)
		[InlineData('B', 1)] // match at search start
		[InlineData('D', 3)] // match normal
		[InlineData('H', 7)] // match at end
		[InlineData(c0, -1)] // no match
		public void IndexOf_ItemIndex(char sought, int expected)
		{
			var inner = new StrictEquatable[] { new(c0), new(c0), new('a'), new('b'), new('c'), new('d'), new('e'), new('f'), new('g'), new('h'), new(c0) };
			var uut = new ImmutableArraySegment<StrictEquatable>(inner, 2, 8, raw: true);
			uut.IndexOf(new(sought), 1).Should().Be(expected);
		}

		[Theory]
		[InlineData('Ã', -1)] // no match (cut off by start position)
		[InlineData('Ɓ', 1)] // match at search start
		[InlineData('Ď', 3)] // match normal
		[InlineData('Ħ', 7)] // match at end
		[InlineData(c0, -1)] // no match
		public void IndexOf_ItemIndexComparer(char sought, int expected)
		{
			var inner = new StrictEquatable[] { new(c0), new(c0), new('a'), new('b'), new('c'), new('d'), new('e'), new('f'), new('g'), new('h'), new(c0) };
			var uut = new ImmutableArraySegment<StrictEquatable>(inner, 2, 8, raw: true);
			uut.IndexOf(new(sought), 1, new StrangeComparer()).Should().Be(expected);
		}

		[Theory]
		[InlineData('A', -1)] // no match (cut off by start position)
		[InlineData('B', 1)] // match at search start
		[InlineData('D', 3)] // match normal
		[InlineData('G', 6)] // match last count
		[InlineData('H', -1)] // no match (cut off by count)
		[InlineData(c0, -1)] // no match
		public void IndexOf_ItemIndexCount(char sought, int expected)
		{
			var inner = new StrictEquatable[] { new(c0), new(c0), new('a'), new('b'), new('c'), new('d'), new('e'), new('f'), new('g'), new('h'), new(c0) };
			var uut = new ImmutableArraySegment<StrictEquatable>(inner, 2, 8, raw: true);
			uut.IndexOf(new(sought), 1, 6).Should().Be(expected);
		}

		[Theory]
		[InlineData('Ã', -1)] // no match (cut off by start position)
		[InlineData('Ɓ', 1)] // match at search start
		[InlineData('Ď', 3)] // match normal
		[InlineData('Ĝ', 6)] // match last count
		[InlineData('Ħ', -1)] // no match (cut off by count)
		[InlineData(c0, -1)] // no match
		public void IndexOf_ItemIndexCountComparer(char sought, int expected)
		{
			var inner = new StrictEquatable[] { new(c0), new(c0), new('a'), new('b'), new('c'), new('d'), new('e'), new('f'), new('g'), new('h'), new(c0) };
			var uut = new ImmutableArraySegment<StrictEquatable>(inner, 2, 8, raw: true);
			uut.IndexOf(new(sought), 1, 6, new StrangeComparer()).Should().Be(expected);
		}

		[Theory]
		[InlineData(0, 0, false)]
		[InlineData(0, 8, false)]
		[InlineData(0, 9, true)]
		[InlineData(1, 7, false)]
		[InlineData(1, 8, true)]
		[InlineData(4, 2, false)]
		[InlineData(8, 0, false)]
		[InlineData(9, 0, true)]
		[InlineData(-1, 7, true)]
		public void IndexOf_ItemIndexCount_ValidatesInputs(int start, int count, bool shouldThrow)
		{
			var inner = new StrictEquatable[] { new(c0), new(c0), new('a'), new('b'), new('c'), new('d'), new('e'), new('f'), new('g'), new('h'), new(c0) };
			var uut = new ImmutableArraySegment<StrictEquatable>(inner, 2, 8, raw: true);
			void act() => uut.IndexOf(new('c'), start, count);

			if (shouldThrow)
				Assert.Throws<ArgumentOutOfRangeException>(act);
			else
				act();
		}

		private class StrangeComparer : IEqualityComparer<StrictEquatable>
		{
			public bool Equals(StrictEquatable x, StrictEquatable y)
			{
				var xChar = x.value;
				var yChar = y.value;
				return xChar switch
				{
					'a' => yChar == 'Ã',
					'b' => yChar == 'Ɓ',
					'c' => yChar == 'Ç',
					'd' => yChar == 'Ď',
					'e' => yChar == 'È',
					'f' => yChar == 'Φ',
					'g' => yChar == 'Ĝ',
					'h' => yChar == 'Ħ',
					'Ã' => yChar == 'a',
					'Ɓ' => yChar == 'b',
					'Ç' => yChar == 'c',
					'Ď' => yChar == 'd',
					'È' => yChar == 'e',
					'Φ' => yChar == 'f',
					'Ĝ' => yChar == 'g',
					'Ħ' => yChar == 'h',
					_ => false
				};
			}

			public int GetHashCode([DisallowNull] StrictEquatable x) 
				=> throw new NotImplementedException();
		}
	}
}