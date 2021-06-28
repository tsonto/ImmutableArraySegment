using System.Linq;
using Tsonto.Collections.Generic;

namespace Benchmarks
{
	partial class Program
	{
		private static void Scenario3_SmallStruct()
		{
			var x = Enumerable.Range(0, 100).Select(i => new SmallStruct(i)).ToImmutableArraySegment();
			var y = Enumerable.Range(100, 100).Select(i => new SmallStruct(i)).ToImmutableArraySegment();
			static int Run((ImmutableArraySegment<SmallStruct> a, ImmutableArraySegment<SmallStruct> b) input)
			{
				var c = input.a[10..].Append(input.b[..^10]);
				var d = c.Insert(90, new SmallStruct(-1));
				var e = d.Insert(30, input.a);
				var f = e.Insert(^30, Enumerable.Range(200, 100).Select(i => new SmallStruct(i)));
				return f.Length;
			}
			Test(nameof(Scenario2_SmallStruct), Run, (x, y), 381);
		}

		private static void Scenario3_MediumStruct()
		{
			var x = Enumerable.Range(0, 100).Select(i => new MediumStruct(i)).ToImmutableArraySegment();
			var y = Enumerable.Range(100, 100).Select(i => new MediumStruct(i)).ToImmutableArraySegment();
			static int Run((ImmutableArraySegment<MediumStruct> a, ImmutableArraySegment<MediumStruct> b) input)
			{
				var c = input.a[10..].Append(input.b[..^10]);
				var d = c.Insert(90, new MediumStruct(-1));
				var e = d.Insert(30, input.a);
				var f = e.Insert(^30, Enumerable.Range(200, 100).Select(i => new MediumStruct(i)));
				return f.Length;
			}
			Test(nameof(Scenario3_MediumStruct), Run, (x, y), 381);
		}

		private static void Scenario3_LargeStruct()
		{
			var x = Enumerable.Range(0, 100).Select(i => new LargeStruct(i)).ToImmutableArraySegment();
			var y = Enumerable.Range(100, 100).Select(i => new LargeStruct(i)).ToImmutableArraySegment();
			static int Run((ImmutableArraySegment<LargeStruct> a, ImmutableArraySegment<LargeStruct> b) input)
			{
				var c = input.a[10..].Append(input.b[..^10]);
				var d = c.Insert(90, new LargeStruct(-1));
				var e = d.Insert(30, input.a);
				var f = e.Insert(^30, Enumerable.Range(200, 100).Select(i => new LargeStruct(i)));
				return f.Length;
			}
			Test(nameof(Scenario3_LargeStruct), Run, (x, y), 381);
		}

		private static void Scenario3_SmallClass()
		{
			var x = Enumerable.Range(0, 100).Select(i => new SmallClass(i)).ToImmutableArraySegment();
			var y = Enumerable.Range(100, 100).Select(i => new SmallClass(i)).ToImmutableArraySegment();
			static int Run((ImmutableArraySegment<SmallClass> a, ImmutableArraySegment<SmallClass> b) input)
			{
				var c = input.a[10..].Append(input.b[..^10]);
				var d = c.Insert(90, new SmallClass(-1));
				var e = d.Insert(30, input.a);
				var f = e.Insert(^30, Enumerable.Range(200, 100).Select(i => new SmallClass(i)));
				return f.Length;
			}
			Test(nameof(Scenario3_SmallClass), Run, (x, y), 381);
		}
	}
}