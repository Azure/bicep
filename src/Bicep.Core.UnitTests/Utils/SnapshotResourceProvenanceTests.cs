// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.Utils.Snapshots;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Utils
{
    [TestClass]
    public class SnapshotResourceProvenanceTests
    {
        private const string TemplateJson = """
            {
              "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
              "contentVersion": "1.0.0.0",
              "languageVersion": "2.0",
              "resources": {
                "stg": {
                  "type": "Microsoft.Storage/storageAccounts",
                  "apiVersion": "2023-01-01",
                  "name": "provstorage",
                  "location": "eastus",
                  "kind": "StorageV2",
                  "sku": { "name": "Standard_LRS" }
                }
              }
            }
            """;

        private const string EmptyParametersJson = """
            { "contentVersion": "1.0.0.0", "parameters": {} }
            """;

        [TestMethod]
        public async Task Snapshot_ResourceProvenanceIsIndexAlignedAndNotSerialized()
        {
            var result = await GetSnapshotAsync();

            result.Snapshot.PredictedResources.Should().ContainSingle();
            var provenance = result.ResourceProvenance.Should().ContainSingle().Subject;
            provenance.SymbolicName.Should().Be("stg");
            provenance.Path.Should().Contain("resources.stg");
            provenance.IsExtensible.Should().BeFalse();

            Serialize(result.Snapshot).Should().NotContain("resourceProvenance");
        }

        [TestMethod]
        public void Snapshot_AnalyzerOnlyExtensibleResourceDoesNotChangeCustomerSerialization()
        {
            var customerResource = JsonDocument.Parse("""{"type":"example/customer"}""").RootElement.Clone();
            var analyzerResource = JsonDocument.Parse("""{"type":"example/customer","bicepSymbolicName":"extensionResource"}""").RootElement.Clone();
            var baseline = new Snapshot([customerResource], [], []);
            var enriched = new SnapshotAnalysisResult(
                baseline,
                [analyzerResource],
                [new SnapshotResourceProvenance("extensionResource", null, null, null, "resources.extensionResource", 1, 1, true)]);

            enriched.ExtensibleResources.Should().ContainSingle().Which
                .GetProperty("bicepSymbolicName").GetString().Should().Be("extensionResource");
            Serialize(enriched.Snapshot).Should().Be(Serialize(baseline));
        }

                [TestMethod]
                public async Task Snapshot_NestedExtensibleResourceCarriesCanonicalTemplatePath()
                {
                        const string extensionArtifact = "example.azurecr.io/extensions/http:1.2.3";
                        var fileSet = MockFileSystemTestFileSet.Create(
                                ("bicepconfig.json", $$"""
                                        {
                                            "extensions": {
                                                "http": "br:{{extensionArtifact}}"
                                            }
                                        }
                                        """),
                                ("main.bicep", """
                                        module child './child.bicep' = {
                                            name: 'child'
                                        }
                                        """),
                                ("child.bicep", """
                                        extension http

                                        resource request 'request@v1' = {
                                            uri: 'https://example.com/forecast'
                                            format: 'raw'
                                        }
                                        """));
                        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(
                                ExtensionResourceTypeHelper.GetHttpExtensionTypesTgz(),
                                new FeatureProviderOverrides(),
                                fileSet.FileSystem,
                                extensionArtifact);
                        var compiler = services
                                .WithFileExplorer(fileSet.FileExplorer)
                                .WithFileSystem(fileSet.FileSystem)
                                .Build()
                                .GetCompiler();
                        var compilation = await compiler.CreateCompilation(fileSet.GetUri("main.bicep"));
                        var model = compilation.GetEntrypointSemanticModel();
                        model.GetAllDiagnostics().Where(diagnostic => diagnostic.Level == DiagnosticLevel.Error).Should().BeEmpty();
                        var templateJson = new TemplateEmitter(model).EmitWithForcedSymbolicNames();

                        var result = await SnapshotHelper.GetSnapshotForAnalysis(
                                targetScope: model.TargetScope,
                                templateContent: templateJson!,
                                parametersContent: EmptyParametersJson,
                                tenantId: null,
                                managementGroupId: null,
                                subscriptionId: "00000000-0000-0000-0000-000000000000",
                                resourceGroup: "rg",
                                deploymentName: null,
                                location: "eastus",
                                externalInputs: [],
                                cancellationToken: default);

                        result.ExtensibleResources.Should().ContainSingle();
                        var provenance = result.ResourceProvenance.Should().ContainSingle().Subject;
                        provenance.SymbolicName.Should().Be("request");
                        provenance.Path.Should().Be("resources.child.properties.template.resources.request");
                        provenance.IsExtensible.Should().BeTrue();
                }

        private static Task<SnapshotAnalysisResult> GetSnapshotAsync() => SnapshotHelper.GetSnapshotForAnalysis(
            targetScope: ResourceScope.ResourceGroup,
            templateContent: TemplateJson,
            parametersContent: EmptyParametersJson,
            tenantId: null,
            managementGroupId: null,
            subscriptionId: "00000000-0000-0000-0000-000000000000",
            resourceGroup: "rg",
            deploymentName: null,
            location: "eastus",
            externalInputs: [],
            cancellationToken: default);

        private static string Serialize(Snapshot snapshot) => JsonSerializer.Serialize(snapshot, SnapshotSerializationContext.FileSerializer.Snapshot);
    }
}