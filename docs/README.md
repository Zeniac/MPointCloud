# MPointCloud Documentation

## Start here

- [Installation](installation.md)
- [Getting Started](getting-started.md)
- [Supported Formats](supported-formats.md)
- [Performance](performance.md)
- [Support and Feedback](../SUPPORT.md)

## Format guides

- [PLY Support](ply.md)
- [PTS Support](pts.md)

PCD usage is introduced in [Getting Started](getting-started.md) and detailed format support is summarized in [Supported Formats](supported-formats.md).

## Samples

Runnable sample projects are maintained under the repository `samples/` directory.

The public sample set covers:

- PCD read/write and encoding conversion
- complete-cloud PCD input from memory and ZIP-entry streams
- bounded PCD streaming
- async PCD streaming
- XYZ-family I/O
- PLY
- PTS
- arbitrary schema inspection

## Mektral integration

See [Mektral Integration](mektral-integration.md) and the [consumer recipe](../samples/MektralIntegration/README.md) for registration, direct-reader usage, mapping limits, and optional-provider behavior. The eight standalone sample projects do not require Mektral.

