using MPointCloud.Core;
using MPointCloud.Formats.Pts;
using MPointCloud.IO;

var schema = new PointSchema(
[
    new PointField("x", 8, PointFieldScalarType.FloatingPoint, 1, 0),
    new PointField("y", 8, PointFieldScalarType.FloatingPoint, 1, 8),
    new PointField("z", 8, PointFieldScalarType.FloatingPoint, 1, 16),
    new PointField("intensity", 2, PointFieldScalarType.SignedInteger, 1, 24),
    new PointField("r", 1, PointFieldScalarType.UnsignedInteger, 1, 26),
    new PointField("g", 1, PointFieldScalarType.UnsignedInteger, 1, 27),
    new PointField("b", 1, PointFieldScalarType.UnsignedInteger, 1, 28)
]);

var data = new PointCloudData(
    schema,
    width: 2,
    height: 1,
[
    PointValue.FromFloating(1), PointValue.FromFloating(2), PointValue.FromFloating(3),
    PointValue.FromSigned(-512), PointValue.FromUnsigned(255), PointValue.FromUnsigned(32), PointValue.FromUnsigned(16),

    PointValue.FromFloating(4), PointValue.FromFloating(5), PointValue.FromFloating(6),
    PointValue.FromSigned(1024), PointValue.FromUnsigned(10), PointValue.FromUnsigned(200), PointValue.FromUnsigned(255)
]);

var outputPath = Path.Combine(Environment.CurrentDirectory, "sample.pts");

PtsWriter.Write(
    outputPath,
    data,
    PtsVariant.XyziRgb,
    new PointCloudWriteOptions
    {
        ParallelMode = ParallelWriteMode.PreserveOrder
    });

var loaded = PtsReader.Read(
    outputPath,
    PtsVariant.XyziRgb,
    new PointCloudReadOptions
    {
        ParallelMode = ParallelReadMode.PreserveOrder
    });

Console.WriteLine($"Wrote and reloaded {loaded.Count} points.");
Console.WriteLine($"Schema: {string.Join(", ", loaded.Schema.Fields.Select(static field => field.Name))}");
Console.WriteLine($"File: {outputPath}");
