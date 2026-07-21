---
name: perf-optimization
description: Use this skill to help identify performance improvements to Bicep. Use for baseline-vs-change comparisons, hotspot triage, and optimization planning.
license: MIT
---

# Bicep Perf Optimization Skill

## Purpose

This skill provides a repeatable workflow to:

1. Capture benchmark results in a consistent format.
2. Collect profiler traces for hotspot analysis.
3. Separate benchmark harness noise from real product costs.
4. Turn trace data into prioritized, actionable optimizations.

Use this for the Bicep repo at the solution root.

## Workflow

Follow this high-level process when invoked:

1. Capture a benchmark with fixed settings.
2. Identify potential CPU and memory improvements from reports and traces.
3. Summarize findings to the user and ask what they want to prioritize.
4. Implement the selected optimization request(s).
5. Re-run the same benchmark command and verify the target issues improved.
6. Summarize measured improvements and offer to create a PR.

Do not skip user checkpoints between analysis and implementation when multiple optimization options exist.

## Preconditions

- Build in Release for benchmark runs.
- Avoid noisy background activity on the machine.
- Use the same benchmark command before and after a change.
- Keep benchmark job settings fixed for comparisons.

## Capture Commands

Run from repository root.

### 1) Holistic short run with profiler

```powershell
dotnet run -c Release --project src/Bicep.Tools.Benchmark/Bicep.Tools.Benchmark.csproj -- -f * -j Short -m -t --exceptions --allStats --join -e json html csv -a BenchmarkDotNet.Artifacts/Holistic --keepFiles
```

### 2) Focused compilation-only run

```powershell
dotnet run -c Release --project src/Bicep.Tools.Benchmark/Bicep.Tools.Benchmark.csproj -- -f *Compilation* -j Short -m -t --exceptions --allStats --join -e json html csv -a BenchmarkDotNet.Artifacts/Compilation --keepFiles
```

## Artifact Locations

Expect outputs under:

- `BenchmarkDotNet.Artifacts/<run-name>/results/*.csv`
- `BenchmarkDotNet.Artifacts/<run-name>/results/*.html`
- `BenchmarkDotNet.Artifacts/<run-name>/*.nettrace`
- `BenchmarkDotNet.Artifacts/<run-name>/*.speedscope.json`

## MCP Server Option (Filtrace)

If the filtrace MCP server is available, prefer it for trace analysis because it can provide workload scoping, rankings, and deltas directly.

Primary MCP tools:

- `mcp_filtrace_trace_info` for capture metadata and analysis availability.
- `mcp_filtrace_trace_rank` for CPU/alloc/exception hotspots (self/inclusive).
- `mcp_filtrace_trace_classify` for CPU category mix (jit, copying, gc, other).
- `mcp_filtrace_trace_diff` for before/after frame-level deltas.
- `mcp_filtrace_trace_callers` and `mcp_filtrace_trace_tree` for ownership and call-path deep dives.

Use benchmark workload scoping when available (for example, `benchmark=true`) to reduce harness noise.

If MCP is not available, use the terminal-only path described in this skill.

## Analysis Workflow

### Step A: Start with summary metrics

Use CSV/HTML reports first to answer:

- Did Mean improve/regress?
- Did Allocated memory improve/regress?
- Did Exceptions per op change?
- Did confidence interval overlap too much to trust the delta?

### Step B: Analyze workload-scoped CPU

For each .nettrace:

1. MCP path: use `mcp_filtrace_trace_info` and `mcp_filtrace_trace_rank` (self + inclusive).
2. Non-MCP path: use matching `.speedscope.json` and run the PowerShell top-frame script.
3. Capture top self and inclusive frames in your perf notes.
4. Compare before/after top frames to confirm hotspot movement.

Interpretation notes:

- High `Monitor.Wait` / `WaitOne` in workload scope often means synchronization-heavy benchmark behavior, not core compute.
- Inclusive stacks reveal owner pipelines. Self stacks reveal low-level cost centers.

### Step C: Analyze exceptions

Use MCP exception ranking (`mcp_filtrace_trace_rank`, `metric=exceptions`) when available.
Otherwise use BenchmarkDotNet CSV columns (for example `Exceptions`) from the same benchmark command.

If one exception type dominates (for example, parser recovery exception storms), treat it as a first-tier optimization target.

### Step D: Analyze runtime categories

Use `mcp_filtrace_trace_classify` when available.
Otherwise use frame-name grouping from speedscope output to estimate where CPU time lands:

- `jit` high: warmup/first-use effects are large.
- `other` high: likely product logic and framework code.

### Step E: Validate with a focused rerun

After a code change, rerun the exact same benchmark command and compare:

- Mean and variance.
- Hot frames share.
- Exception counts.

### Step F: Close the loop with the user

After rerun validation:

1. Summarize what changed and what improved.
2. Call out any regressions or unchanged hotspots.
3. Ask whether to continue with the next optimization target.
4. Offer to prepare or create a PR with benchmark evidence.

## Benchmark-Specific Guidance

### Compilation benchmark

Expected dominant path:

- `Compilation.GetAllDiagnosticsByBicepFile`
- `SemanticModel.AssembleDiagnostics`
- semantic + type assignment + linter paths

Common hotspots:

- Immutable collection materialization churn.
- Lock contention in lazy paths.
- Exception-driven parser recovery.

### LanguageServer typing benchmark

Current implementation waits for diagnostics after each edit. This can dominate with wait frames.

Use it for end-to-end behavior, not pure CPU hotspot attribution.

### PrettyPrint benchmark

Can include compile/setup costs if benchmark compiles per iteration. Split benchmarks if needed:

- compile-only
- pretty-print-only on precompiled model

### Visitor benchmarks (CST/AST)

These are generally clean microbenchmarks. Use for traversal-specific regressions.

## Prioritization Heuristics

Prioritize by:

1. Percent of workload CPU (self and inclusive).
2. Frequency of exceptions per operation.
3. Breadth of impact (affects multiple benchmarks).
4. Complexity/risk of fix.

Typical high-ROI order in this repo:

1. Diagnostics materialization pipeline improvements.
2. Parser exception control-flow reduction.
3. ApiVersion cache and string/parse churn reduction.
4. Lock-contention reduction in lazy/shared paths.

## Quality Bar For Perf Claims

A perf claim should include:

- Exact benchmark command.
- Before and after artifact paths.
- Mean delta and confidence context.
- At least one trace hotspot delta.
- Caveats (for example, source mapping unavailable or wait-heavy workload).
- A clear next-step ask to the user (continue, pivot, or PR).

## Pitfalls To Avoid

- Comparing runs with different job settings.
- Comparing Debug vs Release.
- Using only global CPU ranking without workload scoping.
- Over-interpreting microbenchmark variance with too few iterations.
- Committing large generated benchmark artifacts to PRs unless explicitly required.

## Output Template For Sharing Results

Use this structure in PR comments or perf notes:

1. Goal
2. Command
3. Before path + mean
4. After path + mean
5. Top hotspot deltas
6. Exception deltas
7. Decision and next optimization target
