using MPointCloud.Core;
using MPointCloud.Formats.Ply;

var schema = new PointSchema(
[
    new PointField("x", 4, PointFieldScalarType.FloatingPoint, 1, 0),
    new PointField("y", 4, PointFieldScalarType.FloatingPoint, 1, 4),
    new PointField("z", 4, PointFieldScalarType.FloatingPoint, 1, 8),
    new PointField("red", 1, PointFieldScalarType.UnsignedInteger, 1, 12),
    new PointField("green", 1, PointFieldScalarType.UnsignedInteger, 1, 13),
    new PointField("blue", 1, PointFieldScalarType.UnsignedInteger, 1, 14)
]);

var data = new PointCloudData(
    schema,
    width: 2,
    height: 1,
[
    PointValue.FromFloating(1), PointValue.FromFloating(2), PointValue.FromFloating(3),
    PointValue.FromUnsigned(255), PointValue.FromUnsigned(0), PointValue.FromUnsigned(0),

    PointValue.FromFloating(4), PointValue.FromFloating(5), PointValue.FromFloating(6),
    PointValue.FromUnsigned(0), PointValue.FromUnsigned(128), PointValue.FromUnsigned(255)
]);

var outputPath = Path.Combine(Environment.CurrentDirectory, "sample.ply");

PlyWriter.Write(
    outputPath,
    data,
    PlyDataEncoding.BinaryLittleEndian,
    comments: ["Created by the MPointCloud PLY sample"]);

var (header, loaded) = PlyReader.Read(outputPath);

Console.WriteLine($"Wrote and reloaded {loaded.Count} points.");
Console.WriteLine($"Encoding: {header.DataEncoding}");
Console.WriteLine($"Schema: {string.Join(", ", loaded.Schema.Fields.Select(static field => field.Name))}");
Console.WriteLine($"File: {outputPath}");
