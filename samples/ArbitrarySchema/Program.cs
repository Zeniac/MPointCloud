using MPointCloud.Core;
using MPointCloud.Formats.Pcd;

var schema = new PointSchema(
[
    new PointField("x", 4, PointFieldScalarType.FloatingPoint, 1, 0),
    new PointField("y", 4, PointFieldScalarType.FloatingPoint, 1, 4),
    new PointField("z", 4, PointFieldScalarType.FloatingPoint, 1, 8),
    new PointField("intensity", 2, PointFieldScalarType.UnsignedInteger, 1, 12),
    new PointField("descriptor", 4, PointFieldScalarType.FloatingPoint, 3, 14)
]);

var data = new PointCloudData(
    schema,
    width: 2,
    height: 1,
[
    PointValue.FromFloating(1), PointValue.FromFloating(2), PointValue.FromFloating(3),
    PointValue.FromUnsigned(100),
    PointValue.FromFloating(0.1), PointValue.FromFloating(0.2), PointValue.FromFloating(0.3),

    PointValue.FromFloating(4), PointValue.FromFloating(5), PointValue.FromFloating(6),
    PointValue.FromUnsigned(200),
    PointValue.FromFloating(1.1), PointValue.FromFloating(1.2), PointValue.FromFloating(1.3)
]);

var path = Path.Combine(Environment.CurrentDirectory, "arbitrary-schema.pcd");

PcdWriter.Write(path, data, PcdDataEncoding.Binary);

var (header, loaded) = PcdReader.Read(path);

Console.WriteLine($"Fields ({header.Schema.Fields.Count}):");

foreach (var field in header.Schema.Fields)
{
    Console.WriteLine(
        $"  {field.Name}: {field.ScalarType}, size={field.Size}, count={field.Count}, offset={field.Offset}");
}

var firstPoint = loaded.GetPointValues(0);
Console.WriteLine($"Scalars per point: {loaded.ScalarsPerPoint}");
Console.WriteLine($"First descriptor: {firstPoint[4].GetFloating()}, {firstPoint[5].GetFloating()}, {firstPoint[6].GetFloating()}");
Console.WriteLine($"File: {path}");
