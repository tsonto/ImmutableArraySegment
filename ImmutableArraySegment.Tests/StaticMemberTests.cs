using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
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

        [Fact]
        public void Join_BasicCase()
        {
            var sources = new IReadOnlyList<char>[]
            {
                new[]{ 'a', 'b', 'c' },
                new ImmutableArraySegment<char>(new[]{ 'd', 'e' }),
                new[]{ 'f', 'g', 'h', 'i' }.ToList(),
            };
            var delimiter = new[] { 'x', 'y' };

            var actual = ImmutableArraySegment.Join(delimiter, sources);

            var expected = new[] { 'a', 'b', 'c', 'x', 'y', 'd', 'e', 'x', 'y', 'f', 'g', 'h', 'i' };
            actual.Should().BeEquivalentTo(expected);
        }

        private class BaseClass { }

        private class DerivedClass : BaseClass { }
    }
}
