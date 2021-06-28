using FluentAssertions;
using System.Collections.Immutable;
using Tsonto.Collections.Generic;
using Xunit;

namespace Tests
{
	public class StaticMemberTests
	{
		[Fact]
		public void EmptyProperty_IsEmpty()
		{
			ImmutableArraySegment<char>.Empty.Length.Should().Be(0);
		}

		[Fact]
		public void EmptyMethod_IsEmpty()
		{
			ImmutableArraySegment.Empty<char>().Length.Should().Be(0);
		}

		[Fact]
		// Not a static member, but it's so closely related to Empty that this seems like the best place for testing it
		public void Clear_ReturnsEmpty()
		{
			var uut = (IImmutableList<char>)new ImmutableArraySegment<char>();
			uut.Clear().Should().BeEmpty();
		}

		[Fact]
		public void As_CanCastToBaseType()
		{
			var d = new ImmutableArraySegment<DerivedClass>(new DerivedClass[5]);
			ImmutableArraySegment<BaseClass> b = d.As<DerivedClass, BaseClass>();
			b.Length.Should().Be(5);
		}

		[Fact]
		public void ToImmutableArraySegment_CopiesContent()
		{
			var original = new[] { 'a', 'b', 'c', 'd' };
			var wrap = new StrictEnumerable<char>(original);
			var uut = wrap.ToImmutableArraySegment();
			original[1] = 'x';
			uut.ToArray().Should().BeEquivalentTo('a', 'b', 'c', 'd');
		}

		private class BaseClass { }

		private class DerivedClass : BaseClass { }
	}
}