# Changelog

All notable public changes to MPointCloud will be documented in this file.

MPointCloud follows semantic versioning for public releases.

## [Unreleased]

No unreleased changes are recorded.

## [1.0.0]

First stable release of MPointCloud by Mekneo.

- PCD 0.7 ASCII, binary and binary_compressed; XYZ/XYZI/XYZRGB/XYZRGBA; PLY 1.0 ASCII and both binary endiannesses; documented PTS XYZI/XYZI+RGB dialect.
- Complete-file PCD Stream input for all three encodings, with current-position reads and caller-owned seekable/non-seekable streams.
- Sequential, ordered/unordered parallel and chunk/async I/O where documented for each format.
- Optional MEngine adapter integration through the public PCD stream API.
- Eight standalone sample projects and an MEngine provider integration guide/consumer recipe.
- Optional adoption invitation with permission-based company/product listing.
- Existing free binary license, attribution requirements, .NET 10 target and dependency independence retained.

## [0.9.0-preview.2]

### Added

- Complete-file `PcdAsciiReader.Read(Stream)` and read-options overload.
- Encoding-independent `PcdReader.Read(Stream)` and optional read-options overload for ASCII, binary, and binary_compressed PCD.
- Current-position input, caller-owned stream lifetime, and seekable/non-seekable support without temporary files or header replay.
- Regression coverage for short reads, nonzero starting positions, failure ownership, malformed/truncated/trailing input, ASCII parallel options, and organized-cloud policies.
- `PcdStreamInput` sample reading all three encodings directly from in-memory ZIP entries.

Existing filename APIs, format support, license, and chunk-streaming APIs remain available. This preview adds direct ASCII stream input for applications such as MEngine.

## [0.9.0-preview.1]

Initial public binary preview distributed through NuGet and GitHub Releases, with installation documentation and runnable consumer samples.

### Formats

- PCD 0.7 ASCII read/write
- PCD 0.7 binary read/write
- PCD 0.7 binary_compressed read/write
- XYZ
- XYZI
- XYZRGB
- XYZRGBA
- PLY 1.0 ASCII
- PLY 1.0 binary little-endian
- PLY 1.0 binary big-endian
- PTS XYZI
- PTS XYZI + RGB

### Performance and I/O

- sequential text I/O
- ordered parallel text I/O
- explicit unordered parallel reads where supported
- bounded chunked processing
- synchronous streaming
- asynchronous streaming where supported
- benchmark-backed text and binary chunk defaults
- exact binary floating-point bit preservation

