using Tsonto.Collections.Generic;

namespace Benchmarks
{
	partial class Program
	{
		private static void Scenario1_SmallStruct()
		{
			var array = new SmallStruct[100];
			static int Run(SmallStruct[] input)
			{
				var a = new ImmutableArraySegment<SmallStruct>(input);
				var b = a[30..90];
				var c = b[10..^10];
				return c.ToArray().Length;
			}
			Test("Scenario 1, small struct", Run, array, 40);
		}

		private static void Scenario1_MediumStruct()
		{
			var array = new MediumStruct[100];
			static int Run(MediumStruct[] input)
			{
				var a = new ImmutableArraySegment<MediumStruct>(input);
				var b = a[30..90];
				var c = b[10..^10];
				return c.ToArray().Length;
			}
			Test("Scenario 1, medium struct", Run, array, 40);
		}

		private static void Scenario1_LargeStruct()
		{
			var array = new LargeStruct[100];
			static int Run(LargeStruct[] input)
			{
				var a = new ImmutableArraySegment<LargeStruct>(input);
				var b = a[30..90];
				var c = b[10..^10];
				return c.ToArray().Length;
			}
			Test("Scenario 1, large struct", Run, array, 40);
		}

		private static void Scenario1_SmallClass()
		{
			var array = new SmallClass[100];
			static int Run(SmallClass[] input)
			{
				var a = new ImmutableArraySegment<SmallClass>(input);
				var b = a[30..90];
				var c = b[10..^10];
				return c.ToArray().Length;
			}
			Test("Scenario 1, small class", Run, array, 40);
		}
	}
}