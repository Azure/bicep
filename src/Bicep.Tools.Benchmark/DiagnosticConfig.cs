// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.Tracing;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using Microsoft.Diagnostics.NETCore.Client;

namespace Bicep.Tools.Benchmark;

/// <summary>
/// BenchmarkDotNet config that enables a rich EventPipe trace capturing
/// CPU samples, managed heap allocations, and monitor-contention events.
///
/// Apply with [Config(typeof(DiagnosticConfig))] on a benchmark class.
/// Do NOT combine with the -p EP CLI flag – this config already includes
/// an EventPipeProfiler and adding a second one causes a duplicate-diagnoser error.
///
/// Thread-time / wait metrics (filtrace metric=wait) require kernel context-switch
/// events and are only available with the ETW profiler (-p ETW, Windows admin required).
/// </summary>
internal class DiagnosticConfig : ManualConfig
{
    public DiagnosticConfig()
    {
        // CpuSampling profile provides:
        //   Microsoft-DotNETCore-SampleProfiler  – CPU sample stacks
        //   Microsoft-Windows-DotNETRuntime       – basic runtime keywords
        //
        // The extra provider extends Microsoft-Windows-DotNETRuntime with:
        //   0x0080  GCAllocationTick – fires per ~100 kB allocated, carries call stack
        //   0x4000  Contention       – fires on monitor lock contention
        // EventPipe ORs keyword masks when the same provider appears more than once,
        // so these bits are additive on top of what CpuSampling already enables.
        AddDiagnoser(new EventPipeProfiler(
            EventPipeProfile.CpuSampling,
            providers: new[]
            {
                new EventPipeProvider(
                    "Microsoft-Windows-DotNETRuntime",
                    EventLevel.Verbose,
                    0x4080L) // GCAllocationTick | Contention
            }));
    }
}
