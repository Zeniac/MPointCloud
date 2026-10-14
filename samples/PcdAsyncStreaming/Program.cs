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
for (var i = 0; i < 25_000; i++)
{
    values.Add(PointValue.FromFloating(i));
    values.Add(PointValue.FromFloating(i + 1));
    values.Add(PointValue.FromFloating(i + 2));
}

var source = new PointCloudData(schema, 25_000, 1, values);
var path = Path.Combine(Environment.CurrentDirectory, "async-source.pcd");

PcdWriter.Write(path, source, PcdDataEncoding.Ascii);

var stream = PcdAsciiReader.OpenChunksAsync(
    path,
    new PointCloudReadOptions
    {
        ParallelMode = ParallelReadMode.PreserveOrder,
        TextChunkSize = 4096
    });

var processed = 0;

await foreach (var chunk in stream.Chunks)
{
    processed += chunk.Data.Count;

    // Await asynchronous processing, persistence, upload, or rendering here.
    await Task.Yield();
}

Console.WriteLine($"Header points: {stream.Header.Points}");
Console.WriteLine($"Processed asynchronously: {processed}");
Console.WriteLine($"File: {path}");
