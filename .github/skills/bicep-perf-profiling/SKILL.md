---
name: bicep-perf-profiling
description: 'Capture and analyze .NET traces for Bicep CLI compilation. Use when profiling Bicep builds, batch compilation, CPU hotspots, allocations, GC pressure, contention, or compiler performance with dotnet-trace and filtrace.'
argument-hint: '<folder containing .bicep files>'
---

# Bicep Performance Profiling

Capture a Release trace of `bicep build --pattern`, then use the filtrace MCP server to identify actionable compiler hotspots.

## Prerequisites

- Run commands from the Bicep repository root.
- Ensure `pwsh`, the SDK pinned by `global.json`, and `dotnet-trace` are available.
- If needed, install `dotnet-trace` with `dotnet tool install --global dotnet-trace`.

## Capture

Run the bundled [profile script](./scripts/BuildAndProfile.ps1):

```pwsh
pwsh ./.github/skills/bicep-trace-analysis/scripts/BuildAndProfile.ps1 src/Bicep.Core.Samples/Files/user_submitted
```

The script:

1. Builds `Bicep.Cli` in Release mode under `src/Bicep.Cli/bin/profile/Release`.
2. Runs `bicep build --pattern <folder>/**/*.bicep`.
3. Writes `profile-<UTC timestamp>.nettrace` to the current directory.
4. Captures sampled thread stacks, common runtime events, verbose GC events, and sampled allocations.

## Analyze

Use filtrace in this order:

1. Run `trace_info` with the trace path and `symbols=src/Bicep.Cli/bin/profile/Release`.
2. Require strong method-name resolution before interpreting rankings. Source mapping may be lower because framework PDBs are unavailable; ensure there are no PDB identity mismatches for Bicep assemblies.
3. Run CPU `trace_rank` with both `self` and `inclusive` measures.
4. Scope compiler rankings with `root=Bicep.Cli.Commands.BuildCommand.Compile`. Whole-process rankings include runtime service threads and sampled waits that can obscure compiler work.
5. Run allocation `trace_rank` with both measures and run `trace_gc` for collection count, pause time, peak heap, and promoted bytes.
6. Use `trace_callers` on hot framework leaves such as file I/O, JSON serialization, array growth, boxing, or locks until reaching the Bicep-owned caller.
7. Use `trace_tree` for major Bicep phases and `trace_lines` or `trace_heatmap` only when matching PDBs provide sufficient source attribution.

Useful first-pass analyses:

```text
trace_info(path, symbols)
trace_rank(path, metric=cpu, measure=self, root=Bicep.Cli.Commands.BuildCommand.Compile, symbols=symbols)
trace_rank(path, metric=cpu, measure=inclusive, root=Bicep.Cli.Commands.BuildCommand.Compile, symbols=symbols)
trace_rank(path, metric=alloc, measure=self)
trace_rank(path, metric=alloc, measure=inclusive)
trace_gc(path)
```

## Report

Report the top five optimization areas. For each area include:

- The inclusive or self weight and percentage of the scoped compile workload.
- The Bicep-owned method or phase responsible for the cost.
- A concrete optimization hypothesis and the smallest experiment that could falsify it.
- Any overlap with another finding; inclusive percentages are not additive.

Call out capture limitations. Prefer a longer trace when a result has fewer than 200 samples, and do not claim allocation absence unless allocation capture is known to be enabled. Re-capture and compare like-for-like traces after any optimization.