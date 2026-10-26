# Mektral Integration

MPointCloud is a standalone library. Mektral (previously MEngine) consumes it through an optional point-cloud I/O adapter; MPointCloud does not depend on Mektral.

This guide uses MPointCloud 1.0 for PCD import through the optional Mektral adapter. The adapter converts library results into Mektral-owned data types.

## Requirements

The examples below run in an application that already references Mektral's IO layer and optional MPointCloud adapter. Those Mektral assemblies are acquired separately; they are not included in the MPointCloud NuGet package or standalone ZIP.

MPointCloud itself requires .NET 10. Installing it alone provides point-cloud I/O, not a renderer or viewer.

## Technical identifiers during the rename

Mektral is the product name. The current engine checkout still exposes the `Mengine.IO.MPointCloud` adapter assembly and `MEngine.IO` / `MEngine.IO.MPointCloud` namespaces used in the examples below. These are existing identifiers, not a dependency on a different engine. Use the identifiers supplied by your SDK build; the engine repository and technical identifier migration are coordinated separately.

## Register and use the provider

```csharp
using MEngine.IO;
using MEngine.IO.MPointCloud;

var registry = EngineeringDataProviders.CreateDefault();
registry.RegisterMPointCloudPcd();

if (!registry.TryResolveReader("scan.pcd", out var provider))
    throw new InvalidOperationException("No PCD provider is registered.");

using var input = File.OpenRead("scan.pcd");
var document = provider!.Read(input, "scan.pcd");
var cloud = document.PointClouds[0];
Console.WriteLine($"Imported points: {cloud.PointCount}");
```

For direct adapter use in a Mektral application:

```csharp
using var input = File.OpenRead("scan.pcd");
var cloud = new MPointCloudPcdReader().Read(input, "scan.pcd");
```

The adapter reads ASCII, binary, and binary_compressed through `PcdReader.Read(Stream)`, without temporary PCD files. Input starts at the current position and remains caller-owned. This complete-file path materializes the cloud; it does not implement bounded scene streaming or level of detail.

## Mapping and limits

- Mektral requires scalar `x`, `y`, and `z` fields and maps them to its canonical positions.
- Complete normal triplets map to normals; other supported numeric fields become attributes.
- The current adapter accepts attributes with 1-4 components and requires finite numeric channels. These are Mektral mapping limits: MPointCloud independently supports larger PCD COUNT fields and non-finite floating-point values.
- Organized dimensions, viewpoint, comments, and schema descriptions are retained as document metadata. Viewpoint is not automatically applied as a position transform.

## When the adapter is absent

The default Mektral registry can report `KnownProviderNotRegistered` for PCD input, with guidance to register the optional or a custom provider. This is provider-registration state, not a physical DLL check. Applications opting into the adapter must restore and deploy its dependencies; a missing dependency assembly is a deployment error.

Applications can register their own providers or construct Mektral point-cloud data without MPointCloud. Applications using MPointCloud alone require no Mektral assemblies.

## Independent library examples

The eight [standalone consumer samples](../samples/README.md) exercise MPointCloud without Mektral. In particular, `PcdStreamInput` reads all three PCD encodings from in-memory ZIP entries, and `PcdQuickStart` generates small binary and compressed PCD inputs suitable for trying the provider examples above.
