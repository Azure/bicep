// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Bicep.Core;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SharpYaml;

namespace Bicep.Tools.Benchmark;

[MemoryDiagnoser]
public class PrettyPrint
{
    private record BenchmarkData(
        ImmutableArray<DataSet> DataSets,
        BicepCompiler Compiler);

    private static BenchmarkData CreateBenchmarkData()
    {
        var fileSystem = FileHelper.CreateMockFileSystemForEmbeddedFiles(typeof(DataSet).Assembly, "Files");

        var dataSets = DataSets.AllDataSets
            .Where(x => !x.HasRegistryModules)
            .ToImmutableArray();

        var bicepService = new ServiceBuilder().WithFileSystem(fileSystem).Build();

        return new(dataSets, bicepService.GetCompiler());
    }

    private BenchmarkData benchmarkData = null!;

    private PrettyPrintOptions printOptions = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        this.benchmarkData = CreateBenchmarkData();
        this.printOptions = new PrettyPrintOptions(NewlineOption.LF, IndentKindOption.Space, 2, true);
    }

    [Benchmark(Description = "Pretty-print the main file of each valid dataset")]
    public async Task PrettyPrint_valid_dataset_main_file()
    {
        // Reuse a single IBicepService to amortize the cost of instantiating dependencies
        var (dataSets, compiler) = benchmarkData!;

        foreach (var dataSet in dataSets)
        {
            var compilation = await compiler.CreateCompilation(new Uri($"file:///{dataSet.Name}/main.bicep"), skipRestore: true);

            PrettyPrinter.PrintValidProgram(compilation.GetEntrypointSemanticModel().Root.Syntax, this.printOptions);
        }
    }
}

