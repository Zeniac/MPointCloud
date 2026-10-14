using MPointCloud.Core;
using MPointCloud.Formats.Pcd;
using MPointCloud.IO;

var schema = new PointSchema(
[
    new PointField("x", 4, PointFieldScalarType.FloatingPoint, 1, 0),
    new PointField("y", 4, PointFieldScalarType.FloatingPoint, 1, 4),
    new PointField("z", 4, PointFieldScalarType.FloatingPoint, 1, 8)
]);

var values = new List<PointValue>();
for (var i = 0; i < 50_000; i++)
{
    values.Add(PointValue.FromFloating(i));
    values.Add(PointValue.FromFloating(i + 1));
    values.Add(PointValue.FromFloating(i + 2));
}

var data = new PointCloudData(schema, 50_000, 1, values);
var path = Path.Combine(Environment.CurrentDirectory, "streaming-source.pcd");

PcdBinaryWriter.Write(path, data);

var processed = 0;
var chunks = 0;

var header = PcdBinaryReader.ReadChunks(
    path,
    chunk =>
    {
        processed += chunk.Data.Count;
        chunks++;

        // Process, transform, index, upload, or render chunk.Data here.
    },
    new PointCloudReadOptions
    {
        BinaryChunkSize = 16_384
    });

Console.WriteLine($"Source points: {header.Points}");
Console.WriteLine($"Processed points: {processed}");
Console.WriteLine($"Chunks: {chunks}");
Console.WriteLine($"File: {path}");
