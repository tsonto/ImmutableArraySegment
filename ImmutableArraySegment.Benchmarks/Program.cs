using System;
using System.Diagnostics;

namespace Benchmarks
{
	partial class Program
	{
		private static void Main()
		{
			Scenario1_SmallStruct();
			Scenario1_MediumStruct();
			Scenario1_LargeStruct();
			Scenario1_SmallClass();

			Scenario2_SmallStruct();
			Scenario2_MediumStruct();
			Scenario2_LargeStruct();
			Scenario2_SmallClass();

			Scenario2_SmallStruct_WithFastCheck();
			Scenario2_MediumStruct_WithFastCheck();
			Scenario2_LargeStruct_WithFastCheck();
			Scenario2_SmallClass_WithFastCheck();

			Scenario3_SmallStruct();
			Scenario3_MediumStruct();
			Scenario3_LargeStruct();
			Scenario3_SmallClass();
		}

		private static void Test<TIn, TOut>(string name, Func<TIn, TOut> action, TIn value, TOut expected)
		{
			Stopwatch sw = new();
			int durationMs = 1000;
			int count = 0;
			sw.Start();
			TOut? result;
			while (true)
			{
				result = action(value);
				if (sw.ElapsedMilliseconds > durationMs)
					break;
				++count;
			}

			if (Equals(expected, result))
				Console.WriteLine($"{name}: {count:0,000} iterations");
			else
				Console.WriteLine($"FAILED! {name}: expected {expected}, actual {result}");
		}
	}
}