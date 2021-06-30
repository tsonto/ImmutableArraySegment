namespace Benchmarks
{
    internal readonly struct MediumStruct
	{
		public readonly int a;
		public readonly int b;
		public readonly int c;
		public readonly int d;

		public MediumStruct(int start) : this()
		{
			a = start++;
			b = start++;
			c = start++;
			d = start++;
		}
	}
}
