# Mektral Integration Recipe

This is a consumer integration example for applications that already have the Mektral IO layer and optional `Mengine.IO.MPointCloud` adapter. It is not a ninth standalone MPointCloud sample project: Mektral assemblies are acquired separately.

See [Mektral Integration](../../docs/mektral-integration.md) for the provider-registration and direct-reader examples, prerequisites, stream ownership, and mapping limits.

To generate a small test input using only MPointCloud, run from this repository or the extracted standalone release:

```bash
dotnet run --project samples/PcdQuickStart/PcdQuickStart.csproj -c Release
```

This writes `sample-binary.pcd` and `sample-compressed.pcd` with the same three XYZ points: (1,2,3), (4,5,6), and (7,8,9). In the separately available Mektral application, register the optional provider and read either file as shown in the guide. Both imports should return three canonical positions with those coordinates.

Use the published `MPointCloud` version `1.0.1` package when building the adapter for these examples.

The guide explains the existing adapter/namespace identifiers used during the Mektral product rename.
