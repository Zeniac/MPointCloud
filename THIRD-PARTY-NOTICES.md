# Third-Party Notices

MPointCloud is an independent Mekneo implementation.

## Point Cloud Data (PCD)

The PCD file format was developed as part of the open-source Point Cloud Library (PCL) project.

MPointCloud implements PCD interoperability from publicly available format documentation and independently written .NET code. The MPointCloud runtime does not contain, wrap, link to, or require the PCL C++ implementation.

Development and interoperability tests may use small reference fixtures from the Point Cloud Library data repository under their applicable upstream terms. Those development fixtures are not required by the MPointCloud runtime.

Point Cloud Library project:

https://pointclouds.org/

## Polygon File Format (PLY)

MPointCloud implements the documented PLY 1.0 file conventions independently for point-cloud vertex data.

No third-party PLY reader/writer implementation is bundled with the MPointCloud runtime.

## PTS

PTS is a widely used scanner-oriented plain-text point format with conventions that vary between exporters. MPointCloud documents and implements an explicit supported dialect rather than claiming universal compatibility with every PTS variant.

## MPointCloud

The MPointCloud source code, architecture, APIs, tests, performance work, documentation, packaging, and .NET implementation are original Mekneo software except where third-party attribution is explicitly stated.

Copyright © 2026 Mekneo.
