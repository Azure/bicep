// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.FileSystem;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.Text;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests.Semantics
{
    [TestClass]
    public class ParamsSemanticModelTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private async Task<SemanticModel> CreateSemanticModel(ServiceBuilder services, string paramsFilePath)
        {
            var compiler = services.Build().GetCompiler();
            var compilation = await compiler.CreateCompilation(PathHelper.FilePathToFileUrl(paramsFilePath));

            return compilation.GetEntrypointSemanticModel();
        }

        [DataTestMethod]
        [BaselineData_Bicepparam.TestData()]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public async Task ProgramsShouldProduceExpectedDiagnostic(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);

            var services = await CreateServicesAsync();
            var model = await CreateSemanticModel(services, data.Parameters.OutputFilePath);

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
        public async Task ProgramsShouldProduceExpectedUserDeclaredSymbols(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);

            var services = await CreateServicesAsync();
            var model = await CreateSemanticModel(services, data.Parameters.OutputFilePath);

            var symbols = SymbolCollector
                .CollectSymbols(model)
                .OfType<DeclaredSymbol>();

            string getLoggingString(DeclaredSymbol symbol)
            {
                (_, var startChar) = TextCoordinateConverter.GetPosition(model.SourceFile.LineStarts, symbol.DeclaringSyntax.Span.Position);

                return $"{symbol.Kind} {symbol.Name}. Type: {symbol.Type}. Declaration start char: {startChar}, length: {symbol.DeclaringSyntax.Span.Length}";
            }

            var sourceTextWithDiags = OutputHelper.AddDiagsToSourceText(data.Parameters.EmbeddedFile.Contents, "\n", symbols, symb => symb.NameSource.Span, getLoggingString);

            data.Symbols.WriteToOutputFolder(sourceTextWithDiags);
            data.Symbols.ShouldHaveExpectedValue();
        }

        private async Task<ServiceBuilder> CreateServicesAsync()
            => new ServiceBuilder()
                .WithEnvironmentVariables(
                    ("stringEnvVariableName", "test"),
                    ("intEnvVariableName", "100"),
                    ("boolEnvironmentVariable", "true")
                )
                .WithTestArtifactManager(await MockRegistry.CreateDefaultExternalArtifactManager(TestContext));
    }
}
