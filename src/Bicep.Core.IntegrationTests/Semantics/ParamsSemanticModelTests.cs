// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.FileSystem;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.Text;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Bicep.Core.IntegrationTests.Semantics
{
    [TestClass]
    public class ParamsSemanticModelTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private ParamsSemanticModel CreateSemanticModel(string paramsFilePath)
        {
            var dispatcher = BicepTestConstants.ModuleDispatcher;
            var configuration = BicepTestConstants.BuiltInConfiguration;
            var sourceFileGrouping = SourceFileGroupingBuilder.Build(BicepTestConstants.FileResolver, dispatcher, new Workspace(), PathHelper.FilePathToFileUrl(paramsFilePath), configuration);
            var compilation = new Compilation(BicepTestConstants.Features, TestTypeHelper.CreateEmptyProvider(), sourceFileGrouping, configuration, BicepTestConstants.ApiVersionProvider, BicepTestConstants.LinterAnalyzer);

            return new ParamsSemanticModel(sourceFileGrouping, file => {
                var compilationGrouping = new SourceFileGrouping(BicepTestConstants.FileResolver, file.FileUri, sourceFileGrouping.FileResultByUri, sourceFileGrouping.UriResultByModule, sourceFileGrouping.SourceFileParentLookup);


                return new Compilation(BicepTestConstants.Features, BicepTestConstants.NamespaceProvider, compilationGrouping, configuration, BicepTestConstants.ApiVersionProvider, BicepTestConstants.LinterAnalyzer);
            });
        }

        [DataTestMethod]
        [BaselineData_Bicepparam.TestData()]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void ProgramsShouldProduceExpectedDiagnostic(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);

            var model = CreateSemanticModel(data.Parameters.OutputFilePath);

            // use a deterministic order
            var diagnostics = model.GetAllDiagnostics()
                .OrderBy(x => x.Span.Position)
                .ThenBy(x => x.Span.Length)
                .ThenBy(x => x.Message, StringComparer.Ordinal);

            var sourceTextWithDiags = OutputHelper.AddDiagsToSourceText(data.Parameters.EmbeddedFile.Contents, "\n", diagnostics,
                diag => OutputHelper.GetDiagLoggingString(data.Parameters.EmbeddedFile.Contents, data.OutputFolder.OutputFolderPath, diag));

            data.Diagnostics.WriteToOutputFolder(sourceTextWithDiags);
            data.Diagnostics.ShouldHaveExpectedValue();
        }

        [DataTestMethod]
        [BaselineData_Bicepparam.TestData()]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void ProgramsShouldProduceExpectedUserDeclaredSymbols(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);

            var model = CreateSemanticModel(data.Parameters.OutputFilePath);

            var symbols = SymbolCollector
                .CollectSymbols(model)
                .OfType<ParameterAssignmentSymbol>();

            string getLoggingString(ParameterAssignmentSymbol symbol)
            {
                (_, var startChar) = TextCoordinateConverter.GetPosition(model.BicepParamFile.LineStarts, symbol.AssigningSyntax.Span.Position);

                return $"{symbol.Kind} {symbol.Name}. Type: {symbol.Type}. Declaration start char: {startChar}, length: {symbol.AssigningSyntax.Span.Length}";
            }

            var sourceTextWithDiags = OutputHelper.AddDiagsToSourceText(data.Parameters.EmbeddedFile.Contents, "\n", symbols, symb => symb.NameSyntax.Span, getLoggingString);

            data.Symbols.WriteToOutputFolder(sourceTextWithDiags);
            data.Symbols.ShouldHaveExpectedValue();
        }
    }
}
