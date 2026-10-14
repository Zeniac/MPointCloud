# Getting Started

MPointCloud is a standalone .NET point-cloud I/O library.

It can be used independently or as the preferred point-cloud data provider for MEngine.

## Install

```bash
dotnet add package MPointCloud --version 0.9.0-preview.1
```

Then import the format namespace you need.

## Read PCD

```csharp
using MPointCloud.Formats.Pcd;

var (header, data) = PcdReader.Read("cloud.pcd");

Console.WriteLine($"Points: {data.Count}");
Console.WriteLine($"Fields: {string.Join(", ", data.Schema.Fields.Select(f => f.Name))}");
```

`PcdReader.Read(...)` automatically dispatches to ASCII, binary, or binary_compressed based on the file header. Specialized readers remain available for streaming and format-specific control.

## Write PCD

```csharp
PcdWriter.Write(
    "cloud.pcd",
    data,
    PcdDataEncoding.Binary);
```

`PcdWriter.Write(...)` selects ASCII, binary, or binary_compressed from the requested encoding. Specialized writers remain available when you need streaming or ASCII parallel-format options.

## Read XYZ

```csharp
using MPointCloud.Formats.Xyz;

var data = XyzReader.Read(
    "cloud.xyz",
    XyzVariant.XyzRgb);
```

## Read PLY

```csharp
using MPointCloud.Formats.Ply;

var (header, data) = PlyReader.Read("cloud.ply");

Console.WriteLine(header.DataEncoding);
Console.WriteLine(data.Count);
```

## Read PTS

```csharp
using MPointCloud.Formats.Pts;

var data = PtsReader.Read(
    "scan.pts",
    PtsVariant.XyziRgb);
```

## Ordered parallel text reading

For PCD ASCII, XYZ-family, and PTS readers (PLY reading is sequential):

```csharp
using MPointCloud.IO;

var options = new PointCloudReadOptions
{
    ParallelMode = ParallelReadMode.PreserveOrder
};

var data = XyzReader.Read(
    "cloud.xyz",
    XyzVariant.XyzRgb,
    options);
```

Ordered parallel parsing preserves source point order while allowing parsing work to execute concurrently. PTS uses the same options through `PtsReader.Read(path, variant, options)`.

## Streaming large point clouds

PCD ASCII, PCD binary, and the XYZ family expose bounded streaming APIs where supported.

```csharp
PcdHeader header = PcdAsciiReader.ReadChunks(
    "large-cloud.pcd",
    chunk =>
    {
        Process(chunk.Data);
    });
```

This avoids materializing the full point cloud as one in-memory object.

## Where to go next

- [Supported Formats](supported-formats.md)
- [Performance](performance.md)
- [PLY Support](ply.md)
- [PTS Support](pts.md)
- [Installation](installation.md)
- repository samples under `samples/`
