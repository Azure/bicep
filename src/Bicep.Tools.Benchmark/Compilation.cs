// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using BenchmarkDotNet.Attributes;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Immutable;
using System.IO.Abstractions;
using System.Linq;

namespace Bicep.Tools.Benchmark;

[MemoryDiagnoser]
public class Compilation
{
    private record BenchmarkData(
        ImmutableArray<DataSet> DataSets,
        IDependencyHelper BicepService);

    private static BenchmarkData CreateBenchmarkData()
    {
        var fileSystem = FileHelper.CreateMockFileSystemForEmbeddedFiles(typeof(DataSet).Assembly, "Files");

        var dataSets = DataSets.AllDataSets
            .Where(x => !x.HasRegistryModules)
            .ToImmutableArray();

        var bicepService = new ServiceBuilder()
            .WithRegistration(x => x.AddSingleton<IFileSystem>(fileSystem)).Build();

        return new(dataSets, bicepService);
    }

    private BenchmarkData? benchmarkData;

    [GlobalSetup]
    public void GlobalSetup()
    {
        this.benchmarkData = CreateBenchmarkData();
    }

    [Benchmark(Description = "Get compilation diagnostics for each dataset")]
    public void Get_compilation_diagnostics()
    {
        // Reuse a single IBicepService to amortize the cost of instantiating dependencies
        var (dataSets, service) = benchmarkData!;

        foreach (var dataSet in dataSets)
        {
            var sourceFileGrouping = service.BuildSourceFileGrouping(new Uri($"file:///{dataSet.Name}/main.bicep"), false);
            var compilation = service.BuildCompilation(sourceFileGrouping);

            var diagnostics = compilation.GetAllDiagnosticsByBicepFile();
        }
    }
}
