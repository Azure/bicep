﻿using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.IntegrationTests.Extensons;
using Bicep.Core.Navigation;
using Bicep.Core.Samples;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.UnitTests.Json;
using Bicep.Core.UnitTests.Serialization;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests.SemanticModel
{
    [TestClass]
    public class SemanticModelTests
    {
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void ProgramsShouldProduceExpectedDiagnostics(DataSet dataSet)
        {
            var compilation = new Compilation(SyntaxFactory.CreateFromText(dataSet.Bicep));
            var model = compilation.GetSemanticModel();

            string getDiagnosticSpanText(Diagnostic diagnostic)
                => dataSet.Bicep[new Range(diagnostic.Span.Position, diagnostic.Span.Position + diagnostic.Span.Length)];
            
            var sourceTextWithDiags = OutputHelper.AddDiagsToSourceText(dataSet.Bicep, model.GetAllDiagnostics(), diag => $"{diag.Level} {diag.Message} |{getDiagnosticSpanText(diag)}|");
            var resultsFile = FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}/{DataSet.TestFileMainDiagnostics}", sourceTextWithDiags);

            sourceTextWithDiags.Should().EqualWithLineByLineDiffOutput(
                dataSet.Diagnostics,
                sourceLocation: $"src/Bicep.Core.Samples/{dataSet.Name}/{DataSet.TestFileMainDiagnostics}",
                targetLocation: resultsFile);
        }

        [TestMethod]
        public void EndOfFileFollowingSpaceAfterParameterKeyWordShouldNotThrow()
        {
            var compilation = new Compilation(SyntaxFactory.CreateFromText("parameter "));
            compilation.GetSemanticModel().GetParseDiagnostics();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void ProgramsShouldProduceExpectedUserDeclaredSymbols(DataSet dataSet)
        {
            var compilation = new Compilation(SyntaxFactory.CreateFromText(dataSet.Bicep));
            var model = compilation.GetSemanticModel();

            var symbols = SymbolCollector
                .CollectSymbols(model)
                .OfType<DeclaredSymbol>();

            var sourceTextWithDiags = OutputHelper.AddDiagsToSourceText(dataSet.Bicep, symbols, symb => symb.NameSyntax.Span, symb => $"{symb.Kind} {symb.Name}");
            var resultsFile = FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}/{DataSet.TestFileMainSymbols}", sourceTextWithDiags);

            sourceTextWithDiags.Should().EqualWithLineByLineDiffOutput(
                dataSet.Symbols,
                sourceLocation: $"src/Bicep.Core.Samples/{dataSet.Name}/{DataSet.TestFileMainSymbols}",
                targetLocation: resultsFile);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void NameBindingsShouldBeConsistent(DataSet dataSet)
        {
            var compilation = new Compilation(SyntaxFactory.CreateFromText(dataSet.Bicep));
            
            var symbolReferences = GetSymbolReferences(compilation.ProgramSyntax);

            // just a sanity check
            symbolReferences.Should().AllBeAssignableTo<ISymbolReference>();

            var model = compilation.GetSemanticModel();
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
                        s is OutputSymbol ||
                        s is FunctionSymbol);
                }
                else
                {
                    // invalid files may return errors
                    symbol.Should().Match(s =>
                        s is ErrorSymbol ||
                        s is ParameterSymbol ||
                        s is VariableSymbol ||
                        s is ResourceSymbol ||
                        s is OutputSymbol ||
                        s is FunctionSymbol);
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
            var compilation = new Compilation(SyntaxFactory.CreateFromText(dataSet.Bicep));

            var symbolReferences = GetSymbolReferences(compilation.ProgramSyntax);

            var symbols = symbolReferences
                .Select(symRef => compilation.GetSemanticModel().GetSymbolInfo(symRef))
                .Distinct();

            symbols.Should().NotContainNulls();

            var foundReferences = symbols
                .SelectMany(s => compilation.GetSemanticModel().FindReferences(s!))
                .Where(refSyntax => !(refSyntax is IDeclarationSyntax));

            foundReferences.Should().BeEquivalentTo(symbolReferences);
        }

        private static List<SyntaxBase> GetSymbolReferences(ProgramSyntax program)
        {
            return SyntaxAggregator.Aggregate(
                program,
                new List<SyntaxBase>(),
                (accumulated, current) =>
                {
                    if (current is ISymbolReference)
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
