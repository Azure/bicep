// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Samples;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests.SemanticModel
{
    [TestClass]
    public class SemanticModelTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void ProgramsShouldProduceExpectedDiagnostics(DataSet dataSet)
        {
            var compilation = dataSet.CopyFilesAndCreateCompilation(TestContext, out var outputDirectory);
            var model = compilation.GetEntrypointSemanticModel();
            
            var sourceTextWithDiags = DataSet.AddDiagsToSourceText(dataSet, model.GetAllDiagnostics(), diag => OutputHelper.GetDiagLoggingString(dataSet.Bicep, outputDirectory, diag));
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
            var compilation = new Compilation(TestResourceTypeProvider.Create(), SyntaxFactory.CreateFromText("parameter "));
            compilation.GetEntrypointSemanticModel().GetParseDiagnostics();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void ProgramsShouldProduceExpectedUserDeclaredSymbols(DataSet dataSet)
        {
            var compilation = dataSet.CopyFilesAndCreateCompilation(TestContext, out var outputDirectory);
            var model = compilation.GetEntrypointSemanticModel();

            var symbols = SymbolCollector
                .CollectSymbols(model)
                .OfType<DeclaredSymbol>();

            var lineStarts = compilation.SyntaxTreeGrouping.EntryPoint.LineStarts;
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
        public void NameBindingsShouldBeConsistent(DataSet dataSet)
        {
            var compilation = dataSet.CopyFilesAndCreateCompilation(TestContext, out _);
            var model = compilation.GetEntrypointSemanticModel();
            var symbolReferences = GetAllBoundSymbolReferences(compilation.SyntaxTreeGrouping.EntryPoint.ProgramSyntax, model);

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
                        s is NamespaceSymbol);
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
                        s is NamespaceSymbol);
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
        public void FindReferencesResultsShouldIncludeAllSymbolReferenceSyntaxNodes(DataSet dataSet)
        {
            var compilation = dataSet.CopyFilesAndCreateCompilation(TestContext, out _);
            var semanticModel = compilation.GetEntrypointSemanticModel();
            var symbolReferences = GetAllBoundSymbolReferences(compilation.SyntaxTreeGrouping.EntryPoint.ProgramSyntax, semanticModel);

            var symbols = symbolReferences
                .Select(symRef => semanticModel.GetSymbolInfo(symRef))
                .Distinct();

            symbols.Should().NotContainNulls();

            var foundReferences = symbols
                .SelectMany(s => semanticModel.FindReferences(s!))
                .Where(refSyntax => !(refSyntax is INamedDeclarationSyntax));

            foundReferences.Should().BeEquivalentTo(symbolReferences);
        }

        private static List<SyntaxBase> GetAllBoundSymbolReferences(ProgramSyntax program, Core.SemanticModel.SemanticModel semanticModel)
        {
            return SyntaxAggregator.Aggregate(
                program,
                new List<SyntaxBase>(),
                (accumulated, current) =>
                {
                    if (current is ISymbolReference symbolReference && TestSyntaxHelper.NodeShouldBeBound(symbolReference, semanticModel))
                    {
                        accumulated.Add(current);
                    }
                    
                    return accumulated;
                },
                accumulated => accumulated);
        }

        private static IEnumerable<object[]> GetData() => DataSets.AllDataSets.ToDynamicTestData();

        private static bool FilterSymbol(Symbol symbol)
        {
            switch (symbol.Kind)
            {
                // namespace and function symbols don't have locations
                // type symbols will have their own tests
                case SymbolKind.Namespace:
                case SymbolKind.Function:
                case SymbolKind.Type:
                    return false;

                // allow everything else
                default:
                    return true;
            }
        }
    }
}

