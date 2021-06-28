using System.Linq;
using Tsonto.Collections.Generic;

namespace Benchmarks
{
	partial class Program
	{
		private static void Scenario2_SmallStruct()
		{
			var seed = Enumerable.Range(0, 100).Select(i => new SmallStruct(i)).ToImmutableArraySegment();
			static int Run(ImmutableArraySegment<SmallStruct> input)
			{
				var i = 0;
				var value = input[^50];
				i = input.IndexOf(value, i);
				value = input[^25];
				i = input.IndexOf(value, i);
				value = input[^12];
				i = input.IndexOf(value, i);
				value = input[0];
				return input.IndexOf(value, i);
			}
			Test(nameof(Scenario2_SmallStruct), Run, seed, -1);
		}

		private static void Scenario2_SmallStruct_WithFastCheck()
		{
			var seed = Enumerable.Range(0, 100).Select(i => new SmallStruct(i)).ToImmutableArraySegment();
			static int Run(ImmutableArraySegment<SmallStruct> input)
			{
				static bool Check(in SmallStruct a, in SmallStruct b)
					=> a.X == b.X;

				var i = 0;
				var value = input[^50];
				i = input.IndexOf(value, i, Check);
				value = input[^25];
				i = input.IndexOf(value, i, Check);
				value = input[^12];
				i = input.IndexOf(value, i, Check);
				value = input[0];
				return input.IndexOf(value, i, Check);
			}
			Test(nameof(Scenario2_SmallStruct_WithFastCheck), Run, seed, -1);
		}

		private static void Scenario2_MediumStruct()
		{
			var seed = Enumerable.Range(0, 100).Select(i => new MediumStruct(i)).ToImmutableArraySegment();
			static int Run(ImmutableArraySegment<MediumStruct> input)
			{
				var i = 0;
				var value = input[^50];
				i = input.IndexOf(value, i);
				value = input[^25];
				i = input.IndexOf(value, i);
				value = input[^12];
				i = input.IndexOf(value, i);
				value = input[0];
				return input.IndexOf(value, i);
			}
			Test(nameof(Scenario2_MediumStruct), Run, seed, -1);
		}

		private static void Scenario2_MediumStruct_WithFastCheck()
		{
			var seed = Enumerable.Range(0, 100).Select(i => new MediumStruct(i)).ToImmutableArraySegment();
			static int Run(ImmutableArraySegment<MediumStruct> input)
			{
				static bool Check(in MediumStruct x, in MediumStruct y)
					=> x.a == y.a
					&& x.b == y.b
					&& x.c == y.c
					&& x.d == y.d;

				var i = 0;
				var value = input[^50];
				i = input.IndexOf(value, i, Check);
				value = input[^25];
				i = input.IndexOf(value, i, Check);
				value = input[^12];
				i = input.IndexOf(value, i, Check);
				value = input[0];
				return input.IndexOf(value, i, Check);
			}
			Test(nameof(Scenario2_MediumStruct_WithFastCheck), Run, seed, -1);
		}

		private static void Scenario2_LargeStruct()
		{
			var seed = Enumerable.Range(0, 100).Select(i => new LargeStruct(i)).ToImmutableArraySegment();
			static int Run(ImmutableArraySegment<LargeStruct> input)
			{
				var i = 0;
				var value = input[^50];
				i = input.IndexOf(value, i);
				value = input[^25];
				i = input.IndexOf(value, i);
				value = input[^12];
				i = input.IndexOf(value, i);
				value = input[0];
				return input.IndexOf(value, i);
			}
			Test(nameof(Scenario2_LargeStruct), Run, seed, -1);
		}

		private static void Scenario2_LargeStruct_WithFastCheck()
		{
			var seed = Enumerable.Range(0, 100).Select(i => new LargeStruct(i)).ToImmutableArraySegment();
			static int Run(ImmutableArraySegment<LargeStruct> input)
			{
				static bool Check(in LargeStruct x, in LargeStruct y)
					=> x.a == y.a && x.b == y.b && x.c == y.c && x.d == y.d
					&& x.e == y.e && x.f == y.f && x.g == y.g && x.h == y.h
					&& x.i == y.i && x.j == y.j && x.k == y.k && x.l == y.l
					&& x.m == y.m && x.n == y.n && x.o == y.o && x.p == y.p
					&& x.q == y.q && x.r == y.r && x.s == y.s && x.t == y.t
					&& x.u == y.u && x.v == y.v && x.w == y.w && x.x == y.x
					&& x.y == y.y && x.z == y.z;

				var i = 0;
				var value = input[^50];
				i = input.IndexOf(value, i, Check);
				value = input[^25];
				i = input.IndexOf(value, i, Check);
				value = input[^12];
				i = input.IndexOf(value, i, Check);
				value = input[0];
				return input.IndexOf(value, i, Check);
			}
			Test(nameof(Scenario2_LargeStruct_WithFastCheck), Run, seed, -1);
		}

		private static void Scenario2_SmallClass()
		{
			var seed = Enumerable.Range(0, 100).Select(i => new SmallClass(i)).ToImmutableArraySegment();
			static int Run(ImmutableArraySegment<SmallClass> input)
			{
				var i = 0;
				var value = input[^50];
				i = input.IndexOf(value, i);
				value = input[^25];
				i = input.IndexOf(value, i);
				value = input[^12];
				i = input.IndexOf(value, i);
				value = input[0];
				return input.IndexOf(value, i);
			}
			Test(nameof(Scenario2_SmallClass), Run, seed, -1);
		}

		private static void Scenario2_SmallClass_WithFastCheck()
		{
			var seed = Enumerable.Range(0, 100).Select(i => new SmallClass(i)).ToImmutableArraySegment();
			static int Run(ImmutableArraySegment<SmallClass> input)
			{
				static bool Check(in SmallClass x, in SmallClass y)
					=> x.A == y.A;

				var i = 0;
				var value = input[^50];
				i = input.IndexOf(value, i, Check);
				value = input[^25];
				i = input.IndexOf(value, i, Check);
				value = input[^12];
				i = input.IndexOf(value, i, Check);
				value = input[0];
				return input.IndexOf(value, i, Check);
			}
			Test(nameof(Scenario2_SmallClass_WithFastCheck), Run, seed, -1);
		}
	}
}