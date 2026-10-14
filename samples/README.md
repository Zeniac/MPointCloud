# MPointCloud Samples

These projects demonstrate use of the public MPointCloud API.

They are intentionally small and should remain suitable for copying and adapting into consumer applications.

## Current samples

### PcdQuickStart

Creates a small PCD cloud, writes binary PCD, reads it back, and converts it to PCD binary_compressed.

```bash
dotnet run --project samples/PcdQuickStart/PcdQuickStart.csproj -c Release
```

### PcdStreaming

Creates a larger binary PCD and processes it through bounded binary chunks without materializing a second full-cloud copy.

```bash
dotnet run --project samples/PcdStreaming/PcdStreaming.csproj -c Release
```

### XyzQuickStart

Writes and reads XYZRGB using ordered-parallel text formatting/parsing.

```bash
dotnet run --project samples/XyzQuickStart/XyzQuickStart.csproj -c Release
```

### PcdAsyncStreaming

Demonstrates asynchronous bounded PCD ASCII streaming through `IAsyncEnumerable<PointCloudChunk>`.

```bash
dotnet run --project samples/PcdAsyncStreaming/PcdAsyncStreaming.csproj -c Release
```

### ArbitrarySchema

Creates a PCD schema with XYZ, intensity, and a COUNT 3 descriptor field, writes it as binary PCD, and inspects the schema and expanded scalar values after reading.

```bash
dotnet run --project samples/ArbitrarySchema/ArbitrarySchema.csproj -c Release
```

### PlyQuickStart

Creates a small point cloud, writes binary little-endian PLY, reads it back, and reports the resulting schema.

```bash
dotnet run --project samples/PlyQuickStart/PlyQuickStart.csproj -c Release
```

### PtsQuickStart

Creates a scanner-style XYZI+RGB point cloud, writes PTS using ordered-parallel formatting, reads it with ordered-parallel parsing, and reports the resulting schema.

```bash
dotnet run --project samples/PtsQuickStart/PtsQuickStart.csproj -c Release
```

## Release sample set

Before the first public release, the remaining focused sample is:

- MEngine provider integration, after the preview NuGet package is published

Public release samples should consume the NuGet/binary distribution rather than depend on proprietary implementation source.


## Development vs standalone use

Inside the private development repository, sample projects reference the MPointCloud project directly.

Inside the standalone binary release, the same sample projects automatically reference `lib/net10.0/MPointCloud.dll` instead. This keeps one sample source tree valid for both development CI and public binary distribution.
