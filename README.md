# MPointCloud

**MPointCloud by Mekneo — part of the Mekneo engineering software ecosystem**

**MPointCloud** is a standalone high-performance .NET library for reading and writing point-cloud data, with support for **PCD 0.7**, the **XYZ family**, **PLY 1.0**, and a documented **PTS** scanner-text dialect.

It is designed to provide a lightweight, fast, and easy-to-integrate PCD implementation for .NET applications while remaining independent of any rendering engine or application framework.

MPointCloud is a standalone Mekneo library and the preferred point-cloud I/O provider for MEngine.

## Quick Start

Install from NuGet:

```bash
dotnet add package MPointCloud --version 1.0.0
```

Documentation:

- [Installation](docs/installation.md)
- [Getting Started](docs/getting-started.md)
- [Supported Formats](docs/supported-formats.md)
- [Performance](docs/performance.md)
- [Samples](samples/README.md)
- [Changelog](CHANGELOG.md)
- [Support and Feedback](SUPPORT.md)
- [Full Documentation Index](docs/README.md)

**NuGet users:** the public MPointCloud repository includes runnable sample projects for PCD, streaming, XYZ, PLY, and PTS. The same samples are also included in the standalone binary release package.


## Formats and High-Performance I/O at a Glance

MPointCloud is designed to make common point-cloud formats easy to consume from .NET while also providing explicit high-performance read/write modes for large datasets.

| Format / Variant | Sequential Read | Ordered Parallel Read | Unordered Parallel Read | Sync Chunk Read | Async Chunk Read | Sequential Write | Ordered Parallel Write | Sync Chunk Write | Async Chunk Write |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| **PCD 0.7 ASCII** | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| **PCD 0.7 binary** | Yes | N/A | N/A | Yes | Yes | Yes | N/A | Yes | Yes |
| **PCD 0.7 binary_compressed (LZF)** | Yes | N/A | N/A | No | No | Yes | N/A | No | No |
| **XYZ** | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| **XYZI** | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| **XYZRGB** | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| **XYZRGBA** | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| **PLY 1.0 ASCII** | Yes | No | No | No | No | Yes | No | No | No |
| **PLY 1.0 binary little-endian** | Yes | N/A | N/A | No | No | Yes | N/A | No | No |
| **PLY 1.0 binary big-endian** | Yes | N/A | N/A | No | No | Yes | N/A | No | No |
| **PTS XYZI** | Yes | Yes | Yes | No | No | Yes | Yes | No | No |
| **PTS XYZI RGB** | Yes | Yes | Yes | No | No | Yes | Yes | No | No |

The chunk columns describe incremental point processing. Complete-file .NET `Stream` input is listed separately below; a "No" for chunk reads does not mean that `Read(Stream)` is unavailable.

### Reader modes

PCD ASCII, XYZ-family, and PTS parallel readers use bounded row chunks for parallel parsing instead of loading the entire text body into memory first. The chunk size is configurable through `PointCloudReadOptions.TextChunkSize` and defaults to 4,096 rows.

PCD ASCII, XYZ-family, and PTS readers expose explicit execution modes through the shared `PointCloudReadOptions` API. PLY readers are sequential and do not accept these options:

- **Sequential** — default, deterministic parsing.
- **Ordered parallel** — parses points concurrently while preserving source point order.
- **Unordered parallel** — permits reordering when source order is not required; throughput depends on the workload.
- **Organized PCD safety** — unordered reads are blocked for organized clouds unless explicitly allowed; when allowed, the returned cloud is marked unorganized because grid topology is no longer valid.

Example:

```csharp
var data = XyzReader.Read(
    "cloud.xyz",
    XyzVariant.XyzRgb,
    new PointCloudReadOptions
    {
        ParallelMode = ParallelReadMode.PreserveOrder,
        MaxDegreeOfParallelism = 8
    });
```

For PCD ASCII:

```csharp
var (header, data) = PcdAsciiReader.Read(
    "cloud.pcd",
    new PointCloudReadOptions
    {
        ParallelMode = ParallelReadMode.PreserveOrder
    });
```

### Complete-file PCD stream input

Since `0.9.0-preview.2`, `PcdReader.Read(Stream)` detects ASCII, binary, or binary_compressed from one header parse. `PcdAsciiReader.Read(Stream)` and its read-options overload are also available.

| PCD encoding | Complete-file `Read(Stream)` | Non-seekable input | Parallel parsing from `Stream` | Bounded chunk read |
| --- | --- | --- | --- | --- |
| ASCII | Yes | Yes | Ordered or unordered, with read options | Yes |
| Binary | Yes | Yes | No | Yes |
| binary_compressed | Yes | Yes | No | No |

Caller streams are read from their current position and remain open on success or failure. Seekable and non-seekable inputs are supported, including ZIP-entry streams. Each input must contain one complete PCD through end of stream; these APIs materialize the complete cloud.

The chunk columns above describe incremental point processing, which is separate from accepting a .NET `Stream`. See [Getting Started](docs/getting-started.md) and the [PcdStreamInput sample](samples/README.md#pcdstreaminput).

### Streaming chunk readers

For applications that should not materialize an entire text cloud in memory, MPointCloud also exposes synchronous chunk-callback readers.

Supported streaming sources:

- **PCD 0.7 ASCII**
- **PCD 0.7 binary**
- **XYZ**
- **XYZI**
- **XYZRGB**
- **XYZRGBA**

Each callback receives a `PointCloudChunk` containing:

- chunk sequence index
- source point offset
- whether point order is preserved
- the chunk's `PointCloudData`

Example:

```csharp
PcdHeader header = PcdAsciiReader.ReadChunks(
    "large-cloud.pcd",
    chunk =>
    {
        Process(chunk.Data);
    },
    new PointCloudReadOptions
    {
        ParallelMode = ParallelReadMode.PreserveOrder,
        TextChunkSize = 4096
    });
```

This allows applications to process, convert, upload, index, or render batches without constructing one complete in-memory cloud. Point-data working memory remains bounded by chunk size provided consumers do not retain every chunk.

### Async streaming

PCD ASCII and all supported XYZ-family variants also expose asynchronous chunk streams backed by real asynchronous file reads.

XYZ-family example:

```csharp
await foreach (var chunk in XyzReader.ReadChunksAsync(
    "large-cloud.xyz",
    XyzVariant.XyzRgb,
    new PointCloudReadOptions
    {
        ParallelMode = ParallelReadMode.PreserveOrder,
        TextChunkSize = 4096
    }))
{
    await ProcessAsync(chunk.Data);
}
```

PCD ASCII exposes `PcdAsciiReader.OpenChunksAsync(...)`, which returns the parsed `PcdHeader` plus an `IAsyncEnumerable<PointCloudChunk>`:

```csharp
var stream = PcdAsciiReader.OpenChunksAsync("large-cloud.pcd");

await foreach (var chunk in stream.Chunks.WithCancellation(cancellationToken))
{
    await ProcessAsync(chunk.Data);
}
```

Async streaming supports cancellation and uses the same sequential, ordered-parallel, and unordered parsing policies as synchronous chunk streaming.

### Streaming chunk writers

PCD ASCII and all supported XYZ-family variants can also be written incrementally from `PointCloudChunk` sequences.

PCD binary supports the same bounded chunk model for both reads and writes, including async streaming. `binary_compressed` remains whole-payload I/O because the PCL-compatible encoding stores one compressed payload with declared compressed/uncompressed sizes.

Synchronous streaming writes accept `IEnumerable<PointCloudChunk>`; asynchronous streaming writes accept `IAsyncEnumerable<PointCloudChunk>`.

PCD example:

```csharp
var input = PcdAsciiReader.OpenChunksAsync("input.pcd");

await PcdAsciiWriter.WriteChunksAsync(
    "output.pcd",
    input.Header,
    input.Chunks);
```

XYZ example:

```csharp
await XyzWriter.WriteChunksAsync(
    "output.xyz",
    XyzReader.ReadChunksAsync("input.xyz"));
```

PCD streaming writers validate declared point count, schema compatibility, chunk sequence and offsets, and preserve organized-cloud ordering requirements.

### Writer modes

PCD ASCII, XYZ-family, and PTS writers expose explicit execution modes through the shared `PointCloudWriteOptions` API. PLY writers are sequential and do not accept these options:

- **Sequential** — default deterministic formatting and writing.
- **Ordered parallel** — formats point rows concurrently and emits them in original point order.
- **Deterministic output** — ordered-parallel writer output is required to match sequential output for the same input.

Example:

```csharp
XyzWriter.Write(
    "cloud.xyz",
    data,
    XyzVariant.XyzRgb,
    new PointCloudWriteOptions
    {
        ParallelMode = ParallelWriteMode.PreserveOrder,
        MaxDegreeOfParallelism = 8
    });
```

For PCD ASCII:

```csharp
PcdAsciiWriter.Write(
    "cloud.pcd",
    data,
    options: new PointCloudWriteOptions
    {
        ParallelMode = ParallelWriteMode.PreserveOrder
    });
```

PCD ASCII, the XYZ family, and PTS share text execution options. Chunk streaming is available for PCD ASCII, PCD binary, and the XYZ family; PLY and PTS currently use complete-cloud I/O.

See [Supported Formats and Variants](docs/supported-formats.md) for the detailed compatibility matrix and format-specific rules.

## Purpose

The primary goal of MPointCloud is to provide a high-performance .NET implementation of the PCD file format.

The library currently supports PCD 0.7, the XYZ family, PLY 1.0, and PTS. PCD remains the primary and preferred format of the library, while the format-independent core supports multiple interchange formats without coupling the data model to any one file format.

For the authoritative list of supported formats, variants, limitations, and conversion paths, see [Supported Formats and Variants](docs/supported-formats.md).

**PLY and PTS are implemented for the first public release. LAS, LAZ, PTX, and E57 are deferred** and will be prioritized only if adoption and user demand justify their additional complexity.

## PCD Compatibility

MPointCloud implements the **PCD (Point Cloud Data)** format based on the published PCD format specification and documentation associated with the open-source **Point Cloud Library (PCL)** project.

The PCD format is the native point-cloud data format used by PCL and supports flexible point fields, organized and unorganized point clouds, ASCII data, binary data, and compressed binary data.

MPointCloud is an independent .NET implementation.

It does **not** contain, wrap, require, or depend on the original C++ PCL implementation.

## High-Performance .NET Implementation

MPointCloud is designed specifically for modern .NET and makes use of performance-oriented .NET facilities where appropriate.

The implementation is intended to minimize allocation, parsing overhead, memory copying, and file I/O costs when working with large point-cloud datasets.

Depending on the operation and data representation, the library may use techniques such as:

- High-performance binary I/O
- Buffered and streaming file access
- Efficient memory representations
- Span-based parsing and data access
- Reduced-allocation processing
- Parallel processing where beneficial
- Parallel point-data loading
- Parallel point-data writing
- Optimized ASCII parsing
- Optimized binary reading and writing

The objective is to provide both the convenience expected from a managed .NET library and performance suitable for large engineering and visualization datasets.

Published comparisons and measurement conditions are documented in [Performance](docs/performance.md). The internal benchmark harness is not included in the public binary distribution. Measure the public APIs with representative files and hardware when evaluating your own workload.

## Performance at a Glance

MPointCloud includes benchmarked sequential, ordered-parallel, unordered-parallel, binary, and binary-compressed I/O paths.

On a representative 16-logical-processor .NET 10 run using 1,000,000 points:

- PCD ASCII sequential read: **2.44 Mpts/s**
- PCD ASCII ordered-parallel read: **6.12 Mpts/s**
- PCD binary read: **16.05 Mpts/s**
- PCD binary_compressed read: **17.03 Mpts/s**
- PCD binary write: **14.03 Mpts/s**

These numbers are comparative benchmark results, not universal performance guarantees. Actual throughput depends on factors such as CPU core count, single-core speed, memory bandwidth, storage performance, .NET runtime, dataset size/schema, and current system load. See [Performance](docs/performance.md) for methodology, hardware considerations, write comparisons, allocation data, and chunk-size tuning.

## Supported Formats

MPointCloud currently reads and writes:

- **PCD 0.7 ASCII**
- **PCD 0.7 binary**
- **PCD 0.7 binary_compressed (PCL-compatible LZF)**
- **Plain XYZ text (XYZ, XYZI, XYZRGB, XYZRGBA)**
- **PLY 1.0 (ASCII, binary little-endian, binary big-endian)**
- **PTS text (XYZI, XYZI + RGB)**

PCD support is schema-driven and includes arbitrary field names, supported PCD scalar types, `COUNT > 1`, organized/unorganized clouds, comments, `VIEWPOINT`, and conversion among ASCII, binary, and binary-compressed encodings.

XYZ-family support includes XYZ, XYZI, XYZRGB, and XYZRGBA. PCD ASCII, XYZ-family, and PTS readers support ordered and unordered parallel parsing through the shared read-options API; their writers support ordered parallel formatting. PLY I/O is sequential.

See [Supported Formats and Variants](docs/supported-formats.md) for the detailed compatibility matrix, supported variants, limitations, and planned additions. PLY-specific behavior is documented in [PLY Support](docs/ply.md), and the explicit scanner-text dialect is documented in [PTS Support](docs/pts.md).

## Standalone Library

MPointCloud has no dependency on MEngine.

It can therefore be used independently in applications such as:

- Engineering software
- Point-cloud processing tools
- Data converters
- Desktop applications
- Command-line utilities
- Visualization applications
- Research software
- Academic applications
- Server-side processing systems

Applications can use MPointCloud simply as a PCD reader/writer without using any MEngine functionality.

## MEngine Integration

MPointCloud 1.0 integrates with MEngine through the optional `Mengine.IO.MPointCloud` adapter. It reads ASCII, binary and binary_compressed directly from streams and converts results into MEngine-owned data types.

MPointCloud remains independent of MEngine. Applications may use it alone, register the optional MEngine provider, or supply their own providers.

See [MEngine Integration](docs/mengine-integration.md) for consumer examples, prerequisites, mapping limits, and what happens when the adapter is absent.

## Support and Feedback

Report bugs, request features, or suggest documentation corrections through [GitHub Issues](https://github.com/Zeniac/MPointCloud/issues). See [Support and Feedback](SUPPORT.md) for the information to include and the private contact route.

## Who uses MPointCloud?

If MPointCloud is useful in your application, we would like to hear about it. Use **Contact owners** on the [NuGet package page](https://www.nuget.org/packages/MPointCloud) to share your company/product name, an optional website link, and a short description of how you use the library.

Please state whether we may list the company/product name and link publicly. Usage reporting is voluntary and separate from required license attribution. Logos and testimonials require separate permission.

## Licensing and Source Availability

MPointCloud is **closed-source software**.

The compiled library may be used free of charge for:

- Personal use
- Academic use
- Research use
- Commercial use

The source code is not distributed as part of the public library.

Specific redistribution, modification, reverse-engineering, packaging, and sublicensing terms may be defined in the accompanying MPointCloud license.

## PCD Format and Third-Party Attribution

The PCD file format was developed as part of the **Point Cloud Library (PCL)** project.

MPointCloud is based on the publicly available PCD format documentation and interoperability requirements.

MPointCloud does not claim ownership of the PCD format, the PCL project, or PCL documentation.

The MPointCloud source code, architecture, optimizations, .NET implementation, APIs, tests, tooling, and related original software are independent works and remain the intellectual property of their respective copyright holder.

MPointCloud is not an official component of, and is not affiliated with or endorsed by, the Point Cloud Library project.

## Design Goals

MPointCloud is being developed around several core principles:

- **Fast** — suitable for large point-cloud datasets
- **Standalone** — no dependency on MEngine or another graphics framework
- **Interoperable** — compatible with established PCD data
- **Simple** — straightforward .NET API for reading and writing point clouds
- **Efficient** — designed to reduce unnecessary allocations and copies
- **Extensible** — able to evolve toward additional point-cloud formats
- **Engine-ready** — optimized for use with MEngine without being coupled to it

## Project Status

MPointCloud 1.0 is the first stable release, available through NuGet and standalone GitHub release archives.

The current implementation includes PCD 0.7 ASCII, binary, and binary-compressed read/write; PCD encoding conversion and official PCL interoperability fixtures; XYZ/XYZI/XYZRGB/XYZRGBA; PLY 1.0 ASCII and binary little/big-endian vertex I/O; PTS XYZI and XYZI+RGB; ordered and unordered parallel text readers where supported; ordered parallel text writers; bounded chunked I/O; synchronous streaming; and async `IAsyncEnumerable` streaming where supported.

PCD interoperability is covered by official-PCL ASCII, binary and binary_compressed fixtures. Benchmark-tuned defaults are 4,096 rows for text chunking and 16,384 points for binary streaming.

For current format status, see [Supported Formats and Variants](docs/supported-formats.md). For installation, examples, and performance guidance, see the [Documentation Index](docs/README.md) and [Samples](samples/README.md).

---

**MPointCloud**  
High-performance point-cloud data I/O for .NET.

