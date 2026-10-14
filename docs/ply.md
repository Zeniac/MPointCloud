# PLY Support

MPointCloud supports point-cloud-oriented PLY 1.0 files.

## Supported encodings

- ASCII
- binary_little_endian
- binary_big_endian

## Supported vertex property types

MPointCloud maps PLY scalar vertex properties directly into the format-independent `PointSchema`.

| PLY type | MPointCloud scalar type | Size |
| --- | --- | ---: |
| char / int8 | SignedInteger | 1 |
| uchar / uint8 | UnsignedInteger | 1 |
| short / int16 | SignedInteger | 2 |
| ushort / uint16 | UnsignedInteger | 2 |
| int / int32 | SignedInteger | 4 |
| uint / uint32 | UnsignedInteger | 4 |
| float / float32 | FloatingPoint | 4 |
| double / float64 | FloatingPoint | 8 |

Property names are preserved exactly and property order defines the point-record schema.

Binary Float32 and Float64 values preserve their exact IEEE 754 bit patterns.

## Point-cloud scope

PLY is also commonly used for polygon meshes. MPointCloud intentionally treats PLY as a point-cloud source and destination.

Current behavior:

- the `vertex` element must be the first declared data element;
- scalar properties on the vertex element are loaded into `PointCloudData`;
- list-valued properties on the vertex element are rejected;
- elements declared after the vertex block, such as `face`, are ignored;
- PLY written by MPointCloud contains a vertex element only and does not emit mesh topology.

This allows common mesh PLY files to be used as vertex/point-cloud sources without coupling MPointCloud to mesh processing.

## Reading

```csharp
using MPointCloud.Formats.Ply;

var (header, data) = PlyReader.Read("cloud.ply");

Console.WriteLine($"Vertices: {data.Count}");

foreach (var field in data.Schema.Fields)
    Console.WriteLine($"{field.Name}: {field.ScalarType}, {field.Size} bytes");
```

## Writing

ASCII:

```csharp
PlyWriter.Write(
    "cloud.ply",
    data,
    PlyDataEncoding.Ascii);
```

Little-endian binary:

```csharp
PlyWriter.Write(
    "cloud-binary.ply",
    data,
    PlyDataEncoding.BinaryLittleEndian);
```

Big-endian binary:

```csharp
PlyWriter.Write(
    "cloud-big-endian.ply",
    data,
    PlyDataEncoding.BinaryBigEndian);
```

## Notes

PLY does not encode explicit byte offsets between scalar properties. MPointCloud therefore requires a tightly packed schema when writing PLY.

PLY list properties are appropriate for mesh topology and variable-length data. They are not represented as point fields in the current MPointCloud PLY adapter.
