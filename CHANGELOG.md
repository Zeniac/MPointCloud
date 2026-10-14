# Changelog

All notable public changes to MPointCloud will be documented in this file.

MPointCloud follows semantic versioning for public releases.

## [Unreleased]

### Planned for first public release

- public binary-only GitHub distribution
- NuGet package
- standalone binary ZIP
- installation and getting-started documentation
- runnable sample projects
- MEngine preferred-provider integration validation

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

## [0.9.0-preview.1]

Initial release-engineering preview version reserved for package and standalone-distribution validation before the first public 1.0 release.
