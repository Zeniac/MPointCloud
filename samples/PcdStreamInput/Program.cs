using System.IO.Compression;
using MPointCloud.Core;
using MPointCloud.Formats.Pcd;

var schema = new PointSchema(
[
    new("x", 4, PointFieldScalarType.FloatingPoint, 1, 0),
    new("y", 4, PointFieldScalarType.FloatingPoint, 1, 4),
    new("z", 4, PointFieldScalarType.FloatingPoint, 1, 8)
]);
var source = new PointCloudData(schema, 2, 1,
[
    PointValue.FromFloating(1), PointValue.FromFloating(2), PointValue.FromFloating(3),
    PointValue.FromFloating(4), PointValue.FromFloating(5), PointValue.FromFloating(6)
]);

// Build an archive in memory. No temporary PCD files are created.
using var archiveBytes = new MemoryStream();
using (var archive = new ZipArchive(archiveBytes, ZipArchiveMode.Create, leaveOpen: true))
{
    foreach (var encoding in Enum.GetValues<PcdDataEncoding>())
    {
        using var output = archive.CreateEntry($"{encoding}.pcd").Open();
        switch (encoding)
        {
            case PcdDataEncoding.Ascii: PcdAsciiWriter.Write(output, source); break;
            case PcdDataEncoding.Binary: PcdBinaryWriter.Write(output, source); break;
            case PcdDataEncoding.BinaryCompressed: PcdBinaryCompressedWriter.Write(output, source); break;
        }
    }
}

archiveBytes.Position = 0;
using var inputArchive = new ZipArchive(archiveBytes, ZipArchiveMode.Read, leaveOpen: true);
foreach (var entry in inputArchive.Entries)
{
    using var input = entry.Open();
    var (header, data) = PcdReader.Read(input);
    if (data.Count != source.Count || data.GetPointValues(1)[2].GetFloating() != 6)
        throw new InvalidDataException("ZIP-entry PCD read did not reproduce the source points.");
    if (!input.CanRead)
        throw new InvalidOperationException("The reader closed the caller's ZIP-entry stream.");
    Console.WriteLine($"{entry.Name}: {data.Count} points, {header.DataEncoding}, input left open.");
}
