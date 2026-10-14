# PTS Support

MPointCloud supports a deliberately defined PTS text dialect for scanner-style point interchange.

PTS is widely used but not governed by one universally enforced formal specification. Exporters differ, especially in intensity range and optional columns. MPointCloud therefore uses explicit variants instead of guessing the layout from each row.

## File structure

The first non-empty line of a supported PTS file contains a single non-negative integer point count. Blank lines are ignored; comment lines and comma-separated columns are not supported. Numeric parsing and formatting use invariant culture.

Each following non-empty row contains exactly one point.

Supported variants:

### XYZI

```text
<count>
x y z intensity
x y z intensity
...
```

### XYZI + RGB

```text
<count>
x y z intensity r g b
x y z intensity r g b
...
```

## Data types

When reading:

- x, y, z -> Float64
- intensity -> Float64
- r, g, b -> UInt8

Intensity is intentionally parsed as Float64 because real exporters use different conventions, including integer ranges, negative scanner values, normalized 0..1 values, and positive integer ranges.

When writing:

- x, y, z must be floating-point fields;
- intensity may be signed integer, unsigned integer, or floating-point;
- r, g, b must be UInt8 fields in the range 0..255.

Required fields have COUNT 1 and are located by name case-insensitively. Extra input fields are not written. PTS reads return an unorganized cloud; PTS output does not preserve organized dimensions or PCD viewpoint metadata.

## Count validation

The first-line count is treated as authoritative.

MPointCloud rejects:

- files that end before the declared number of points;
- files containing additional non-empty point rows after the declared count;
- rows with the wrong number of columns;
- invalid numeric values;
- RGB values outside 0..255.

## Reading

```csharp
using MPointCloud.Formats.Pts;

var data = PtsReader.Read(
    "scan.pts",
    PtsVariant.XyziRgb);

Console.WriteLine($"Points: {data.Count}");
```

Ordered parallel parsing:

```csharp
using MPointCloud.IO;

var data = PtsReader.Read(
    "scan.pts",
    PtsVariant.XyziRgb,
    new PointCloudReadOptions
    {
        ParallelMode = ParallelReadMode.PreserveOrder
    });
```

Unordered parallel parsing is also available when source point order is not required.

## Writing

```csharp
PtsWriter.Write(
    "scan.pts",
    data,
    PtsVariant.XyziRgb);
```

Ordered parallel formatting:

```csharp
PtsWriter.Write(
    "scan.pts",
    data,
    PtsVariant.XyziRgb,
    new PointCloudWriteOptions
    {
        ParallelMode = ParallelWriteMode.PreserveOrder
    });
```

## Scope

The default variant is `PtsVariant.Xyzi`. MPointCloud does not currently infer arbitrary PTS column layouts. Chunk streaming and async APIs are not currently implemented.

The explicit supported layouts are:

- `PtsVariant.Xyzi`
- `PtsVariant.XyziRgb`

This is intentional. PTS files with additional normals, timestamps, scanner-specific columns, or other extensions should be handled through a future explicit variant rather than ambiguous column-count guessing.
