# Performance

MPointCloud includes a reproducible Release-mode benchmark harness for comparing read/write modes, parallel execution, binary encodings, compression, and chunk sizes.

The results below come from a representative local run using:

- .NET 10.0.12
- 16 logical processors
- 7 iterations
- runtime-default worker count
- 1,000,000-point dataset for the headline comparison

Run the same benchmark locally with:

```bash
dotnet run --project benchmarks/MPointCloud.Benchmarks/MPointCloud.Benchmarks.csproj -c Release -- --iterations 7
```

Absolute throughput varies by hardware, runtime, dataset, and application environment. The numbers are most useful as comparative evidence between MPointCloud execution modes, not as universal performance guarantees.

Performance can be materially affected by:

- **logical/physical core count** — parallel modes have more CPU resources available on higher-core systems;
- **single-core performance** — sequential parsing, formatting, compression, and coordination overhead are sensitive to per-core speed;
- **memory bandwidth and cache behavior** — large point clouds move substantial amounts of data through memory;
- **storage speed and latency** — NVMe SSDs, SATA SSDs, network storage, and spinning disks can produce very different file-I/O results;
- **file size and schema** — larger records, additional fields, and different encodings change parsing and memory pressure;
- **runtime and JIT version** — .NET runtime improvements or regressions can affect throughput and allocation behavior;
- **worker configuration and system load** — parallel performance depends on available CPU time and scheduler contention;
- **thermal and power limits** — sustained workloads can behave differently on laptops, desktops, workstations, and servers.

For this reason, users evaluating MPointCloud for a performance-sensitive workload should run the benchmark harness on hardware representative of their deployment environment.

## 1,000,000-point read comparison

| Mode | Throughput | Average time | Allocation |
| --- | ---: | ---: | ---: |
| XYZ sequential | 1.86 Mpts/s | 537.59 ms | 563.9 B/pt |
| XYZ ordered parallel | **5.29 Mpts/s** | 188.95 ms | 748.5 B/pt |
| XYZ unordered parallel | 4.97 Mpts/s | 201.24 ms | 868.5 B/pt |
| PCD ASCII sequential | 2.44 Mpts/s | 410.42 ms | 562.3 B/pt |
| PCD ASCII ordered parallel | **6.12 Mpts/s** | 163.36 ms | 571.5 B/pt |
| PCD ASCII unordered parallel | 5.38 Mpts/s | 185.88 ms | 691.5 B/pt |
| PCD binary | **16.05 Mpts/s** | 62.30 ms | 248.0 B/pt |
| PCD binary_compressed | **17.03 Mpts/s** | 58.73 ms | 237.4 B/pt |

On this machine, ordered parallel parsing materially outperformed sequential text parsing while preserving source order. Binary and binary-compressed PCD were substantially faster again.

## 1,000,000-point write comparison

| Mode | Throughput | Average time | Allocation |
| --- | ---: | ---: | ---: |
| XYZ sequential | 2.35 Mpts/s | 425.48 ms | 832.5 B/pt |
| XYZ ordered parallel | 2.54 Mpts/s | 394.47 ms | 840.6 B/pt |
| PCD ASCII sequential | 2.28 Mpts/s | 438.21 ms | 887.5 B/pt |
| PCD ASCII ordered parallel | **2.87 Mpts/s** | 347.96 ms | 895.5 B/pt |
| PCD binary | **14.03 Mpts/s** | 71.30 ms | 99.1 B/pt |
| PCD binary_compressed | 5.77 Mpts/s | 173.20 ms | 91.6 B/pt |

Binary PCD provides the strongest write throughput in the current implementation. Binary-compressed output trades write speed for a compressed payload.

## Chunk-size tuning

The benchmark sweep was also used to select production defaults.

### Text

For 1,000,000-point ordered-parallel reads:

| Format | Chunk 256 | Chunk 1024 | Chunk 4096 | Chunk 16384 |
| --- | ---: | ---: | ---: | ---: |
| XYZ | 3.02 Mpts/s | 4.46 Mpts/s | **5.51 Mpts/s** | 2.84 Mpts/s |
| PCD ASCII | 3.92 Mpts/s | 4.91 Mpts/s | **6.69 Mpts/s** | 2.89 Mpts/s |

Default:

`PointCloudReadOptions.TextChunkSize = 4096`

### Binary streaming

For 1,000,000-point binary streaming reads:

| Chunk | Throughput | Allocation |
| ---: | ---: | ---: |
| 256 | 13.71 Mpts/s | 248.9 B/pt |
| 1024 | 13.80 Mpts/s | 248.2 B/pt |
| 4096 | 13.91 Mpts/s | 248.1 B/pt |
| 16384 | **13.94 Mpts/s** | 248.0 B/pt |

Default:

`PointCloudReadOptions.BinaryChunkSize = 16384`

The larger binary chunk also materially improved the 100,000-point workload while keeping allocation pressure effectively unchanged.

## Interpretation

The benchmark supports several practical conclusions:

- sequential modes remain the simplest deterministic baseline;
- ordered parallel text parsing can provide substantial speedups without sacrificing point order;
- unordered parallel parsing is available when applications explicitly permit reordering, but it is not assumed to be faster on every workload;
- PCD binary provides much higher throughput and lower allocation pressure than text;
- PCD binary_compressed can read very quickly while reducing stored payload size, although compression makes writes more expensive;
- chunk defaults are benchmark-backed rather than arbitrary.

The complete recorded run, including 10,000-, 100,000-, and 1,000,000-point results, is maintained in `benchmarks/MPointCloud.Benchmarks/RESULTS.md`.
