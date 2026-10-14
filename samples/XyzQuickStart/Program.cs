using MPointCloud.Core;
using MPointCloud.Formats.Xyz;
using MPointCloud.IO;

var schema = new PointSchema(
[
    new PointField("x", 8, PointFieldScalarType.FloatingPoint, 1, 0),
    new PointField("y", 8, PointFieldScalarType.FloatingPoint, 1, 8),
    new PointField("z", 8, PointFieldScalarType.FloatingPoint, 1, 16),
    new PointField("r", 1, PointFieldScalarType.UnsignedInteger, 1, 24),
    new PointField("g", 1, PointFieldScalarType.UnsignedInteger, 1, 25),
    new PointField("b", 1, PointFieldScalarType.UnsignedInteger, 1, 26)
]);

var data = new PointCloudData(
    schema,
    2,
    1,
[
    PointValue.FromFloating(1), PointValue.FromFloating(2), PointValue.FromFloating(3),
    PointValue.FromUnsigned(255), PointValue.FromUnsigned(0), PointValue.FromUnsigned(0),

    PointValue.FromFloating(4), PointValue.FromFloating(5), PointValue.FromFloating(6),
    PointValue.FromUnsigned(0), PointValue.FromUnsigned(128), PointValue.FromUnsigned(255)
]);

var path = Path.Combine(Environment.CurrentDirectory, "sample.xyz");

XyzWriter.Write(
    path,
    data,
    XyzVariant.XyzRgb,
    new PointCloudWriteOptions
    {
        ParallelMode = ParallelWriteMode.PreserveOrder
    });

var loaded = XyzReader.Read(
    path,
    XyzVariant.XyzRgb,
    new PointCloudReadOptions
    {
        ParallelMode = ParallelReadMode.PreserveOrder
    });

Console.WriteLine($"Wrote and reloaded {loaded.Count} XYZRGB points.");
Console.WriteLine($"File: {path}");
