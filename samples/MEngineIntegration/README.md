# MEngine Integration Recipe

This is a consumer integration example for applications that already have the MEngine IO layer and optional `Mengine.IO.MPointCloud` adapter. It is not a ninth standalone MPointCloud sample project: MEngine assemblies are acquired separately.

See [MEngine Integration](../../docs/mengine-integration.md) for the provider-registration and direct-reader examples, prerequisites, stream ownership, and mapping limits.

To generate a small test input using only MPointCloud, run from this repository or the extracted standalone release:

```bash
dotnet run --project samples/PcdQuickStart/PcdQuickStart.csproj -c Release
```

This writes `sample-binary.pcd` and `sample-compressed.pcd` with the same three XYZ points: (1,2,3), (4,5,6), and (7,8,9). In the separately available MEngine application, register the optional provider and read either file as shown in the guide. Both imports should return three canonical positions with those coordinates.

The completed initial MEngine integration used preview.2. After publication, update the adapter's MPointCloud package reference to `1.0.0` and repeat the import smoke test using the published package.
