// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Extensions;
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
            var compilation = await compiler.CreateCompilation(PathHelper.FilePathToFileUrl(paramsFilePath).ToIOUri());

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

        [TestMethod]
        public async Task Params_file_should_handle_registry_module_resource_derived_types()
        {
            const string moduleRef = "br:mockregistry.io/route/table:v1";

            const string moduleContent = """
                param routes resourceInput<'Microsoft.Network/routeTables@2024-07-01'>.properties.routes?
            """;

            const string paramsContent = $@"using '{moduleRef}'
                param routes = [
                    {{
                        id: 'myroute'
                        properties: {{
                            addressPrefix: '0.0.0.0/0'
                            nextHopType: 'Internet'
                        }}
                    }}
                ]
            ";

            var artifactManager = await MockRegistry.CreateDefaultExternalArtifactManager(TestContext);
            await artifactManager.PublishRegistryModule(moduleRef, moduleContent);

            var paramsFilePath = FileHelper.SaveResultFile(TestContext, "main.bicepparam", paramsContent);
            var fileUri = PathHelper.FilePathToFileUrl(paramsFilePath);

            var services = await CreateServicesAsync();
            services = services.WithTestArtifactManager(artifactManager);

            var compiler = services.Build().GetCompiler();
            var compilation = await compiler.CreateCompilation(fileUri.ToIOUri());

            var diagnostics = compilation.GetEntrypointSemanticModel().GetAllDiagnostics().ExcludingLinterDiagnostics();

            diagnostics.Should().BeEmpty();
        }

        private async Task<ServiceBuilder> CreateServicesAsync()
            => new ServiceBuilder()
                .WithFeatureOverrides(new(TestContext))
                .WithEnvironmentVariables(
                    ("stringEnvVariableName", "test"),
                    ("intEnvVariableName", "100"),
                    ("boolEnvironmentVariable", "true")
                )
                .WithTestArtifactManager(await MockRegistry.CreateDefaultExternalArtifactManager(TestContext));
    }
}
