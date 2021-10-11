// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Navigation;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Text;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bicep.Core.IntegrationTests.Semantics
{
    [TestClass]
    public class SemanticModelTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        // TODO:  handle varying linter diagnostic expectations for data driven test
        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task ProgramsShouldProduceExpectedDiagnostics(DataSet dataSet)
        {
            var (compilation, outputDirectory, _) = await dataSet.SetupPrerequisitesAndCreateCompilation(TestContext);
            var model = compilation.GetEntrypointSemanticModel();

            // use a deterministic order
            var diagnostics = model.GetAllDiagnostics()
                .OrderBy(x => x.Span.Position)
                .ThenBy(x => x.Span.Length)
                .ThenBy(x => x.Message, StringComparer.Ordinal);

            var sourceTextWithDiags = DataSet.AddDiagsToSourceText(dataSet, diagnostics, diag => OutputHelper.GetDiagLoggingString(dataSet.Bicep, outputDirectory, diag));
            var resultsFile = Path.Combine(outputDirectory, DataSet.TestFileMainDiagnostics);
            File.WriteAllText(resultsFile, sourceTextWithDiags);

            sourceTextWithDiags.Should().EqualWithLineByLineDiffOutput(
                TestContext,
                dataSet.Diagnostics,
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainDiagnostics),
                actualLocation: resultsFile);
        }

        [TestMethod]
        public void EndOfFileFollowingSpaceAfterParameterKeyWordShouldNotThrow()
        {
            var compilation = new Compilation(TestTypeHelper.CreateEmptyProvider(), SourceFileGroupingFactory.CreateFromText("parameter ", BicepTestConstants.FileResolver), BicepTestConstants.BuiltInConfiguration);
            compilation.GetEntrypointSemanticModel().GetParseDiagnostics();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task ProgramsShouldProduceExpectedUserDeclaredSymbols(DataSet dataSet)
        {
            var (compilation, outputDirectory, _) = await dataSet.SetupPrerequisitesAndCreateCompilation(TestContext);
            var model = compilation.GetEntrypointSemanticModel();

            var symbols = SymbolCollector
                .CollectSymbols(model)
                .OfType<DeclaredSymbol>();

            var lineStarts = compilation.SourceFileGrouping.EntryPoint.LineStarts;
            string getLoggingString(DeclaredSymbol symbol)
            {
                (_, var startChar) = TextCoordinateConverter.GetPosition(lineStarts, symbol.DeclaringSyntax.Span.Position);

                return $"{symbol.Kind} {symbol.Name}. Type: {symbol.Type}. Declaration start char: {startChar}, length: {symbol.DeclaringSyntax.Span.Length}";
            }

            var sourceTextWithDiags = DataSet.AddDiagsToSourceText(dataSet, symbols, symb => symb.NameSyntax.Span, getLoggingString);
            var resultsFile = Path.Combine(outputDirectory, DataSet.TestFileMainDiagnostics);
            File.WriteAllText(resultsFile, sourceTextWithDiags);

            sourceTextWithDiags.Should().EqualWithLineByLineDiffOutput(
                TestContext,
                dataSet.Symbols,
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainSymbols),
                actualLocation: resultsFile);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task NameBindingsShouldBeConsistent(DataSet dataSet)
        {
            var (compilation, _, _) = await dataSet.SetupPrerequisitesAndCreateCompilation(TestContext);
            var model = compilation.GetEntrypointSemanticModel();
            var symbolReferences = GetAllBoundSymbolReferences(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);

            // just a sanity check
            symbolReferences.Should().AllBeAssignableTo<ISymbolReference>();

            foreach (SyntaxBase symbolReference in symbolReferences)
            {
                var symbol = model.GetSymbolInfo(symbolReference);
                symbol.Should().NotBeNull();

                if (dataSet.IsValid)
                {
                    // valid cases should not return error symbols for any symbol reference node
                    symbol.Should().NotBeOfType<ErrorSymbol>();
                    symbol.Should().Match(s =>
                        s is ParameterSymbol ||
                        s is VariableSymbol ||
                        s is ResourceSymbol ||
                        s is ModuleSymbol ||
                        s is OutputSymbol ||
                        s is FunctionSymbol ||
                        s is ImportedNamespaceSymbol ||
                        s is BuiltInNamespaceSymbol ||
                        s is LocalVariableSymbol);
                }
                else
                {
                    // invalid files may return errors
                    symbol.Should().Match(s =>
                        s is ErrorSymbol ||
                        s is ParameterSymbol ||
                        s is VariableSymbol ||
                        s is ResourceSymbol ||
                        s is ModuleSymbol ||
                        s is OutputSymbol ||
                        s is FunctionSymbol ||
                        s is ImportedNamespaceSymbol ||
                        s is BuiltInNamespaceSymbol ||
                        s is LocalVariableSymbol);
                }

                var foundRefs = model.FindReferences(symbol!);

                // the returned references should contain the original ref that we used to find the symbol
                foundRefs.Should().Contain(symbolReference);

                // each ref should map to the same exact symbol
                foreach (SyntaxBase foundRef in foundRefs)
                {
                    model.GetSymbolInfo(foundRef).Should().BeSameAs(symbol);
                }
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task FindReferencesResultsShouldIncludeAllSymbolReferenceSyntaxNodes(DataSet dataSet)
        {
            var (compilation, _, _) = await dataSet.SetupPrerequisitesAndCreateCompilation(TestContext);
            var semanticModel = compilation.GetEntrypointSemanticModel();
            var symbolReferences = GetAllBoundSymbolReferences(compilation.SourceFileGrouping.EntryPoint.ProgramSyntax);

            var symbols = symbolReferences
                .Select(symRef => semanticModel.GetSymbolInfo(symRef))
                .Distinct();

            symbols.Should().NotContainNulls();

            var foundReferences = symbols
                .SelectMany(s => semanticModel.FindReferences(s!))
                .Where(refSyntax => !(refSyntax is INamedDeclarationSyntax));

            symbolReferences.Should().BeSubsetOf(foundReferences);
        }

        private static List<SyntaxBase> GetAllBoundSymbolReferences(ProgramSyntax program)
        {
            return SyntaxAggregator.Aggregate(
                program,
                new List<SyntaxBase>(),
                (accumulated, current) =>
                {
                    if (current is ISymbolReference symbolReference && TestSyntaxHelper.NodeShouldBeBound(symbolReference))
                    {
                        accumulated.Add(current);
                    }

                    return accumulated;
                },
                accumulated => accumulated);
        }

        private static IEnumerable<object[]> GetData() => DataSets.AllDataSets.ToDynamicTestData();
    }
}

