// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using BenchmarkDotNet.Attributes;
using Bicep.Core.Samples;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Utils;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.Tools.Benchmark;

[MemoryDiagnoser]
public class Visitors
{
    private class BenchmarkCstVisitor : CstVisitor
    {
    }

    private class BenchmarkAstVisitor : AstVisitor
    {
    }

    private static ImmutableArray<ProgramSyntax> CreateBenchmarkData() => DataSets.AllDataSets
        .Select(x => ParserHelper.Parse(x.Bicep))
        .ToImmutableArray();

    private ImmutableArray<ProgramSyntax> benchmarkData;

    private BenchmarkCstVisitor cstVisitor = null!;

    private BenchmarkAstVisitor astVisitor = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        this.benchmarkData = CreateBenchmarkData();
        this.astVisitor = new();
        this.cstVisitor = new();
    }

    [Benchmark(Description = "Visit concrete syntax tree.", Baseline = true)]
    public void Visit_cst()
    {
        foreach (var programSyntax in this.benchmarkData)
        {
            this.cstVisitor.Visit(programSyntax);
        }
    }

    [Benchmark(Description = "Visit abstract syntax tree.")]
    public void Visit_ast()
    {
        foreach (var programSyntax in this.benchmarkData)
        {
            this.astVisitor.Visit(programSyntax);
        }
    }
}
