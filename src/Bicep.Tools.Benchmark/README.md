# Running Benchmarks

## Compilation

Run the "Compilation" benchmark suite, and collect profiler data:
```sh
dotnet run --configuration Release --project src/Bicep.Tools.Benchmark -- --filter *Compilation* --profiler EP
```