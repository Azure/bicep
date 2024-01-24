// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Intermediate;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Text;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Syntax;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.Core.IntegrationTests.Semantics
{
    [TestClass]
    public class SemanticModelTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static ServiceBuilder Services => new ServiceBuilder()
            .WithEnvironmentVariables(
                ("stringEnvVariableName", "test"),
                ("intEnvVariableName", "100"),
                ("boolEnvironmentVariable", "true")
            );

        // NOTE: Uses the linter analyzers specified in BicepTestConstants.BuiltInConfigurationWithProblematicAnalyzersDisabled
        //   Problematic ones that should be disabled in this and most other tests by default can be added to BicepTestConstants.AnalyzerRulesToDisableInTests
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
            var compilation = Services.BuildCompilation("parameter ");

            FluentActions.Invoking(() => compilation.GetEntrypointSemanticModel().GetAllDiagnostics()).Should().NotThrow();
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

            var sourceTextWithDiags = DataSet.AddDiagsToSourceText(dataSet, symbols, symb => symb.NameSource.Span, getLoggingString);
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
                        s is MetadataSymbol ||
                        s is ParameterSymbol ||
                        s is TypeAliasSymbol ||
                        s is AmbientTypeSymbol ||
                        s is VariableSymbol ||
                        s is ResourceSymbol ||
                        s is ModuleSymbol ||
                        s is OutputSymbol ||
                        s is FunctionSymbol ||
                        s is DeclaredFunctionSymbol ||
                        s is ProviderNamespaceSymbol ||
                        s is BuiltInNamespaceSymbol ||
                        s is LocalVariableSymbol ||
                        s is TestSymbol ||
                        s is ImportedTypeSymbol ||
                        s is ImportedVariableSymbol ||
                        s is ImportedFunctionSymbol ||
                        s is WildcardImportSymbol);
                }
                else
                {
                    // invalid files may return errors
                    symbol.Should().Match(s =>
                        s is ErrorSymbol ||
                        s is MetadataSymbol ||
                        s is ParameterSymbol ||
                        s is TypeAliasSymbol ||
                        s is AmbientTypeSymbol ||
                        s is VariableSymbol ||
                        s is ResourceSymbol ||
                        s is ModuleSymbol ||
                        s is OutputSymbol ||
                        s is FunctionSymbol ||
                        s is DeclaredFunctionSymbol ||
                        s is ProviderNamespaceSymbol ||
                        s is BuiltInNamespaceSymbol ||
                        s is LocalVariableSymbol ||
                        s is TestSymbol ||
                        s is ImportedTypeSymbol ||
                        s is ImportedVariableSymbol ||
                        s is ImportedFunctionSymbol ||
                        s is ErroredImportSymbol ||
                        s is WildcardImportSymbol);
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
                .Select(semanticModel.GetSymbolInfo)
                .Distinct();

            symbols.Should().NotContainNulls();

            var foundReferences = symbols
                .SelectMany(s => semanticModel.FindReferences(s!))
                .Where(refSyntax => !(refSyntax is INamedDeclarationSyntax));

            symbolReferences.Should().BeSubsetOf(foundReferences);
        }

        [TestMethod]
        public void GetAllDiagnostics_VerifyDisableNextLineDiagnosticsDirectiveDoesNotSupportCoreCompilerErrorSuppression()
        {
            var bicepFileContents = @"#disable-next-line BCP029 BCP068
resource test";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", bicepFileContents);
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var uri = documentUri.ToUriEncoded();

            var files = new Dictionary<Uri, string>
            {
                [uri] = bicepFileContents,
            };

            var compilation = Services.BuildCompilation(files, uri);
            var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics();

            diagnostics.Count().Should().Be(2);
            diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP068");
                },
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP029");
                });
        }

        [TestMethod]
        public void GetAllDiagnostics_VerifyDisableNextLineDiagnosticsDirectiveSupportsCoreCompilerWarningSuppression()
        {
            var bicepFileContents = @"var vmProperties = {
  diagnosticsProfile: {
    bootDiagnostics: {
      enabled: 123
      storageUri: true
      unknownProp: 'asdf'
    }
  }
  evictionPolicy: 'Deallocate'
}
resource vm 'Microsoft.Compute/virtualMachines@2020-12-01' = {
  name: 'vm'
#disable-next-line no-hardcoded-location
  location: 'West US'
#disable-next-line BCP036 BCP037
  properties: vmProperties
}";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", bicepFileContents);
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var uri = documentUri.ToUriEncoded();

            var files = new Dictionary<Uri, string>
            {
                [uri] = bicepFileContents,
            };

            var compilation = Services.BuildCompilation(files, uri);

            compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllDiagnostics_VerifyDisableNextLineDiagnosticsDirectiveSupportsLinterWarningSuppression()
        {
            var bicepFileContents = @"#disable-next-line no-unused-params
param storageAccount string = 'testStorageAccount'";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", bicepFileContents);
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var uri = documentUri.ToUriEncoded();

            var files = new Dictionary<Uri, string>
            {
                [uri] = bicepFileContents,
            };

            var compilation = Services.BuildCompilation(files, uri);

            compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Should().BeEmpty();
        }

        [TestMethod]
        public void GetAllDiagnostics_WithNoDisableNextLineDiagnosticsDirectiveInPreviousLine_ShouldReturnDiagnostics()
        {
            var bicepFileContents = @"#disable-next-line no-unused-params

param storageAccount string = 'testStorageAccount'";
            var bicepFilePath = FileHelper.SaveResultFile(TestContext, "main.bicep", bicepFileContents);
            var documentUri = DocumentUri.FromFileSystemPath(bicepFilePath);
            var uri = documentUri.ToUriEncoded();

            var files = new Dictionary<Uri, string>
            {
                [uri] = bicepFileContents,
            };

            var compilation = Services.BuildCompilation(files, uri);

            compilation.GetEntrypointSemanticModel().GetAllDiagnostics().Count().Should().Be(1);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public async Task All_nodes_should_be_parented(DataSet dataSet)
        {
            var (compilation, outputDirectory, _) = await dataSet.SetupPrerequisitesAndCreateCompilation(TestContext);
            var model = compilation.GetEntrypointSemanticModel();

            var allNodes = SyntaxCollectorVisitor.Build(model.Root.Syntax);
            foreach (var node in allNodes)
            {
                if (node.Syntax == model.Root.Syntax)
                {
                    model.Binder.GetParent(node.Syntax).Should().BeNull();
                }
                else
                {
                    model.Binder.GetParent(node.Syntax).Should().NotBeNull();
                }
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidDataSets), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task ProgramsShouldProduceExpectedIrTree(DataSet dataSet)
        {
            var (compilation, outputDirectory, _) = await dataSet.SetupPrerequisitesAndCreateCompilation(TestContext);
            var model = compilation.GetEntrypointSemanticModel();

            var builder = new ExpressionBuilder(new(model));
            var converted = builder.Convert(model.Root.Syntax);

            var expressionList = ExpressionCollectorVisitor.Build(converted);
            var expressionByParent = expressionList.ToLookup(x => x.Parent);

            TextSpan getSpan(ExpressionCollectorVisitor.ExpressionItem data) => data.Expression.SourceSyntax?.Span ?? TextSpan.TextDocumentStart;

            var sourceTextWithDiags = DataSet.AddDiagsToSourceText(dataSet, expressionList, getSpan, expression => ExpressionCollectorVisitor.GetExpressionLoggingString(expressionByParent, expression));
            var resultsFile = FileHelper.SaveResultFile(this.TestContext, Path.Combine(dataSet.Name, DataSet.TestFileMainIr), sourceTextWithDiags);

            sourceTextWithDiags.Should().EqualWithLineByLineDiffOutput(
                TestContext,
                dataSet.Ir ?? "",
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainIr),
                actualLocation: resultsFile);
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

        private static IEnumerable<object[]> GetValidDataSets() => DataSets
            .AllDataSets
            .Where(ds => ds.IsValid)
            .ToDynamicTestData();
    }
}
