// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;

namespace Bicep.Tools.Benchmark;

[MemoryDiagnoser]
public class PrettyPrint
{
    private record BenchmarkData(
        ImmutableArray<DataSet> DataSets,
        BicepCompiler Compiler);

    private static BenchmarkData CreateBenchmarkData()
    {
        var fileSystem = FileHelper.CreateMockFileSystemForEmbeddedFiles(typeof(DataSet).Assembly, "Files/baselines");

        var dataSets = DataSets.AllDataSets
            .Where(x => !x.HasRegistryModules)
            .ToImmutableArray();

        var bicepService = new ServiceBuilder().WithFileSystem(fileSystem).Build();

        return new(dataSets, bicepService.GetCompiler());
    }

    private BenchmarkData benchmarkData = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        this.benchmarkData = CreateBenchmarkData();
    }

    [Benchmark(Description = "Pretty-print the main file of each valid dataset")]
    public async Task PrettyPrint_valid_dataset_main_file()
    {
        // Reuse a single IBicepService to amortize the cost of instantiating dependencies
        var (dataSets, compiler) = benchmarkData!;

        foreach (var dataSet in dataSets)
        {
            var compilation = await compiler.CreateCompilation(new Uri($"file:///{dataSet.Name}/main.bicep").ToIOUri(), skipRestore: true);
            var semanticModel = compilation.GetEntrypointSemanticModel();
            var context = PrettyPrinterV2Context.From(semanticModel);

            PrettyPrinterV2.Print(semanticModel.Root.Syntax, context);
        }
    }
}

