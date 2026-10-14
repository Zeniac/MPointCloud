# Installation

MPointCloud is distributed as proprietary closed-source binaries under the MPointCloud Free Binary License.

The first public release is intended to be available through both NuGet and a standalone GitHub release archive.

## NuGet

For the current preview, specify the version explicitly:

```bash
dotnet add package MPointCloud --version 0.9.0-preview.1
```

## Samples

Runnable consumer samples are maintained in the public MPointCloud repository under `samples/`.

Current sample projects cover:

- PCD read/write and encoding conversion
- bounded PCD streaming
- async PCD streaming
- arbitrary-schema PCD data
- XYZRGB ordered-parallel I/O
- PLY read/write
- PTS read/write

After the preview package is published, users can clone the public repository and run the samples; the projects restore the pinned preview version from NuGet when the standalone DLL is absent. The standalone binary release also includes the same sample source, configured to reference the packaged `MPointCloud.dll`.

See [Samples](../samples/README.md) for the current list and commands.

## Standalone binary package

The GitHub release ZIP is intended for applications that do not want to acquire the library through NuGet.

The standalone package contains the compiled library, XML API documentation, license, notices, public documentation, and sample source.

A typical release layout is:

```text
MPointCloud-<version>/
  lib/
    net10.0/
      MPointCloud.dll
      MPointCloud.xml
  docs/
  samples/
  README.md
  LICENSE
  THIRD-PARTY-NOTICES.md
  CHANGELOG.md
```

Reference `MPointCloud.dll` from the appropriate application project when using the standalone package.

## Runtime target

The current development target is:

- .NET 10 (`net10.0`)

Additional target frameworks may be considered later if real adoption requires them.

## License

Use of the compiled library is governed by the MPointCloud Free Binary License included with every authorized package.

The library is free for permitted personal, academic, research, and commercial use subject to the license terms and attribution requirements.

## Source availability

The public distribution is binary-only. The proprietary development repository is not part of the public package.

Public samples contain only consumer code that demonstrates the public MPointCloud API.
