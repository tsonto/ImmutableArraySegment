# ImmutableArraySegment

This package provides a type `ImmutableArraySegment<T>` that provides a view of an immutable array. It is optimized for slice operations.

## Usage

```
using Tsonto.Collections.Generic;

var segment = new ImmutableArraySegment<char>(new [] { 'a', 'b', 'c', 'd' });
var mid = segment[1..^1];     // b,c
var last = segment[^1];       // d
var combined = mid + segment; // b,c,a,b,c,d
```

## Documentation
Every type, method, property, etc. has descriptive XML comments. Every method's documentation includes remarks detailing its performance characteristics.

## License
DockerSdk is licensed under the [MIT license](LICENSE).

## Code of Conduct
The DockerSdk project follows the [Contributor Covenant](CODE_OF_CONDUCT.md). Harrasment will not be tolerated.
