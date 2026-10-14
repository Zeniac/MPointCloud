# Supported Formats and Variants

This document is the user-facing reference for file formats and variants currently supported by MPointCloud.

It should be updated whenever format support changes.

## Support summary

| Format | Variant / Encoding | Read | Write | Notes |
| --- | --- | --- | --- | --- |
| PCD 0.7 | ASCII | Yes | Yes | Arbitrary fields, COUNT > 1, organized/unorganized clouds, NaN handling. |
| PCD 0.7 | Binary | Yes | Yes | Schema-driven binary I/O using the declared field layout. |
| PCD 0.7 | Binary compressed | Yes | Yes | PCL-compatible field-major layout and LZF compression. |
| XYZ | Plain XYZ text | Yes | Yes | x y z |
| XYZ | XYZI | Yes | Yes | x y z intensity; intensity is Float64. |
| XYZ | XYZRGB | Yes | Yes | x y z r g b; RGB channels are UInt8 (0-255). |
| XYZ | XYZRGBA | Yes | Yes | x y z r g b a; RGBA channels are UInt8 (0-255). |
| PLY 1.0 | ASCII | Yes | Yes | Scalar vertex properties; later mesh elements are ignored. |
| PLY 1.0 | Binary little-endian | Yes | Yes | Scalar vertex properties with exact floating-point bit preservation. |
| PLY 1.0 | Binary big-endian | Yes | Yes | Scalar vertex properties with exact floating-point bit preservation. |
| PTS | XYZI | Yes | Yes | Required point-count header; Float64 intensity on read. |
| PTS | XYZI + RGB | Yes | Yes | Required point-count header; RGB UInt8. |

## Reader and writer execution modes

MPointCloud exposes a shared execution model for supported text formats so applications can choose the appropriate balance between determinism and throughput.

### Read modes

Parallel text readers process bounded row chunks rather than buffering the full text body. `PointCloudReadOptions.TextChunkSize` controls the maximum rows buffered per chunk and defaults to 4,096.

`PointCloudReadOptions.ParallelMode` supports:

- `Disabled` — sequential parsing; this is the default.
- `PreserveOrder` — parse rows concurrently while preserving original point order.
- `Unordered` — parse rows concurrently without preserving original source order.

`MaxDegreeOfParallelism` can be used to cap worker concurrency.

For organized PCD clouds, unordered parsing is rejected unless `AllowUnorderedOrganizedClouds` is explicitly enabled. If enabled, the returned cloud is converted to unorganized dimensions because its original grid topology can no longer be considered valid.

These text parallel-read modes currently apply to:

- PCD 0.7 ASCII
- XYZ
- XYZI
- XYZRGB
- XYZRGBA

PCD binary supports bounded synchronous and asynchronous chunk streaming, but it does not currently use the text-oriented ordered/unordered parallel parsing modes.

### Streaming chunk APIs

PCD ASCII, PCD binary, and all currently supported XYZ-family variants expose synchronous chunk readers.

The callback receives a `PointCloudChunk` with:

- `SequenceIndex`
- `SourcePointOffset`
- `IsOrderPreserved`
- `Data`

The full source cloud does not need to be materialized as one `PointCloudData` instance. Each chunk is represented as an unorganized cloud because a partial batch does not, by itself, represent complete organized topology.

For PCD, the original full-cloud metadata and dimensions remain available from the `PcdHeader` returned by `PcdAsciiReader.ReadChunks`.

Streaming callbacks support the same sequential, ordered-parallel, and unordered parsing modes as normal text reads.

### Async streaming APIs

PCD ASCII and all currently supported XYZ-family variants also expose asynchronous chunk streaming backed by asynchronous file reads.

PCD binary also supports bounded synchronous and asynchronous chunk streaming for both reads and writes. `PointCloudReadOptions.BinaryChunkSize` controls the maximum points per binary read chunk and defaults to 16,384, based on the representative Release-mode benchmark recorded in the benchmark results.

PCD `binary_compressed` does not currently expose true bounded streaming because the PCL-compatible format stores one compressed payload preceded by total compressed and uncompressed sizes.

- XYZ-family readers expose `IAsyncEnumerable<PointCloudChunk>` directly through `ReadChunksAsync`.
- PCD ASCII exposes `OpenChunksAsync`, returning a `PcdChunkStream` containing the source `PcdHeader` and an `IAsyncEnumerable<PointCloudChunk>`.
- Cancellation is supported through the async enumerable cancellation token.
- CPU parsing inside each chunk continues to honor the selected sequential, ordered-parallel, or unordered mode.

### Streaming write APIs

PCD ASCII, PCD binary, and all currently supported XYZ-family variants support incremental chunk-fed output.

- Synchronous writers consume `IEnumerable<PointCloudChunk>`.
- Asynchronous writers consume `IAsyncEnumerable<PointCloudChunk>`.
- Ordered-parallel row formatting remains available inside each chunk.
- PCD streaming output validates schema compatibility, chunk sequence, source offsets, declared point count, and organized-cloud ordering.

This allows direct streaming pipelines such as reader -> transform -> writer without constructing one complete point cloud in memory.

### Write modes

`PointCloudWriteOptions.ParallelMode` supports:

- `Disabled` — sequential formatting; this is the default.
- `PreserveOrder` — format point rows concurrently, then emit them in original point order.

Ordered parallel writing currently applies to:

- PCD 0.7 ASCII
- XYZ
- XYZI
- XYZRGB
- XYZRGBA

Parallel writer output is required to match sequential writer output for the same input.

### Binary formats

PCD binary and binary_compressed already operate on binary buffers rather than text rows, so the text-oriented parallel parse/format modes above are not applied to them.

## PCD 0.7

MPointCloud targets practical format-complete support for PCD 0.7.

### Supported header features

- VERSION 0.7
- FIELDS
- SIZE
- TYPE
- COUNT
- WIDTH
- HEIGHT
- VIEWPOINT
- POINTS
- DATA
- header comments beginning with `#`
- omitted COUNT on read, defaulting each field count to 1
- arbitrary field names
- COUNT values greater than 1
- organized and unorganized clouds

### Supported scalar field types

| TYPE | Meaning | Supported SIZE values |
| --- | --- | --- |
| I | Signed integer | 1, 2, 4 bytes |
| U | Unsigned integer | 1, 2, 4 bytes |
| F | Floating point | 4, 8 bytes |

### Supported PCD encodings

#### ASCII

Read and write support includes:

- arbitrary schemas
- COUNT > 1 fields
- integer and floating-point values
- NaN parsing/writing
- organized-cloud dimensions

#### Binary

Read and write support includes:

- arbitrary schemas
- all supported PCD scalar sizes/types
- COUNT > 1 fields
- organized-cloud dimensions
- exact body-size validation

#### Binary compressed

Read and write support includes:

- PCL-compatible field-major layout
- LZF compression/decompression
- 32-bit compressed/uncompressed size prefixes
- COUNT > 1 fields
- organized-cloud dimensions
- conversion to and from ASCII and binary

For PCL compatibility, fields named `_` are treated as padding when writing `binary_compressed` and are not preserved in the compressed output schema.

### PCD conversion

MPointCloud currently supports conversion among:

- ASCII -> Binary
- ASCII -> Binary compressed
- Binary -> ASCII
- Binary -> Binary compressed
- Binary compressed -> ASCII
- Binary compressed -> Binary

Conversion preserves the logical point data, schema, dimensions, viewpoint, and comments where the destination encoding can represent them.

## PTS

MPointCloud supports an explicit scanner-style PTS dialect rather than inferring arbitrary columns.

Supported layouts:

- `PtsVariant.Xyzi` -> x y z intensity
- `PtsVariant.XyziRgb` -> x y z intensity r g b

The first non-empty line must contain the declared point count.

Coordinates are read as Float64. Intensity is read as Float64 because exporter conventions vary; writers accept signed integer, unsigned integer, or floating-point intensity fields. RGB channels are UInt8.

PTS supports sequential, ordered-parallel, and unordered-parallel reads, plus sequential and ordered-parallel writes. Streaming APIs are not currently exposed because the required count header must be known before output begins.

See [PTS Support](pts.md) for the exact dialect and examples.

## PLY

MPointCloud supports PLY 1.0 as a point-cloud interchange format.

Supported:

- ASCII
- binary little-endian
- binary big-endian
- all standard scalar PLY property widths
- common scalar type aliases such as `float32`, `uint8`, and `int16`
- comments
- exact binary Float32/Float64 bit preservation
- common mesh PLY files where the vertex element is first and topology elements follow it

Current scope:

- the vertex element must be the first declared data element;
- scalar vertex properties are supported;
- list properties on the vertex element are rejected;
- later elements such as faces are ignored;
- MPointCloud writes vertex-only PLY files.

See [PLY Support](ply.md) for details and examples.

## XYZ

MPointCloud supports a deliberately strict plain XYZ variant.

### Supported XYZ syntax

Each data row must contain exactly:

`X Y Z`

Coordinates are whitespace-separated. Tabs and multiple spaces are accepted.

Example:

```text
1.0 2.0 3.0
4.5 5.5 6.5
```

The reader also accepts:

- blank lines
- lines beginning with `#` as comments

XYZ files are read as unorganized clouds with:

- WIDTH = point count
- HEIGHT = 1
- x, y, z stored as 64-bit floating-point fields

### XYZ writer behavior

The writer locates fields named `x`, `y`, and `z` case-insensitively.

The input schema may contain other fields or place x/y/z in a different order; only x, y, and z are written.

### Supported XYZ-family variants

MPointCloud supports explicit variants selected by the caller:

- `XyzVariant.Xyz` -> x y z
- `XyzVariant.Xyzi` -> x y z intensity
- `XyzVariant.XyzRgb` -> x y z r g b
- `XyzVariant.XyzRgba` -> x y z r g b a

Coordinates and intensity are stored as Float64. Color channels are UInt8 and must be in the range 0-255.

The reader does not infer a variant from column count; callers select the expected layout explicitly.

### Not currently treated as supported XYZ-family variants

MPointCloud does not currently guess or support variants such as:

- comma-separated XYZ
- files with arbitrary additional unnamed columns
- locale-dependent decimal separators

These may be added later as explicit variants/options rather than inferred heuristically.

## Planned formats

The architecture is intended to support additional point-cloud formats without coupling the core model to PCD.

### First public release formats

- **PLY** — implemented with ASCII, binary little-endian, and binary big-endian scalar vertex support.
- **PTS** — implemented as a deliberately documented scanner-text dialect with XYZI and XYZI+RGB variants.

### Later / demand-driven formats

- **LAS** — deferred because of its larger specification and geospatial metadata surface.
- **LAZ** — deferred with LAS because compressed LAS adds additional implementation and interoperability complexity.
- additional explicit XYZ-family variants may be added when there is a concrete interoperability need.

LAS/LAZ are not release blockers for the first public version. Their implementation should be driven by adoption, user demand, and the value they add relative to development cost.

## Performance extensions

Parallel point parsing/writing is an MPointCloud capability rather than a file-format variant.

Planned/ongoing work includes:

- ordered parallel ASCII PCD reads — implemented
- unordered parallel ASCII PCD reads — implemented; organized clouds require explicit opt-in and are returned unorganized
- ordered parallel XYZ-family reads — implemented
- unordered parallel XYZ-family reads — implemented
- ordered parallel ASCII PCD writes — implemented
- ordered parallel XYZ-family writes — implemented
- bounded chunked parallel text reads — implemented
- synchronous callback streaming APIs — implemented
- async `IAsyncEnumerable` chunk streaming reads — implemented
- synchronous chunk streaming writes — implemented
- async `IAsyncEnumerable` chunk streaming writes — implemented
- additional allocation and buffer optimizations

These performance modes must not silently change file-format semantics.

## Interoperability testing

MPointCloud uses:

- generated deterministic fixtures
- official PCL reference PCD fixtures
- user-provided regression fixtures where redistribution is safe

See also:

- `pcd-format-coverage.md` for detailed PCD implementation tracking
- `legacy-reference-retirement.md` for the old-reference retirement checklist
