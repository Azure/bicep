// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Samples;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.IO.Abstractions.TestingHelpers;
using System.IO.Abstractions;
using System.Collections.Immutable;
using FluentAssertions;
using System;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using SharpYaml;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;

namespace Bicep.Tools.Benchmark;

[MemoryDiagnoser]
public class PrettyPrint
{
    private record BenchmarkData(
        ImmutableArray<DataSet> DataSets,
        IDependencyHelper BicepService);

    private static BenchmarkData CreateBenchmarkData()
    {
        var fileSystem = FileHelper.CreateMockFileSystemForEmbeddedFiles(typeof(DataSet).Assembly, "Files");

        var dataSets = DataSets.AllDataSets
            .Where(x => x.IsValid)
            .ToImmutableArray();

        var bicepService = new ServiceBuilder()
            .WithRegistration(x => x.AddSingleton(fileSystem)).Build();

        return new(dataSets, bicepService);
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
    public void PrettyPrint_valid_dataset_main_file()
    {
        // Reuse a single IBicepService to amortize the cost of instantiating dependencies
        var (dataSets, service) = benchmarkData;

        foreach (var dataSet in dataSets)
        {
            var sourceFileGrouping = service.BuildSourceFileGrouping(new Uri($"file:///{dataSet.Name}/main.bicep"), false);
            var compilation = service.BuildCompilation(sourceFileGrouping);

            PrettyPrinter.PrintValidProgram(compilation.GetEntrypointSemanticModel().Root.Syntax, this.printOptions);
        }
    }
}

