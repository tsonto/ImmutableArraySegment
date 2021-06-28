using System;

namespace Tests
{
	internal class StrictEquatable : IEquatable<StrictEquatable>, IComparable<StrictEquatable>
	{
		internal readonly char value;

		public StrictEquatable(char value)
		{
			this.value = value;
		}

		bool IEquatable<StrictEquatable>.Equals(StrictEquatable other)
			=> char.ToLowerInvariant(value) == char.ToLowerInvariant(other.value);

		public bool Equals(StrictEquatable other)
			=> throw new InvalidOperationException("The code should not use T.Equals.");

		public override bool Equals(object obj)
			=> throw new InvalidOperationException("The code should not use Object.Equals.");

		public static bool operator==(StrictEquatable a, StrictEquatable b)
			=> throw new InvalidOperationException("The code should not use ==.");

		public static bool operator !=(StrictEquatable a, StrictEquatable b)
			=> throw new InvalidOperationException("The code should not use !=.");

		public override int GetHashCode()
			=> value;

		int IComparable<StrictEquatable>.CompareTo(StrictEquatable other)
			=> throw new InvalidOperationException("The code should not use CompareTo.");
	}
}