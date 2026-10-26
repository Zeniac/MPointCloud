# MPointCloud

**MPointCloud by Mekneo — part of the Mekneo engineering software ecosystem**

**MPointCloud** is a high-performance .NET point-cloud data I/O library with support for PCD, XYZ-family text, PLY, and PTS.

MPointCloud is a standalone Mekneo library and the preferred point-cloud I/O provider for Mektral.

## Install

```bash
dotnet add package MPointCloud --version 1.0.0
```

## Supported formats

- PCD 0.7 ASCII
- PCD 0.7 binary
- PCD 0.7 binary_compressed
- XYZ
- XYZI
- XYZRGB
- XYZRGBA
- PLY 1.0 ASCII
- PLY 1.0 binary little-endian
- PLY 1.0 binary big-endian
- PTS XYZI
- PTS XYZI + RGB

## Quick PCD example

```csharp
using MPointCloud.Formats.Pcd;

var (header, data) = PcdReader.Read("cloud.pcd");

Console.WriteLine($"Points: {data.Count}");

PcdWriter.Write(
    "copy.pcd",
    data,
    PcdDataEncoding.BinaryCompressed);
```

## Read from a stream

```csharp
using var input = File.OpenRead("cloud.pcd");
var (header, data) = PcdReader.Read(input);
```

Since preview.2, ASCII, binary, and binary_compressed PCD can be read from seekable or non-seekable .NET streams. Reads start at the current position and leave caller streams open on success and failure. These complete-cloud APIs are separate from bounded chunk streaming.

## Samples

The public MPointCloud repository includes runnable sample projects covering:

- PCD read/write and encoding conversion
- complete-cloud PCD input from memory and ZIP-entry streams
- bounded PCD streaming
- async PCD streaming
- XYZ-family I/O
- PLY
- PTS
- arbitrary-schema PCD data

Browse the samples:

https://github.com/Zeniac/MPointCloud/tree/master/samples

The standalone GitHub release ZIP also includes the same sample source configured to reference the packaged MPointCloud DLL.

## Mektral integration

MPointCloud 1.0 integrates with Mektral through an optional point-cloud I/O adapter. The adapter imports PCD data into Mektral-owned types while MPointCloud remains usable independently. Mektral assemblies are acquired separately.

Provider usage and mapping limits: https://github.com/Zeniac/MPointCloud/blob/master/docs/mektral-integration.md

## Documentation

- Getting started: https://github.com/Zeniac/MPointCloud/blob/master/docs/getting-started.md
- Installation: https://github.com/Zeniac/MPointCloud/blob/master/docs/installation.md
- Supported formats: https://github.com/Zeniac/MPointCloud/blob/master/docs/supported-formats.md
- Performance: https://github.com/Zeniac/MPointCloud/blob/master/docs/performance.md
- PLY: https://github.com/Zeniac/MPointCloud/blob/master/docs/ply.md
- PTS: https://github.com/Zeniac/MPointCloud/blob/master/docs/pts.md

## Performance

MPointCloud includes benchmarked sequential, ordered-parallel, unordered-parallel, binary, and binary-compressed I/O paths.

Performance depends on CPU core count, single-core speed, memory bandwidth, storage performance, runtime/JIT version, dataset schema/size, system load, and thermal/power limits. Published benchmark figures are comparative evidence, not universal throughput guarantees.

See:

https://github.com/Zeniac/MPointCloud/blob/master/docs/performance.md

## Support and feedback

Report bugs and request features at https://github.com/Zeniac/MPointCloud/issues. Include the package/runtime version, format/encoding, API/options, expected and actual results, and a small shareable reproducer. For private inquiries, use **Contact owners** on this NuGet page.

Support guidance: https://github.com/Zeniac/MPointCloud/blob/master/SUPPORT.md

## Who uses MPointCloud?

If MPointCloud is useful in your application, we would like to hear about it. Use **Contact owners** on the [NuGet package page](https://www.nuget.org/packages/MPointCloud) to share your company/product name, an optional website link, and a short description of how you use the library.

Please state whether we may list the company/product name and link publicly. Usage reporting is voluntary and separate from required license attribution. Logos and testimonials require separate permission.

## License

MPointCloud is proprietary closed-source software distributed under the MPointCloud Free Binary License.

Permitted personal, academic, research, and commercial use is available subject to the license and attribution requirements.

Copyright © 2026 Mekneo.

