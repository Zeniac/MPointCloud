using MPointCloud.Core;
using MPointCloud.Formats.Pcd;

var schema = new PointSchema(
[
    new PointField("x", 4, PointFieldScalarType.FloatingPoint, 1, 0),
    new PointField("y", 4, PointFieldScalarType.FloatingPoint, 1, 4),
    new PointField("z", 4, PointFieldScalarType.FloatingPoint, 1, 8)
]);

var data = new PointCloudData(
    schema,
    width: 3,
    height: 1,
[
    PointValue.FromFloating(1), PointValue.FromFloating(2), PointValue.FromFloating(3),
    PointValue.FromFloating(4), PointValue.FromFloating(5), PointValue.FromFloating(6),
    PointValue.FromFloating(7), PointValue.FromFloating(8), PointValue.FromFloating(9)
]);

var binaryPath = Path.Combine(Environment.CurrentDirectory, "sample-binary.pcd");
var compressedPath = Path.Combine(Environment.CurrentDirectory, "sample-compressed.pcd");

PcdWriter.Write(
    binaryPath,
    data,
    PcdDataEncoding.Binary,
    comments: ["Created by the MPointCloud PCD sample"]);

var (header, loaded) = PcdReader.Read(binaryPath);

Console.WriteLine($"Loaded {loaded.Count} points from {header.DataEncoding} PCD.");

PcdConverter.Convert(
    binaryPath,
    compressedPath,
    PcdDataEncoding.BinaryCompressed);

var (compressedHeader, compressed) = PcdReader.Read(compressedPath);

Console.WriteLine($"Converted to {compressedHeader.DataEncoding}; points: {compressed.Count}");
Console.WriteLine($"Files: {binaryPath}, {compressedPath}");
